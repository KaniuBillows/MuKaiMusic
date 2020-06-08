using System.Collections.Generic;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Music;
using Xunit;
namespace Test
{
    public class Playlist_UnitTest : BaseTest
    {
        /// <summary>
        /// ���Դ����û��赥
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateUserPlaylist()
        {
            // Arrange
            var userId = 32;
            var list = new UserPlaylist()
            {
                Name = "һ��һ�׳�����",
                PicUrl = "https://p2.music.126.net/W6cLGyzro66Q2MYy3TYU8w==/109951162869093110.jpg",
                Tracks = new List<DataAbstract.MusicInfo>(),
                Public = true
            };

            var result = await Client.PostAsync<UserPlaylist, TestResult<UserPlaylist>>
               ($"/api/playlist/user/create?loginUserId={userId}", list);
            Assert.Equal(200, result.Code);
            Assert.NotNull(result.Content.Id);
        }

        /// <summary>
        /// ���Ը��¸赥��Ϣ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateUserPlaylist()
        {
            var userId = 4;
            var listId = "5edb579918e5443984d76a12";
            var updateList = new UserPlaylist()
            {
                Name = "�����Ҿͷ�����",
                Public = false
            };
            var result = await Client.PutAsync<UserPlaylist, TestResult<object>>
                ($"/api/playlist/user/update?loginUserId={userId}&id={listId}", updateList);
            // Assert
            Assert.Equal(200, result.Code);
        }


        /// <summary>
        /// ���Ը��ݸ赥Id��ȡ�û��赥
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UserPlaylistGet()
        {
            // Arrange
            var userId = 4; var lid = "5edb579918e5443984d76a12";
            // Act 
            var result = await Client.GetAsync<TestResult<UserPlaylist>>
                ($"/api/playlist/user/detail?loginUserId={userId}&id={lid}");
            // Assert 
            if (result.Code == 200)
                Assert.NotNull(result.Content);
            else
                Assert.NotNull(result.Message);

        }

        /// <summary>
        /// ��ȡ�û�������ȫ���赥��������������Ŀ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UserPlaylistsGet()
        {
            // Arrange
            var userId0 = 1;
            int limit = 10, offset = 0;
            //Act
            var result0 = await Client.GetAsync<TestPageResult<UserPlaylist>>
                ($"api/playlist/user/list?userId={userId0}&limit={limit}&offset={offset}");
            Assert.Equal(200, result0.Code);

            // Assert
            var userId1 = 999;
            var result1 = await Client.GetAsync<TestPageResult<UserPlaylist>>
                ($"api/playlist/user/list?userId={userId1}&limit={limit}&offset={offset}");
            Assert.Equal(404, result1.Code);
        }


        /// <summary>
        /// ���û��赥����Ӹ�������
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddMusicsToPlaylist()
        {
            // Arrange 
            var userId = 32; var listId = "5eddec7fa270ef17d4a556f7";
            List<MusicInfo> musicInfos = new List<MusicInfo>
            {
                new MusicInfo()
                {
                    DataSource = DataSource.Kuwo,
                    Name = "������",
                    Duration = 298,
                    Kw_Id = 94237,
                    Album = new Album()
                    {
                        Name = "������",
                        Id = null,
                        PicUrl = "http://img2.kuwo.cn/star/albumcover/300/30/97/4276557883.jpg"
                    },
                    Artists = new System.Collections.ObjectModel.Collection<Artist>()
                {
                    new Artist
                    {
                        Name="�ܽ���",
                        Id= 336
                    }
                }
                },
                new MusicInfo()
                {
                    DataSource = DataSource.Kuwo,
                    Name = "ʮ��",
                    Duration = 206,
                    Kw_Id = 80403,
                    Artists = new System.Collections.ObjectModel.Collection<Artist>()
                {
                    new Artist()
                    {
                        Name = "����Ѹ",
                        Id = 47
                    }
                },
                    Album = new Album()
                    {
                        Name = "�ڰ׻�",
                        PicUrl = "http://img1.kuwo.cn/star/albumcover/300/91/30/2595482136.jpg"
                    }
                },
                new MusicInfo()
                {
                    Name ="һ�����",
                    Ne_Id = 153784,
                    Duration= 264,
                    DataSource = DataSource.NetEase,
                    Artists = new System.Collections.ObjectModel.Collection<Artist>()
                    {
                      new Artist(){
                          Id= 5205,
                            Name= "̷ӽ��"
                      }
                    },
                    Album = new Album()
                    {
                        Id=15455,
                        Name = "һ�����",
                        PicUrl= "https://p1.music.126.net/EDe8WuEF_RbxugBQJcofXQ==/41781441856503.jpg"
                    }
                }
            };
            var result = await this.Client.PostAsync<List<MusicInfo>, TestResult<object>>
                ($"/api/playlist/user/insertToList?loginUserId={userId}&id={listId}", musicInfos);

            Assert.NotEmpty(result.Message);
        }

        /// <summary>
        /// ɾ���赥����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePlaylist()
        {
            var userId = 18; string listId = "5edb8b83c011f026b48ec2bf";
            var result = await this.Client.DeleteAsync<TestResult<object>>
                ($"api/playlist/user/delete?loginUserId={userId}&id={listId}");

            Assert.NotNull(result.Message);
        }

        /// <summary>
        /// �Ӹ赥���Ƴ�����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteMusicFromPlaylist()
        {
            var userId = 32; var listId = "5eddec7fa270ef17d4a556f7";
            List<string> sids = new List<string>()
            {
                "Kuwo_94237","NetEase_153784"
            };
            TestResult<object> result = await this.Client.PostAsync<List<string>, TestResult<object>>
                ($"api/playlist/user/removeFromList?loginUserId={userId}&id={listId}", sids);

            Assert.Equal(200, result.Code);
        }
    }
}
