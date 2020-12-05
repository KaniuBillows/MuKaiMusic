using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAbstract.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;

namespace DataAbstract.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class DecryptMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Pkcs1Encoding pkcs1Encoding;
        public DecryptMiddleware(IConfiguration configuration, RequestDelegate next)
        {

            using TextReader reader = new StringReader(configuration["PrivateKey"]);
            PemObject pemObject = new PemReader(reader).ReadPemObject();
            AsymmetricKeyParameter pkey = PrivateKeyFactory.CreateKey(pemObject.Content);
            this.pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
            pkcs1Encoding.Init(false, pkey);
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint == null)
            {
                await _next(httpContext);
                return;
            }
            var metaDatas = endpoint.Metadata;
            var crypted = metaDatas.GetMetadata<EncryptAttribute>();
            if (crypted == null)
            {
                await _next(httpContext);
                return;
            }
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
                string result = Encoding.UTF8.GetString(decrypted);
                httpContext.Request.Body = new MemoryStream(decrypted);
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 400;
                return;
            }
            await _next(httpContext);
            return;
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

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DecryptMiddlewareExtensions
    {
        public static IApplicationBuilder UseDecryptMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DecryptMiddleware>();
        }
    }
}
