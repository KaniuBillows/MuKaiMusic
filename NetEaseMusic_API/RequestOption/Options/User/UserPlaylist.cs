using RequestHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetEaseMusic_API.RequestOption.Options.User
{
    /// <summary>
    /// 用户歌单
    /// </summary>
    public class UserPlaylist : BaseRequestOption
    {
        public UserPlaylist(Hashtable cookies, int userId, int limit, int offset) : base(cookies)
        {
            this.Params.Add("uid", userId);
            this.Params.Add("limit", limit);
            this.Params.Add("offset", offset);
        }

        public UserPlaylist(int userId, int limit, int offset) : this(new Hashtable(), userId, limit, offset)
        {

        }

        public override string Url => "https://music.163.com/weapi/user/playlist";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}
