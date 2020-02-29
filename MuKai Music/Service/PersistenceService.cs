using MuKai_Music.DataContext;
using MuKai_Music.Model.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Service
{
    /// <summary>
    /// 持久化音乐数据
    /// </summary>
    public class PersistenceService
    {
        private readonly MusicContext _context;
        public PersistenceService(MusicContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// 添加音乐实体
        /// </summary>
        /// <param name="musicInfo"></param>
        /// <returns></returns>
        public async Task AddMusicInfo(MusicInfo musicInfo)
        {
            await this._context.MusicInfos.AddAsync(musicInfo);
            await this._context.SaveChangesAsync();
        }

    }
}
