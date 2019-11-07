using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MuKai_Music.Controllers;
using MuKai_Music.Model.Service;
using MuKai_Music.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuKai_Music.Filter
{
    public class MyActionFilter : IActionFilter
    {
        private readonly UserService userService;

        public MyActionFilter(UserService userService)
        {
            this.userService = userService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var res= context.Result as ActionResult;
            
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
