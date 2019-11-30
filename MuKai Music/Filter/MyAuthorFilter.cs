using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MuKai_Music.Filter
{
    public class MyAuthorFilter : IAuthorizationFilter
    {
        private HttpContext httpContext;
        public HttpContext HttpContext => httpContext;

        public MyAuthorFilter()
        {

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.httpContext = context.HttpContext;
        }
    }

}
