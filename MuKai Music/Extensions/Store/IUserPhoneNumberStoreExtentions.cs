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
            this IUserPhoneNumberStore<UserInfo> userPhoneNumberStore,
            string phoneNumber,
            CancellationToken cancellationToken = default
            )
        {
            if (userPhoneNumberStore is AccountStore store)
                return store.FindByPhoneNumberAsync(phoneNumber, cancellationToken);
            throw new NotSupportedException(Resources.StoreNotIUserPhoneNumberStore);
        }
    }

    public class AccountStore : UserStore<UserInfo, UserRole, AccountContext, int>,
        IUserPhoneNumberStore<UserInfo>, IUserStore<UserInfo>
    {
        public AccountStore(AccountContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public Task<UserInfo> FindByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(Users.Where(u => u.PhoneNumber == phoneNumber).SingleOrDefault());
        }
    }
}
