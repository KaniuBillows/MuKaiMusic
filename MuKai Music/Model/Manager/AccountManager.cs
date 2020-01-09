using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuKai_Music.Model.DataEntity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MuKai_Music.Extensions.Store;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Linq;

namespace MuKai_Music.Model.Manager
{
    public class AccountManager : UserManager<UserInfo>
    {
        private readonly IServiceProvider services;

        public AccountManager(IUserStore<UserInfo> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<UserInfo> passwordHasher,
            IEnumerable<IUserValidator<UserInfo>> userValidators,
            IEnumerable<IPasswordValidator<UserInfo>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services,
            ILogger<UserManager<UserInfo>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this.services = services;
        }

        /// <summary>
        /// 根据手机号码查找用户
        /// </summary>
        /// <param name="phoneNunmber"></param>
        /// <returns></returns>
        public virtual async Task<UserInfo> FindByPhoneNumberAsync(string phoneNunmber)
        {
            ThrowIfDisposed();
            IUserPhoneNumberStore<UserInfo> store = GetPhoneNumberStore();
            if (phoneNunmber == null)
            {
                throw new ArgumentNullException(nameof(phoneNunmber));
            }

            var user = await store.FindByPhoneNumberAsync(phoneNunmber);

            if (user == null && Options.Stores.ProtectPersonalData)
            {
                ILookupProtectorKeyRing keyRing = services.GetService<ILookupProtectorKeyRing>();
                var protector = services.GetService<ILookupProtector>();
                if (keyRing != null && protector != null)
                {
                    foreach (string key in keyRing.GetAllKeyIds())
                    {
                        string oldKey = protector.Protect(key, phoneNunmber);
                        user = await store.FindByPhoneNumberAsync(oldKey, CancellationToken);
                        if (user != null)
                        {
                            return user;
                        }
                    }
                }
            }
            return user;
        }


        private IUserPhoneNumberStore<UserInfo> GetPhoneNumberStore()
        {
            if (!(this.Store is IUserPhoneNumberStore<UserInfo> cast))
            {
                throw new NotSupportedException(Resources.StoreNotIUserPhoneNumberStore);
            }
            return cast;
        }
    }


}
