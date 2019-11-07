using RequestHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetEaseMusic_API.RequestOption.Options.Playlist
{
    public class RecommendPlaylist : BaseRequestOption
    {
        public RecommendPlaylist(Hashtable cookies) : base(cookies)
        {
        }

        public override string Url => "https://music.163.com/weapi/v1/discovery/recommend/resource";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
