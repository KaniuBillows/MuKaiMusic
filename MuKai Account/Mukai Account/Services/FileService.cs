
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using DataAbstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mukai_Account.Service.Interface;

namespace Mukai_Account.Service
{
    public class OssFileService : IFileService
    {
        private readonly IConfiguration configuration;
        private readonly IOss oss;
        private readonly ILogger<OssFileService> logger;

        public OssFileService(IConfiguration configuration, IOss oss, ILogger<OssFileService> logger)
        {
            this.configuration = configuration;
            this.oss = oss;
            this.logger = logger;
        }

        /// <summary>
        /// 异步上传文件到Oss
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Result<string>> UploadFileAsync(IFormFile file, string fileName)
        {
            string bucketName = configuration["BucketName"];
            string endpoint = configuration["OssEndPoint"];
            StringBuilder upFilePath = new StringBuilder(configuration["LocationName"]);
            upFilePath.Append("/");
            upFilePath.Append(fileName);
            string extName = Path.GetExtension(file.FileName);
            upFilePath.Append("_");
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            upFilePath.Append(ts.TotalMilliseconds.ToString());
            upFilePath.Append(extName);
            try
            {
                //进行上传
                var result = Task.Run(() => oss.PutObject(bucketName, upFilePath.ToString(), file.OpenReadStream()));
                StringBuilder url = new StringBuilder(bucketName);
                url.Append(".");
                url.Append(endpoint);
                url.Append("/");
                url.Append(upFilePath.ToString());
                if (await result == null)
                {
                    logger.LogError("upload file failed!");
                    return Result<string>.FailResult("服务器Oss异常！");
                }
                logger.LogInformation("upload file successful");
                return Result<string>.SuccessReuslt(url.ToString());
            }
            catch (OssException e)
            {
                logger.LogError(e.Message);
                return Result<string>.FailResult("服务器Oss异常！");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Result<string>.FailResult("服务器异常!");
            }
        }

        public Result<string> UploadFile(IFormFile file, string fileName)
        {
            string bucketName = configuration["BucketName"];
            string endpoint = configuration["OssEndPoint"];
            StringBuilder upFilePath = new StringBuilder(configuration["LocationName"]);
            upFilePath.Append("/");
            upFilePath.Append(fileName);
            string extName = Path.GetExtension(file.FileName);
            upFilePath.Append("_");
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            upFilePath.Append(ts.TotalMilliseconds.ToString());
            upFilePath.Append(extName);
            try
            {
                var result = oss.PutObject(bucketName, upFilePath.ToString(), file.OpenReadStream());
                if (result == null)
                {
                    logger.LogError("upload file failed!");
                    return Result<string>.FailResult("服务器Oss异常！");
                }
                logger.LogInformation("upload file successful");
                StringBuilder url = new StringBuilder(bucketName);
                url.Append(".");
                url.Append(endpoint);
                url.Append("/");
                url.Append(upFilePath.ToString());
                return Result<string>.SuccessReuslt(url.ToString());
            }
            catch (OssException e)
            {
                logger.LogError(e.Message);
                return Result<string>.FailResult("服务器Oss异常！");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Result<string>.FailResult("服务器异常!");
            }
        }

        /// <summary>
        /// 删除Oss文件
        /// </summary>
        /// <param name="fileUrl">Oss文件Url</param>
        /// <returns></returns>
        public Task DeleteFileAsync(string fileUrl)
        {
            int index = fileUrl.IndexOf("/");
            string objectName = fileUrl.Substring(index + 1);
            string bucketName = configuration["BucketName"];
            return Task.Run(() => oss.DeleteObject(bucketName, objectName));
        }
    }
}