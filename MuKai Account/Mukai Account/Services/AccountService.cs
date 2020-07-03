using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chloe;
using DataAbstract;
using DataAbstract.Account;
using Mukai_Account.Model;

namespace Mukai_Account.Service
{
    public class AccountService
    {
        private readonly IDbContext accountContext;

        public AccountService(IDbContext accountContext)
        {
            this.accountContext = accountContext;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Result> Register(UserRegisterModel user)
        {
            if (user == null) return Result.FailResult("参数错误", 400);
            //将用户保存数据库
            try
            {
                await this.accountContext.InsertAsync<User>(new User()
                {
                    NickName = user.NickName,
                    Email = user.Email,
                    PasswordHash = Sha1(user.Password),
                    PhoneNumber = user.PhoneNumber
                });
                return Result.SuccessReuslt("注册成功");
            }
            catch (Exception)
            {
                return Result.FailResult("注册失败！");
            }
        }

        /// <summary>
        /// 通过旧密码更新新密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Result> ChangePasswordByOld(long userId, ChangePasswordModel model)
        {
            if (model.NewPassword.Equals(model.OldPassword))
            {
                return Result.FailResult("两个密码咋会相同呢？", 400);

            }
            User user = await this.accountContext.QueryByKeyAsync<User>(userId);
            if (user == null)
            {
                return Result.FailResult("此用户不存在", 400);
            }
            if (this.ValidPassword(user, model.OldPassword))
            {
                //得到新的密码
                var newPwdHashed = Sha1(model.NewPassword);
                try
                {
                    int res = await this.accountContext.UpdateAsync<User>(usr => usr.Id == userId, usr => new User()
                    {
                        PasswordHash = newPwdHashed
                    });
                    return res > 0 ? Result.SuccessReuslt("更改成功") : Result.FailResult("更改失败");
                }
                catch (Exception)
                {
                    return Result.FailResult("更改失败，服务器开小差了");
                }
            }
            else
            {
                return Result.FailResult("原密码错误！", 400);
            }
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result<User>> GetUserInfo(long userId)
        {
            User user = await accountContext.QueryByKeyAsync<User>(userId);
            if (user == null)
                return Result<User>.FailResult("用户不存在！");
            else
            {
                user.PasswordHash = null;
                return Result<User>.SuccessReuslt(user);
            }

        }

        /// <summary>
        /// 更新用户头像图片链接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAvator(string url, long userId)
        {
            try
            {
                int res = await this.accountContext.UpdateAsync<User>(usr => usr.Id == userId,
                                                                      usr => new User() { AvatarUrl = url });
                return res > 0 ? Result.SuccessReuslt("更新成功") : Result.FailResult("更新失败");
            }
            catch (Exception)
            {
                return Result.FailResult("服务器异常");
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Result> UpdateUserInfo(User user, long userId)
        {
            if (user == null) return Result.FailResult();
            user.Id = null;
            user.PasswordHash = null;
            user.PhoneNumber = null;
            user.Email = null;
            try
            {
                int res = await this.accountContext.UpdateAsync<User>(usr => usr.Id == userId, usr => user);
                return Result.SuccessReuslt("更新成功");
            }
            catch (Exception)
            {
                return Result.FailResult();
            }
        }

        /// <summary>
        /// 验证邮箱或者手机号嘛是否已经注册
        /// </summary>
        /// <param name="validParam"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Result<object>> ValidExists(string validParam)
        {
            if (validParam == null) return Result<object>.FailResult("参数错误", 400);
            //判断是否为邮箱
            Regex email_regex = new Regex(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
            bool isExist = email_regex.IsMatch(validParam)
                ? await this.accountContext.Query<User>().AnyAsync(usr => validParam.ToUpper().Equals(usr.NormalizedEmail))
                : await this.accountContext.Query<User>().AnyAsync(usr => validParam.Equals(usr.PhoneNumber));
            return Result.SuccessReuslt(new { isExist });
        }


        private bool ValidPassword(User user, string password)
        {
            return user.PasswordHash != null && user.PasswordHash.Equals(Sha1(password));
        }

        private string Sha1(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            byte[] data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (byte t in data)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

    }
}
