﻿using System.Collections;

namespace MusicApi.NetEase.Music
{
    /// <summary>
    /// 获取歌词
    /// </summary>
    public sealed class GetLyric : BaseRequestOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="musicId"></param>
        public GetLyric(Hashtable cookies, int musicId) : base(cookies)
        {
            Params.Add("id", musicId.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="musicId"></param>
        public GetLyric(int musicId) : this(new Hashtable(), musicId)
        {
        }

        public override string Url => "https://music.163.com/weapi/song/lyric?lv=-1&kv=-1&tv=-1";

        public override CryptoType Crypto => CryptoType.Netease_linuxApi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}