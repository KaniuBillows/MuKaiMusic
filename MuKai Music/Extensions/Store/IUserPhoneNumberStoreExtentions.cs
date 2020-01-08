using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MuKai_Music.Model.DataEntity;
using MuKai_Music.DataContext;

namespace MuKai_Music.Extensions.Store
{
    public static class IUserPhoneNumberStoreExtentions
    {
        public static Task<UserInfo> FindByPhoneNumberAsync(
            this PhoneNumberStore userPhoneNumberStore,
            string phoneNumber,
            CancellationToken cancellationToken = default
            )
        {
            return userPhoneNumberStore.FindByPhoneNumberAsync(phoneNumber, cancellationToken);
        }
    }

    public class PhoneNumberStore : UserStore<UserInfo, UserRole, AccountContext, int>,
        IUserPhoneNumberStore<UserInfo>, IUserStore<UserInfo>
    {
        public PhoneNumberStore(AccountContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        /// <summary>
        /// 扩展实现根据手机号查找用户
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<UserInfo> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(Users.Where(u => u.PhoneNumber == phoneNumber).SingleOrDefault());
        }

    }
}
