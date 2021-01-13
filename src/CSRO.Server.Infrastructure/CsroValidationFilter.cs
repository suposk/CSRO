using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
{
    public class CsroValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {            
            if (!filterContext.ModelState.IsValid)
            {
                filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}
