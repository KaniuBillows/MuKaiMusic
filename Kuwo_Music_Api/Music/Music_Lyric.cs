using System;
using System.Collections;

namespace MusicApi.Kuwo.Music
{
    public class Music_Lyric : BaseRequestOption
    {
        public Music_Lyric(Hashtable cookies, int musicId) : base(cookies)
        {
            var id = Guid.NewGuid();
            this.Url = $"http://m.kuwo.cn/newh5/singles/songinfoandlrc?musicId={musicId}&&reqId={id.ToString()}";
        }

        public Music_Lyric(int musicId) : this(new Hashtable(), musicId)
        {

        }

        public override string Url { get; }

        public override CryptoType Crypto => CryptoType.KuWo_Web;

        public override string Ua => "pc";

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override string OptionUrl { get; }
    }
}
