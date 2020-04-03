using MusicApi;
using System;
using System.Collections;
using System.Text;

namespace MiGu_Music_API.Music
{
    public class Mobile_Search : BaseRequestOption
    {
        public Mobile_Search(Hashtable cookies, string keyword) : base(cookies)
        {
            this.Params.Add("keyword", keyword);
            this.Params.Add("pgc", 1);
            this.Params.Add("rows", 20);
            this.Params.Add("type", "song");
        }

        public override string Url => "http://m.music.migu.cn/migu/remoting/scr_search_tag";

        public override CryptoType Crypto => CryptoType.MiGU_Mobile;

        public override string Ua => "Andorid_migu";

        public override HttpMethod HttpMethod => HttpMethod.GET;

        public override string OptionUrl => "";

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
            return builder.ToString();
        }
    }
}
