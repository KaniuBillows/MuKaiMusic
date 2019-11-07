using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Filter
{
    public sealed class CookieResultFilter : ResultFilterAttribute
    {

        public override void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
        }
    }
}
