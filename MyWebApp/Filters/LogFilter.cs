using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MyWebApp.Filters
{
    public class LogFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var req = context.HttpContext.Request;
            Trace.WriteLine($"Action Executing.|Method={req.Method}|Path={req.Path}");
        }
    }
}
