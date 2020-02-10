using MusicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MiGu_Music_API.Music
{
    public class Web_Lyric : BaseRequestOption
    {
        public Web_Lyric(Hashtable cookies, string id) : base(cookies)
        {
            this.Url = $"http://music.migu.cn/v3/api/music/audioPlayer/getLyric?copyrightId={id}";
        }

        public Web_Lyric(string id) : this(new Hashtable(), id)
        {

        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.MiGU_Web;

        public override string Ua => "pc";

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override string OptionUrl { get; }
    }
}
