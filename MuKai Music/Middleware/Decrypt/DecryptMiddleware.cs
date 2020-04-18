using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MuKai_Music;
using MuKai_Music.Attributes;
using MuKai_Music.Service;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MuKai_Account.Middleware
{
    public class DecryptMiddleware
    {
        private readonly RequestDelegate next;
        private readonly Pkcs1Encoding pkcs1Encoding;
        private readonly Dictionary<string, EncryptAttribute> ApiMap = new Dictionary<string, EncryptAttribute>();
        public DecryptMiddleware(RequestDelegate next)
        {
            this.next = next;
            using TextReader reader = new StringReader(Startup.Configuration.GetPivateKey());
            PemObject pemObject = new PemReader(reader).ReadPemObject();
            AsymmetricKeyParameter pkey = PrivateKeyFactory.CreateKey(pemObject.Content);
            this.pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
            pkcs1Encoding.Init(false, pkey);
            var assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            Type[] apicontrollers = types.Where(type => type.GetCustomAttribute<ApiControllerAttribute>() != null).ToArray();
            foreach (Type ctrl in apicontrollers)
            {
                MethodInfo[] controllerMethods = ctrl.GetMethods();
                RouteAttribute apiRoute = ctrl.GetCustomAttribute<RouteAttribute>();
                if (apiRoute == null) throw new Exception($"{ctrl.Name} doesn't have a \"Route\" Attribute");
                string ctrRoute = apiRoute.Template;
                foreach (MethodInfo meth in controllerMethods)
                {
                    EncryptAttribute encryptAttribute = meth.GetCustomAttribute<EncryptAttribute>();
                    if (encryptAttribute != null)
                    {
                        string route;
                        HttpPostAttribute post = meth.GetCustomAttribute<HttpPostAttribute>();

                        if (post != null)
                        {
                            route = post.Template;
                        }
                        else
                        {
                            throw new Exception("Api Mehtod Be Encrypted Http Method Must Be POST ");
                        }
                        string key = "/" + ctrRoute + "/" + route;
                        if (this.ApiMap.ContainsKey(key))
                        {
                            throw new Exception("Api Method Route Repeat Exception！");
                        }

                        this.ApiMap.Add(key, encryptAttribute);
                    }
                }
            }
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string apikey = httpContext.Request.Path.Value;
            if (!this.ApiMap.ContainsKey(apikey))
            {
                await next(httpContext);
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
                byte[] aesKey = this.RsADecrypt(encryptedKey);
                byte[] decrypted = this.AesDecrypt(encrypted, Encoding.UTF8.GetString(aesKey));//24
                string re = Encoding.UTF8.GetString(decrypted);
                httpContext.Request.Body = new MemoryStream(decrypted);
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                await next(httpContext);
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 400;
                return;
            }
        }

        private byte[] RsADecrypt(byte[] encrypted) => this.pkcs1Encoding.ProcessBlock(encrypted, 0, encrypted.Length);

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
