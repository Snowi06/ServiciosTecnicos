using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServiciosTecnicos.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeSessionAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(role))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Login", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

