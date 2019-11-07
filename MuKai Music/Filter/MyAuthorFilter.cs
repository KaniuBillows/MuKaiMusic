using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

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
            this.httpContext = context.HttpContext;
        }
    }

}
