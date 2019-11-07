using RequestHandler;
using System.Collections;

namespace NetEaseMusic_API.RequestOption.Options.Search
{
    public enum SearchType
    {
        Song = 1,
        Album = 10,
        Singer = 100,
        MusicList = 1000,
        User = 1002,
        MV = 1004,
        Lyric = 1006,
        Radio = 1009,
        Video = 1014,
    }

    public sealed class Search : BaseRequestOption
    {
        public Search(Hashtable cookie, string keyword, SearchType searchType, int limit, int offset) : base(cookie)
        {
            Params.Add("s", keyword);
            Params.Add("type", (int)searchType);
            Params.Add("limit", limit);
            Params.Add("offset", offset);
        }

        public Search(Hashtable cookie, string keyword) : this(cookie, keyword, SearchType.Song, 30, 0)
        {
        }

        public Search(Hashtable cookie, string keyword, SearchType searchType) : this(cookie, keyword, searchType, 30, 0)
        {
        }

        public Search(string keyword, SearchType searchType, int limit, int offset) : this(new Hashtable(), keyword, searchType, limit, offset)
        {

        }

        public Search(string keyword) : this(new Hashtable(), keyword)
        {
        }

        public override string Url => "https://music.163.com/weapi/search/get";

        public override CryptoType Crypto => CryptoType.Netease_weapi;

        public override string Ua { get; }

        public override HttpMethod HttpMethod => HttpMethod.POST;

        public override string OptionUrl { get; }
    }
}