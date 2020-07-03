

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAbstract;
using DataAbstract.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Mukai_Account.Service;
using Mukai_Account.Filters;
using Mukai_Account.Model;
using Mukai_Account.Service.Interface;

namespace Mukai_Account.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly AccountService accountService;
        private readonly IFileService fileService;

        public AccountController(AccountService account, IFileService fileService)
        {
            this.accountService = account;
            this.fileService = fileService;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ServiceFilter(typeof(EncryptAttribute))]
        public Task<Result> Register([FromBody] UserRegisterModel user)
        {
            return this.accountService.Register(user);
        }

        /// <summary>
        /// 验证手机号码或者邮箱是否存在
        /// </summary>
        /// <param name="validParam"></param>
        /// <returns>Result code为200即不存在，可以使用</returns>
        [HttpGet("isExists")]
        [AllowAnonymous]
        public Task<Result<object>> ValidExists([Required] string validParam)
        {
            return accountService.ValidExists(validParam);
        }

        /// <summary>
        /// 通过旧密码修改新密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("updatePassword")]
        [Authorization]
        [ServiceFilter(typeof(EncryptAttribute))]
        public Task<Result> ChangePasswordByOld([Required][FromBody] ChangePasswordModel model, long loginUserId)
        {
            return this.accountService.ChangePasswordByOld(loginUserId, model);
        }

        /// <summary>
        /// 根据Id获取用户资料信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("userInfo")]
        public Task<Result<User>> GetUserInfo(long userId)
        {
            return this.accountService.GetUserInfo(userId);
        }

        /// <summary>
        /// 上传并更新用户头像
        /// </summary>
        /// <returns></returns>
        [HttpPost("uploadAvatar")]
        [Authorization]
        public async Task<Result<string>> UpdateAvatar(IFormFile file, long loginUserId)
        {
            Task<Result<User>> userQuery = this.accountService.GetUserInfo(loginUserId);
            if (!file.ContentType.ToLower().Contains("image"))
            {
                return Result<string>.FailResult("仅限图片格式");
            }
            //同一格式的文件名
            string fileName = new StringBuilder("avatar_").Append(loginUserId).ToString();
            User user = (await userQuery).Content;
            if (user == null) return Result<string>.FailResult("用户不存在", 404);
            //将头像文件保存
            var uploadRes = await this.fileService.UploadFileAsync(file, fileName);
            if (uploadRes.Content != null)
            {
                //上传成功，更新用户资料
                Result dbUpdateRes = await this.accountService.UpdateAvator(uploadRes.Content, loginUserId);
                if (200.Equals(dbUpdateRes.Code))
                {
                    //删除原本
                    await this.fileService.DeleteFileAsync(user.AvatarUrl);
                    return Result<string>.SuccessReuslt(uploadRes.Content);
                }
                //更新用户资料失败
                else
                {
                    //删除已经上传的文件
                    await this.fileService.DeleteFileAsync(uploadRes.Content);
                    return Result<string>.FailResult("更新失败!");
                }
            }
            else
            {
                //上传失败
                return Result<string>.FailResult("服务器异常，上传失败");
            }
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateUserInfo")]
        [Authorization]
        public Task<Result> UpdateUserInfo([FromBody] User user, long loginUserId)
        {
            return this.accountService.UpdateUserInfo(user, loginUserId);
        }
    }
}