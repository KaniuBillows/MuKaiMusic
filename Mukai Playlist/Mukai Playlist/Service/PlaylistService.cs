using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Music;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Mukai_Playlist.Service
{
    public class PlaylistService
    {
        private readonly IMongoCollection<UserMusic> userMusicCollection;

        public PlaylistService(IMongoDatabase db)
        {
            this.userMusicCollection = db.GetCollection<UserMusic>("user_musics");
        }
        
        /// <summary>
        /// 创建歌单
        /// </summary>
        /// <param name="playlist">歌单对象</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<Result<UserPlaylist>> CreateUserPlaylist(UserPlaylist playlist, long userId)
        {
            playlist.UserId = userId;
            playlist.CreateTime = DateTime.Now;
            playlist.PlayedCount = 0;
            playlist.Id = ObjectId.GenerateNewId().ToString();
            var update = Builders<UserMusic>.Update.SetOnInsert(usr => usr.UserId, userId)
                .AddToSet(usr => usr.UserPlaylists, playlist);
            var filter = Builders<UserMusic>.Filter.Eq("_id", userId);
            var result = await this.userMusicCollection.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = true
            });
            return result.ModifiedCount > 0 ? Result<UserPlaylist>.SuccessReuslt(playlist) :
               result.IsAcknowledged ? Result<UserPlaylist>.SuccessReuslt(playlist) : Result<UserPlaylist>.FailResult();
        }

        /// <summary>
        /// 添加歌曲进入歌单
        /// </summary>
        /// <returns></returns>
        public async Task<Result> AddMusicToPlaylist(long userId, string listId, List<MusicInfo> musicInfos)
        {
            if (!await ValidPlaylistOwner(userId, listId))
            {
                return Result.FailResult("您不是歌单的拥有者哦", 403);
            }
            musicInfos.ForEach(mic =>
            {
                mic.Sid = mic.GetSid();
                if (mic.DataSource != DataSource.Migu)
                    mic.Url = null;
            });
            var update = Builders<UserMusic>.Update.AddToSetEach("playlists.$.tracks", musicInfos);
            var filter = Builders<UserMusic>.Filter.ElemMatch(usr => usr.UserPlaylists,
               Builders<UserPlaylist>.Filter.Eq(list => list.Id, listId));
            var updateResult = await this.userMusicCollection.UpdateOneAsync(filter, update);
            return updateResult.MatchedCount > 0
                ? Result.SuccessReuslt("添加成功") : Result.FailResult("添加失败，目标不存在！", 404);
        }

        /// <summary>
        /// 从歌单中删除某些歌曲
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listId"></param>
        /// <param name="sids">Sid，(string)DataSource_(string)Id格式</param>
        /// <returns></returns>
        public async Task<Result> RemoveMusicFromList(long userId, string listId, List<string> sids)
        {
            if (!await ValidPlaylistOwner(userId, listId))
            {
                return Result.FailResult("您不是歌单的拥有者哦", 403);
            }
            //聚合查询元素所在索引位置
            IPipelineStageDefinition stage0 = new JsonPipelineStageDefinition<UserMusic, UserMusic>
            ("{\"$match\":{\"_id\" :" + userId.ToString() + "}}");
            IPipelineStageDefinition stage1 = new JsonPipelineStageDefinition<UserMusic, IndexInfo>
            ("{ \"$project\": { \"index\": { \"$indexOfArray\":[\"$playlists._id\",ObjectId(\"" + listId + "\")] },\"_id\":0}}");

            var pipeline = new PipelineStagePipelineDefinition<UserMusic, IndexInfo>
               (new List<IPipelineStageDefinition>() { stage0, stage1 });
            var indexResult = (await this.userMusicCollection.AggregateAsync(pipeline)).FirstOrDefault();
            if (indexResult == null) return Result.FailResult("操作失败，目标不存在！", 404);
            var update = Builders<UserMusic>.Update.PullFilter<MusicInfo>(
               $"playlists.{indexResult.Index}.tracks",
                    Builders<MusicInfo>.Filter.In(mic => mic.Sid, sids));
            var updateResult = await this.userMusicCollection.UpdateManyAsync(
                Builders<UserMusic>.Filter.Eq(usr => usr.UserId, userId), update);
            return updateResult.MatchedCount > 0
                 ? Result.SuccessReuslt("操作成功") : Result.FailResult("操作失败，目标不存在！", 404);
        }

        /// <summary>
        /// 更新歌单信息，只允许对歌单名字，是否公开，封面进行修改
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listId"></param>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public async Task<Result> UpdatePlaylist(long userId, string listId, UserPlaylist playlist)
        {
            if (!await ValidPlaylistOwner(userId, listId))
            {
                return Result.FailResult("您不是歌单的拥有者哦", 403);
            }
            var updates = new List<UpdateDefinition<UserMusic>>();
            if (playlist.Name != null)
            {
                updates.Add(Builders<UserMusic>.Update.Set("playlists.$.name", playlist.Name));
            }
            if (playlist.Public.HasValue)
            {
                updates.Add(Builders<UserMusic>.Update.Set("playlists.$.public", playlist.Public.Value));
            }
            if (playlist.PicUrl != null)
            {
                updates.Add(Builders<UserMusic>.Update.Set("playlists.$.picUrl", playlist.PicUrl));
            }
            var update = Builders<UserMusic>.Update.Combine(updates);
            var filter = Builders<UserMusic>.Filter.ElemMatch(usr => usr.UserPlaylists,
                Builders<UserPlaylist>.Filter.Eq(list => list.Id, listId));
            var updateResult = await this.userMusicCollection.UpdateOneAsync(filter, update);
            return updateResult.MatchedCount > 0
                 ? Result.SuccessReuslt("更新成功") : Result.FailResult("更新失败，目标不存在！");
        }

        /// <summary>
        /// 删除歌单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task<Result> DeletePlaylist(long userId, string listId)
        {
            if (!await ValidPlaylistOwner(userId, listId))
            {
                return Result.FailResult("您不是歌单的拥有者哦", 403);
            }
            var update = Builders<UserMusic>.Update.PullFilter<UserPlaylist>(usr => usr.UserPlaylists,
              Builders<UserPlaylist>.Filter.Eq(list => list.Id, listId));
            var updateResult = await this.userMusicCollection.UpdateOneAsync(usr => usr.UserId == userId, update);
            return updateResult.MatchedCount > 0 ? Result.SuccessReuslt("删除歌单成功") :
                Result.FailResult("歌单不存在！", 404);
        }

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> ValidUserExist(long userId)
        {
            var filter = Builders<UserMusic>.Filter.Eq(usr => usr.UserId, userId);
            var projection = Builders<UserMusic>.Projection.Include(usr => usr.UserId);
            return (await this.userMusicCollection.Find(filter).Project(projection).FirstOrDefaultAsync()) != null;
        }

        /// <summary>
        /// 获取用户歌单详情,包含完整数据，如果歌单不为公开，则会验证用户是否拥有歌单
        /// </summary>
        /// <param name="loginUserId">当前登录用户，如果为歌单的拥有者，则可访问私有歌单</param>
        /// <param name="id">希望获取的歌单Id</param>
        /// <returns>歌单不存在或者不为公开，则为null</returns>
        public async Task<UserPlaylist> GetUserPlaylistDetail(long loginUserId, string id)
        {
            //歌单的创建者可以访问，或者歌单为公开可以访问

            var projection = Builders<UserMusic>.Projection.ElemMatch(
                u => u.UserPlaylists,
                    Builders<UserPlaylist>.Filter.And(
                        Builders<UserPlaylist>.Filter.Eq(list => list.Id, id),
                        Builders<UserPlaylist>.Filter.Or(
                            Builders<UserPlaylist>.Filter.Eq(list => list.Public, true),
                            Builders<UserPlaylist>.Filter.Eq("userId", loginUserId))
                        ));
            var userMusicInfo = await this.userMusicCollection.Find(Builders<UserMusic>.Filter.Empty)
                 .Project<UserMusic>(projection).FirstOrDefaultAsync();
            if (userMusicInfo == null) return null;
            return userMusicInfo.UserPlaylists == null || userMusicInfo.UserPlaylists.Count == 0 ? null
                : userMusicInfo.UserPlaylists.First();
        }

        /// <summary>
        /// 获取用户创建的歌单,不包含音乐列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<PageResult<UserPlaylist>> GetUserPlaylist(long userId, int limit, int offset)
        {
            //查询用户歌单，排除不需要的字段
            var projection = Builders<UserMusic>.Projection.ElemMatch(usr => usr.UserPlaylists,
                    Builders<UserPlaylist>.Filter.Eq(list => list.UserId, userId))
                .Slice(usr => usr.UserPlaylists, offset, limit)
                .Exclude(usr => usr.Likes)
                .Exclude(usr => usr.SubscribePlaylists)
                .Exclude("playlists.tracks");
            Task<UserMusic> list_task = this.userMusicCollection.Find(item => item.UserId == userId)
                  .Project<UserMusic>(projection).FirstOrDefaultAsync();

            //统计总数
            var aggregateOption = new AggregateOptions();
            //管道件一，输入类型UserMusic，筛选出对应_id的用户
            IPipelineStageDefinition stage0 = new JsonPipelineStageDefinition<UserMusic, UserMusic>
                ("{\"$match\":{\"_id\" :" + userId.ToString() + "}}");
            //管道件二，通过$size聚合得到数组长度，输出类型为内部类CountInfo
            IPipelineStageDefinition stage1 = new JsonPipelineStageDefinition<UserMusic, CountInfo>
                ("{ \"$project\": { \"count\": { \"$size\":\"$playlists\" },\"_id\":0}}");
            //生成管道
            var pipeline = new PipelineStagePipelineDefinition<UserMusic, CountInfo>
                (new List<IPipelineStageDefinition>() { stage0, stage1 });
            var count_task = this.userMusicCollection.AggregateAsync(pipeline);

            List<UserPlaylist> playlists = (await list_task)?.UserPlaylists;
            var countInfo = (await count_task).FirstOrDefault();
            return playlists == null ? PageResult<UserPlaylist>.FailResult("用户不存在！", 404)
             : PageResult<UserPlaylist>.SuccessResult(playlists, countInfo != null ? countInfo.Count : 0);
        }

        /// <summary>
        /// 验证用户是否拥有这个歌单
        /// </summary>
        /// <param name="userId">待验证用户</param>
        /// <param name="listId">待验证歌单</param>
        /// <returns></returns>
        public async Task<bool> ValidPlaylistOwner(long userId, string listId)
        {
            var listFilter = Builders<UserPlaylist>.Filter.And(
                Builders<UserPlaylist>.Filter.Eq(list => list.Id, listId),
                Builders<UserPlaylist>.Filter.Eq(list => list.UserId, userId));
            var projection = Builders<UserMusic>.Projection.ElemMatch(usr => usr.UserPlaylists, listFilter)
                .Exclude(usr => usr.UserPlaylists)
                .Exclude(usr => usr.SubscribePlaylists)
                .Exclude(usr => usr.Likes);
            var result = await this.userMusicCollection.Find(usr => usr.UserId == userId)
                .Project<UserMusic>(projection).FirstOrDefaultAsync();
            return result != null;
        }

        /// <summary>
        /// 统计数量信息内部类
        /// </summary>
        private class CountInfo
        {
            [BsonElement("count")]
            public int Count { get; set; }
        }

        /// <summary>
        /// 查询元素索引位置内部类
        /// </summary>
        private class IndexInfo
        {
            [BsonElement("index")]
            public int Index { get; set; }
        }
    }

}