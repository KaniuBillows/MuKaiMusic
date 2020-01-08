using Encrypt;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestHandler
{
    internal class RequestSender
    {
        const string Netease_base62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const string Netease_iv = "0102030405060708";
        const string Netease_presetKey = "0CoJUm6Qyw8W8jud";
        const string Netease_linuxapiKey = "rFgB&h#%2?^eDg:Q";
        const string Netease_eapiKey = "e82ckenh8dichen8";

        private const string Netease_publicKey =
            "-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDgtQn2JZ34ZC28NWYpAUd98iZ37BUrX/aKzmFbt7clFSs6sXqHauqKWqdtLkF2KexO40H1YTX8z2lSgBBOAxLsvaklV8k4cBFK9snQXE9/DDaFt6Rr7iVZMldczhC0JNgTz+SHXT6CBHuX3e9SdB1Ua44oncaTWz7OBGLbCiK45wIDAQAB\n-----END PUBLIC KEY-----";

        public Task<HttpResponseMessage> Request(IRequestOption request)
        {
            throw new NotImplementedException();
        }

        private const string Migu_publicKey =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC8asrfSaoOb4je+DSmKdriQJKW\nVJ2oDZrs3wi5W67m3LwTB9QVR+cE3XWU21Nx+YBxS0yun8wDcjgQvYt625ZCcgin\n2ro/eOkNyUOTBIbuj9CvMnhUYiR61lC1f1IGbrSYYimqBVSjpifVufxtx/I3exRe\nZosTByYp4Xwpb1+WAQIDAQAB";
        private static readonly string[] UserAgentList =
            {
        "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
        "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
        "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
        "Mozilla/5.0 (Linux; Android 5.1.1; Nexus 6 Build/LYZ28E) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Mobile/14F89 GameHelper",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1",
        "Mozilla/5.0 (iPad; CPU OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.12; rv:46.0) Gecko/20100101 Firefox/46.0",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/603.2.4 (KHTML, like Gecko) Version/10.1.1 Safari/603.2.4",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:46.0) Gecko/20100101 Firefox/46.0",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/13.10586"};

        public static async Task<HttpResponseMessage> Send(IRequestOption requestOption)
        {
            using HttpClient httpClient = HttpClientFactory.Create();
            StringContent content = null;
            switch (requestOption.Crypto)
            {
                case CryptoType.Netease_weapi:
                    {
                        requestOption.Params.Add("csrf_token", (string)requestOption.Cookies["_csrf"] ?? "");
                        string url = Regex.Replace(requestOption.Url, "\\w*api", "weapi");
                        httpClient.BaseAddress = new Uri(url);
                        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, GetUserAgent(requestOption));
                        content = GetWeapiParams(requestOption.Params);
                        if (requestOption.HttpMethod == HttpMethod.POST) content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        if (requestOption.Url.Contains("music.163.com")) httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://music.163.com");
                        if (requestOption.Cookies.Count > 0) httpClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, GetCookieString(requestOption.Cookies));
                    }
                    break;
                case CryptoType.Netease_linuxApi:
                    {
                        httpClient.BaseAddress = new Uri("https://music.163.com/api/linux/forward");
                        httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36");
                        string url = Regex.Replace(requestOption.Url, "\\w*api", "api");
                        var ob = new { method = requestOption.HttpMethod.ToString(), url, @params = requestOption.Params };
                        content = GetLinuxParams(ob);
                        if (requestOption.Url.Contains("music.163.com")) httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://music.163.com");
                        if (requestOption.HttpMethod == HttpMethod.POST) content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        if (requestOption.Cookies.Count > 0) httpClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, GetCookieString(requestOption.Cookies));
                    }
                    break;
                case CryptoType.Netease_eapi:
                    {
                        string url = requestOption.Url;
                        Regex.Replace(url, "\\w*api", "eapi");
                        httpClient.BaseAddress = new Uri(url);
                        if (requestOption.Url.Contains("music.163.com")) httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://music.163.com");
                        var hashtable = new Hashtable
                            {
                                { "osver", requestOption.Cookies["osver"]},
                                { "deviceId", requestOption.Cookies["deviceId"]},
                                { "appver", requestOption.Cookies["appver"] ?? "6.11" },
                                { "versioncode", requestOption.Cookies["versioncode"] ?? "140" },
                                { "mobilename", requestOption.Cookies["mobilename"]},
                                { "buildver", requestOption.Cookies["buildver"] ?? new DateTime().ToString(CultureInfo.InvariantCulture).Substring(0, 10) },
                                { "resolution", requestOption.Cookies["resolution"] ?? "1920x1080" },
                                { "__csrf", requestOption.Cookies["__csrf"] ?? "" },
                                { "os", requestOption.Cookies["os"] ?? "android" },
                                { "channel", requestOption.Cookies["channel"] },
                                { "requestId", $"{new DateTime().ToString(CultureInfo.InvariantCulture)}_{$"{new DateTime().ToString(CultureInfo.InvariantCulture)}_{new Random().Next(0, 1000).ToString().PadLeft(4, '0')}"}" }
                            };
                        if (requestOption.Cookies["MUSIC_A"] != null)
                        {
                            hashtable.Add("MUSIC_A", requestOption.Cookies["MUSIC_A"]);
                        }
                        if (requestOption.Cookies["MUSIC_U"] != null)
                        {
                            hashtable.Add("MUSIC_U", requestOption.Cookies["MUSIC_U"]);
                        }
                        httpClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, GetCookieString(hashtable));
                        requestOption.Params.Add("header", hashtable);
                        content = GetEapiParams(requestOption.OptionUrl, requestOption.Params);
                        if (requestOption.HttpMethod == HttpMethod.POST) httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, GetUserAgent(requestOption));
                        if (requestOption.Url.Contains("music.163.com")) httpClient.DefaultRequestHeaders.Add(HeaderNames.Referer, "https://music.163.com");
                    }
                    break;
                case CryptoType.MiGU_Mobile:
                    {
                        httpClient.BaseAddress = new Uri(requestOption.Url + requestOption.GetQueryString());
                        httpClient.DefaultRequestHeaders.Add("ua", requestOption.Ua);
                        httpClient.DefaultRequestHeaders.Add("User-Agent", GetUserAgent(requestOption));
                        httpClient.DefaultRequestHeaders.Add("channel", "0231111");
                        return await httpClient.GetAsync(httpClient.BaseAddress);
                    }
                case CryptoType.MiGU_Web:
                    {
                        var s = requestOption.GetQueryString();
                        httpClient.BaseAddress = new Uri(requestOption.Url + s);
                        httpClient.DefaultRequestHeaders.Add("ua", requestOption.Ua);
                        httpClient.DefaultRequestHeaders.Add("User-Agent", GetUserAgent(requestOption));
                        httpClient.DefaultRequestHeaders.Add("Origin", "http://music.migu.cn/");
                        httpClient.DefaultRequestHeaders.Add("Referer", "http://music.migu.cn/");
                        httpClient.DefaultRequestHeaders.Add("channel", "0231111");
                        return await httpClient.GetAsync(httpClient.BaseAddress);
                    }
                case CryptoType.KuWo_Web:
                    {
                        httpClient.BaseAddress = new Uri(requestOption.Url + requestOption.GetQueryString());
                        httpClient.DefaultRequestHeaders.Add("ua", requestOption.Ua);
                        httpClient.DefaultRequestHeaders.Add("User-Agent", GetUserAgent(requestOption));
                        httpClient.DefaultRequestHeaders.Add("Referer", "http://www.kuwo.cn");
                        return await httpClient.GetAsync(httpClient.BaseAddress);
                    }
            }
            var result = await httpClient.PostAsync(httpClient.BaseAddress, content);
            return result;
        }

        private static string GetUserAgent(IRequestOption option)
        {
            int index;
            if (option.Ua == null) index = new Random().Next(0, UserAgentList.Length - 1);
            else if (option.Ua == "mobile" || option.Ua.Contains("Andorid")) index = new Random().Next(0, 8);
            else if (option.Ua == "pc") index = new Random().Next(8, 13);
            else return option.Ua;
            return UserAgentList[index];
        }

        private static StringContent GetWeapiParams(object data)
        {

            var text = JsonSerializer.Serialize(data);
            byte[] array = new byte[16];
            byte[] secretKey = new byte[16];
            var r = new Random();
            r.NextBytes(array);
            for (var i = 0; i < array.Length; i++)
            {
                var n = array[i];
                secretKey[i] = Convert.ToByte(Netease_base62[n % 62]);
            }
            byte[] encryptedData = AesEncrypt.CreateEncrypt().Encrypt(text, Encoding.UTF8.GetBytes(Netease_presetKey), Encoding.UTF8.GetBytes(Netease_iv), CipherMode.CBC);
            var encryptedString = Convert.ToBase64String(encryptedData);
            var @params = Convert.ToBase64String(AesEncrypt.CreateEncrypt().Encrypt(encryptedString, secretKey, Encoding.UTF8.GetBytes(Netease_iv), CipherMode.CBC));
            Array.Reverse(secretKey);
            byte[] rsaEncrypted = RsaEncrypt.CreateEncrypt().Encrypt_NoPadding(secretKey, Netease_publicKey);
            var encSecKey = BitConverter.ToString(rsaEncrypted, 0).Replace("-", string.Empty).ToLower();
            string s = "params=" + Uri.EscapeDataString(@params) + "&encSecKey=" + Uri.EscapeDataString(encSecKey);
            return new StringContent(s);
        }

        private static StringContent GetLinuxParams(object data)
        {

            var text = JsonSerializer.Serialize(data);
            var eparams = BitConverter.ToString(AesEncrypt.CreateEncrypt().Encrypt(text, Encoding.UTF8.GetBytes(Netease_linuxapiKey), Encoding.UTF8.GetBytes(""), CipherMode.ECB), 0).Replace("-", string.Empty).ToUpper();
            string s = "eparams=" + Uri.EscapeDataString(eparams);
            return new StringContent(s);
        }

        private static StringContent GetEapiParams(string url, object Data)
        {
            var text = JsonSerializer.Serialize(Data, new JsonSerializerOptions { IgnoreNullValues = true });
            var message = $"nobody{url}use{text}md5forencrypt";
            string digest;
            using (var md5 = MD5.Create())
            {
                digest = md5.ComputeHash(Encoding.UTF8.GetBytes(message))
                    .Aggregate("", (current, b) => current + b.ToString("X2"));
            }

            var data = $"{url}-36cd479b6b5-{text}-36cd479b6b5-{digest}";

            var @params = AesEncrypt.CreateEncrypt()
                 .Encrypt(data, Encoding.UTF8.GetBytes(Netease_eapiKey), Encoding.UTF8.GetBytes(""), CipherMode.ECB)
                 .Aggregate("", (current, b) => current + b.ToString("X2")).ToUpper();
            string s = "params=" + Uri.EscapeDataString(@params);
            return new StringContent(s);
        }

        private static string GetCookieString(Hashtable cookies)
        {
            string s = "";
            foreach (string key in cookies.Keys)
            {
                s += key + "=";
                s += cookies[key].ToString() + ";";
            }
            s = s.Substring(0, s.LastIndexOf(";"));
            return s;
        }
    }
}
