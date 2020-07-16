using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;

namespace Mukai_Account.Filters
{
    /// <summary>
    /// Body加密，被此特性标注的Action的Body会进行尝试解密
    /// </summary>
    public class EncryptAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration configuration;
        private readonly Pkcs1Encoding pkcs1Encoding;
        public EncryptAttribute(IConfiguration configuration)
        {
            this.configuration = configuration;
            using TextReader reader = new StringReader(configuration["PrivateKey"]);
            PemObject pemObject = new PemReader(reader).ReadPemObject();
            AsymmetricKeyParameter pkey = PrivateKeyFactory.CreateKey(pemObject.Content);
            this.pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
            pkcs1Encoding.Init(false, pkey);
        }
        /// <summary>
        /// 进行Http Body解密
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (!httpContext.Request.Query.TryGetValue("key", out StringValues cryptedAes))
            {
                httpContext.Response.StatusCode = 400;
                return;
            }
            httpContext.Request.EnableBuffering();
            using var reader = new StreamReader(httpContext.Request.Body);
            byte[] encrypted = Convert.FromBase64String(await reader.ReadToEndAsync());
            try
            {
                byte[] encryptedKey = Convert.FromBase64String(cryptedAes.ToString().Replace(' ', '+'));
                byte[] aesKey = this.RsaDecrypt(encryptedKey);
                byte[] decrypted = this.AesDecrypt(encrypted, Encoding.UTF8.GetString(aesKey));//24
                string re = Encoding.UTF8.GetString(decrypted);
                httpContext.Request.Body = new MemoryStream(decrypted);
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                await next();
                return;
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 400;
                return;
            }
        }

        private byte[] RsaDecrypt(byte[] encrypted) => this.pkcs1Encoding.ProcessBlock(encrypted, 0, encrypted.Length);

        private byte[] AesDecrypt(byte[] encrypted, string key)
        {
            using SymmetricAlgorithm des = Aes.Create();
            des.Key = Convert.FromBase64String(key);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            using ICryptoTransform cTransform = des.CreateDecryptor();
            return cTransform.TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

    }
}