using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MuKai_Music.Model.Handler
{
    public class DefaultAuthenticationHandler : IAuthenticationHandler, IAuthenticationSignInHandler, IAuthenticationSignOutHandler
    {


        public AuthenticationScheme Scheme { get; private set; }
        public HttpContext Context { get; private set; }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            var cookie = Context.Request.Cookies[""];
            return null;
        }
        public Task ChallengeAsync(AuthenticationProperties properties) => throw new NotImplementedException();
        public Task ForbidAsync(AuthenticationProperties properties) => throw new NotImplementedException();
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => throw new NotImplementedException();
        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties) => throw new NotImplementedException();
        public Task SignOutAsync(AuthenticationProperties properties) => throw new NotImplementedException();
    }
}
