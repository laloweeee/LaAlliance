using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Authentication
{
    public class AuthenticationUserFilters : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if user is authenticated
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                // Redirect based on role
                var redirectResult = userRole switch
                {
                    "Customer" => new RedirectToActionResult("Index", "Customer", new { area = "Customer" }),
                    "Restaurant" => new RedirectToActionResult("Index", "Restaurant", new { area = "Restaurant" }),
                    _ => null
                };

                if (redirectResult != null)
                {
                    context.Result = redirectResult;
                }
            }
        }
    }
}
