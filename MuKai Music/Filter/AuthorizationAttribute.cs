using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace MuKai_Music.Filter
{
    /// <summary>
    /// 授权过滤器，尝试读取请求头部中的token，并附加到Action的参数列表
    /// </summary>
    public class AuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authentication))
            {
                string token = authentication.ToString().Substring("Bearer ".Length).Trim();
                if (context.ActionDescriptor.Parameters.Any(item => item.Name == "token" && item.ParameterType == typeof(string)))
                {
                    context.ActionArguments.Add("token", authentication.ToString());
                }
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                    string userId = jwtToken.Payload["id"] as string;
                    if (context.ActionArguments.TryGetValue("loginUserId", out object val))
                    {
                        context.ActionArguments["loginUserId"] = long.Parse(userId);
                    }
                    else
                    {
                        context.ActionArguments.Add("loginUserId", long.Parse(userId));
                    }

                }
                catch (Exception)
                {
                    context.Result = new ForbidResult("无效的Token");
                    return;
                }
            }
        }
    }
}
