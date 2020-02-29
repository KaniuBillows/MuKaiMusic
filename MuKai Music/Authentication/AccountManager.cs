using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuKai_Music.Model.DataEntity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace MuKai_Music.Model.Authentication
{
    /// <summary>
    /// 针对业务需求，扩展账户管理
    /// </summary>
    public class AccountManager : UserManager<UserInfo>
    {

        public AccountManager(IUserStore<UserInfo> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<UserInfo> passwordHasher,
            IEnumerable<IUserValidator<UserInfo>> userValidators,
            IEnumerable<IPasswordValidator<UserInfo>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services,
            ILogger<UserManager<UserInfo>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        /// <summary>
        /// 根据手机号码查找用户
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public virtual async Task<UserInfo> FindByPhoneNumberAsync(string phoneNumber)
        {
            ThrowIfDisposed();
            CancellationToken cancellationToken = default;
            cancellationToken.ThrowIfCancellationRequested();
            if (phoneNumber == null)
                throw new ArgumentNullException(nameof(phoneNumber));
            return await Task.FromResult(Users.Where(u => u.PhoneNumber == phoneNumber).SingleOrDefault());
        }
    }
}
