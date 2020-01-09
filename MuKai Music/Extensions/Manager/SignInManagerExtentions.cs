using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MuKai_Music.Model.DataEntity;
using MuKai_Music.Model.Manager;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MuKai_Music.Extensions.Manager
{
    public static class SignInManagerExtentions
    {
        public static async Task<SigninResult> JwtSignInAsync(
            this SignInManager<UserInfo> signInManager,
            UserInfo user,
            string password,
            bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return await signInManager.CheckPasswordSigninAsync(user, password, lockoutOnFailure);
        }

        public static async Task<SigninResult> CheckPasswordSigninAsync(
            this SignInManager<UserInfo> signInManager,
            UserInfo user, string password, bool lockoutOnFailure)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var error = await signInManager.PreSignInCheck(user);
            if (error != null) return error;

            if (await signInManager.UserManager.CheckPasswordAsync(user, password))
            {
                bool alwaysLockout = AppContext.TryGetSwitch("Microsoft.AspNetCore.Identity.CheckPasswordSignInAlwaysResetLockoutOnSuccess", out bool enabled) && enabled;
                // Only reset the lockout when TFA is not enabled when not in quirks mode
                if (alwaysLockout || !await signInManager.IsTfaEnabled(user))
                {
                    await signInManager.ResetLockout(user);
                }

                //token
                return SigninResult.Success(TokenManager.GetAccessToken(user));
            }
            signInManager.Logger.LogWarning(2, "User {userId} failed to provide the correct password.", await signInManager.UserManager.GetUserIdAsync(user));

            if (signInManager.UserManager.SupportsUserLockout && lockoutOnFailure)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await signInManager.UserManager.AccessFailedAsync(user);
                if (await signInManager.UserManager.IsLockedOutAsync(user))
                {
                    return await signInManager.LockedOut(user);
                }
            }
            return SigninResult.Failed();
        }



        private static async Task<SigninResult> PreSignInCheck(this SignInManager<UserInfo> signInManager,
            UserInfo user)
        {
            if (!await signInManager.CanSigninAsync(user))
            {
                return SigninResult.NotAllowd();
            }
            if (await signInManager.IsLockedOut(user))
            {
                return await signInManager.LockedOut(user);
            }
            return null;
        }

        private static async Task<bool> CanSigninAsync(this SignInManager<UserInfo> signInManager,
            UserInfo user)
        {
            if (signInManager.Options.SignIn.RequireConfirmedEmail && !(await signInManager.UserManager.IsEmailConfirmedAsync(user)))
            {
                signInManager.Logger.LogWarning(0, "User {userId} cannot sign in without a confirmed email.", await signInManager.UserManager.GetUserIdAsync(user));
                return false;
            }
            if (signInManager.Options.SignIn.RequireConfirmedPhoneNumber && !(await signInManager.UserManager.IsPhoneNumberConfirmedAsync(user)))
            {
                signInManager.Logger.LogWarning(1, "User {userId} cannot sign in without a confirmed phone number.", await signInManager.UserManager.GetUserIdAsync(user));
                return false;
            }
            if (signInManager.Options.SignIn.RequireConfirmedAccount && !(await IsConfirmedAsync(signInManager.UserManager, user)))
            {
                signInManager.Logger.LogWarning(4, "User {userId} cannot sign in without a confirmed account.", await signInManager.UserManager.GetUserIdAsync(user));
                return false;
            }
            return true;
        }

        private static async Task<bool> IsConfirmedAsync(UserManager<UserInfo> manager, UserInfo user)
        {
            if (!await manager.IsEmailConfirmedAsync(user))
            {
                return false;
            }
            return true;
        }

        private static async Task<bool> IsLockedOut(this SignInManager<UserInfo> signInManager, UserInfo user)
        {
            return signInManager.UserManager.SupportsUserLockout && await signInManager.UserManager.IsLockedOutAsync(user);
        }

        private static async Task<bool> IsTfaEnabled(this SignInManager<UserInfo> signInManager, UserInfo user)
            => signInManager.UserManager.SupportsUserTwoFactor &&
            await signInManager.UserManager.GetTwoFactorEnabledAsync(user) &&
            (await signInManager.UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

        private static Task ResetLockout(this SignInManager<UserInfo> signInManager, UserInfo user)
        {
            if (signInManager.UserManager.SupportsUserLockout)
            {
                return signInManager.UserManager.ResetAccessFailedCountAsync(user);
            }
            return Task.CompletedTask;
        }

        private static async Task<SigninResult> LockedOut(this SignInManager<UserInfo> signInManager, UserInfo user)
        {
            signInManager.Logger.LogWarning(3, "User {userId} is currently locked out.", await signInManager.UserManager.GetUserIdAsync(user));
            return SigninResult.LockedOut();
        }

        public static async Task<bool> IsTwoFactorClientRememberedAsync(this SignInManager<UserInfo> signInManager, UserInfo user)
        {
            var userId = await signInManager.UserManager.GetUserIdAsync(user);
            var result = await signInManager.Context.AuthenticateAsync(IdentityConstants.TwoFactorRememberMeScheme);
            return (result?.Principal != null && result.Principal.FindFirstValue(ClaimTypes.Name) == userId);
        }
    }
}
