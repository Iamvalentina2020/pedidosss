using GestionPedidos.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace GestionPedidos.Filters
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        private readonly UserRole[]? _allowedRoles;

        public AuthorizeAttribute(params UserRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userJson = context.HttpContext.Session.GetString("CurrentUser");
            
            if (string.IsNullOrEmpty(userJson))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            if (_allowedRoles != null && _allowedRoles.Length > 0)
            {
                try
                {
                    var userData = JsonSerializer.Deserialize<dynamic>(userJson);
                    var userRole = (UserRole)((JsonElement)userData.GetProperty("Role")).GetInt32();
                    
                    if (!_allowedRoles.Contains(userRole))
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
                catch
                {
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}