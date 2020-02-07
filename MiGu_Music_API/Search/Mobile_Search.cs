using System;
using System.Collections;
using System.Text;

namespace MusicApi.Migu.Search
{
    public class Mobile_Search : BaseRequestOption
    {
        public Mobile_Search(Hashtable cookies) : base(cookies)
        {
        }

        public Mobile_Search(int pageNo, int pageSize, string keyword) : this(new Hashtable())
        {
            Params.Add("isCopyright", 1);
            Params.Add("isCorrect", 1);
            Params.Add("pageNo", pageNo);
            Params.Add("pageSize", pageSize);
            Params.Add("text", keyword);
            Params.Add("sort", 0);
        }

        public override string Url { get; } = "https://app.c.nf.migu.cn/MIGUM2.0/v1.0/content/search_all.do?";

        public override CryptoType Crypto { get; } = CryptoType.MiGU_Mobile;

        public override string Ua { get; } = "Andorid_migu";

        public override HttpMethod HttpMethod { get; } = HttpMethod.GET;

        public override string OptionUrl { get; }

        public override string GetQueryString()
        {
            string query = string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (string key in this.Params.Keys)
            {
                builder.Append(key);
                builder.Append("=");
                builder.Append(Uri.EscapeDataString(Params[key].ToString()));
                builder.Append("&");
                //query += key + "=" + Uri.EscapeDataString(Params[key].ToString()) + "&";
            }
            builder.Append("searchSwitch={%22song%22:1,%22album%22:0,%22singer%22:0,%22tagSong%22:1,%22mvSong%22:0,%22songlist%22:0,%22bestShow%22:1}");
            //query += "searchSwitch={%22song%22:1,%22album%22:0,%22singer%22:0,%22tagSong%22:1,%22mvSong%22:0,%22songlist%22:0,%22bestShow%22:1}";
            return builder.ToString();
        }
    }
}
