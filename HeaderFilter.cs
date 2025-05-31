using Microsoft.AspNetCore.Mvc.Filters;

namespace CinemaTicketServerREST
{
    public class HeaderFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers.Add("Header", "Test");
        }
    }
}
