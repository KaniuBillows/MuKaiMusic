using MusicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MiGu_Music_API.Music
{
    public class Web_Music_Pic : BaseRequestOption
    {
        public Web_Music_Pic(Hashtable cookies, string musicId) : base(cookies)
        {
            this.Url = $"http://music.migu.cn/v3/api/music/audioPlayer/getSongPic?songId={musicId}";
        }

        public Web_Music_Pic(string musicId) : this(new Hashtable(), musicId)
        {

        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.MiGU_Web;

        public override string Ua { get; }

        public override HttpMethod HttpMethod { get; }

        public override string OptionUrl { get; }
    }
}
