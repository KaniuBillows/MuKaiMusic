using Encrypt;
using RequestHandler;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MiGu_Music_API
{
    public class Web_PlayInfo : BaseRequestOption
    {
        public Web_PlayInfo(Hashtable cookies) : base(cookies)
        {

        }

        public Web_PlayInfo(string copyRightId) : this(new Hashtable())
        {
            this.Params.Add("copyrightId", copyRightId);
        }

        public override string Url { get; } = "http://music.migu.cn/v3/api/music/audioPlayer/getPlayInfo?";

        public override CryptoType Crypto { get; } = CryptoType.MiGU_Web;

        public override string Ua { get; } = "pc";

        public override HttpMethod HttpMethod { get; } = HttpMethod.GET;

        public override string OptionUrl { get; }

        public override string GetQueryString()
        {
            var text = JsonSerializer.Serialize(Params);
            byte[] password = new byte[32], salt = new byte[8];
            var r = new Random();
            r.NextBytes(password);
            r.NextBytes(salt);
            string passwordHex = string.Join("", password.Select(p => p.ToString("x2")));
            password = Encoding.UTF8.GetBytes(passwordHex);
            byte[] result = new byte[0];
            byte[] previousResult = new byte[0];
            for (int i = 0; i < 5; i++)
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    previousResult = md5.ComputeHash(previousResult.Concat(password).Concat(salt).ToArray());
                    result = result.Concat(previousResult).ToArray();
                }
            }
            byte[] aesKey = result.Take(32).ToArray();
            byte[] aesIV = result.Skip(32).Take(16).ToArray();
            byte[] encryptedData = AesEncrypt.CreateEncrypt().Encrypt(text, aesKey, aesIV, CipherMode.CBC);
            byte[] secKey = RsaEncrypt.CreateEncrypt().Encrypt(password, "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC8asrfSaoOb4je+DSmKdriQJKW\nVJ2oDZrs3wi5W67m3LwTB9QVR+cE3XWU21Nx+YBxS0yun8wDcjgQvYt625ZCcgin\n2ro/eOkNyUOTBIbuj9CvMnhUYiR61lC1f1IGbrSYYimqBVSjpifVufxtx/I3exRe\nZosTByYp4Xwpb1+WAQIDAQAB");
            return "dataType=2&data="
                + Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes("Salted__").Concat(salt).Concat(encryptedData).ToArray()))
                + "&secKey=" + Uri.EscapeDataString(Convert.ToBase64String(secKey));
        }
    }
}
