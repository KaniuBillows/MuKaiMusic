using System.Threading.Tasks;
using DataAbstract;
using Microsoft.AspNetCore.Http;

namespace Mukai_Account.Service.Interface
{
    public interface IFileService
    {
        /// <summary>
        /// 上传文件，并返回文件URL
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Result<string> UploadFile(IFormFile file, string fileName);


        Task<Result<string>> UploadFileAsync(IFormFile file, string fileName);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task DeleteFileAsync(string fileName);
    }
}