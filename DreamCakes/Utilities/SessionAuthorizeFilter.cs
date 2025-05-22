using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DreamCakes.Utilities;
using System;
using System.Linq;

namespace DreamCakes.Filters
{
    public class SessionAuthorizeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            var userId = SessionManagerUtility.GetCurrentUserId(session);

            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;

            // Acciones públicas permitidas sin sesión
            var publicRoutes = new[]
            {
        new { Controller = "Home", Action = "Index" },
        new { Controller = "Auth", Action = "Login" },
        new { Controller = "Auth", Action = "Register" },
        new { Controller = "Auth", Action = "Logout" },
        new { Controller = "Account", Action = "ForgotPassword" },
        new { Controller = "Account", Action = "ResetPassword" },
        new { Controller = "Cleanup", Action = "ClearTempMessages" }
    };

            bool isPublicRoute = publicRoutes.Any(r =>
                r.Controller.Equals(controller, StringComparison.OrdinalIgnoreCase) &&
                r.Action.Equals(action, StringComparison.OrdinalIgnoreCase));

            // Si no hay sesión y no es una ruta pública, redirige
            if (userId == null && !isPublicRoute)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
            { "controller", "Home" },
            { "action", "Index" }
        });
            }

            base.OnActionExecuting(filterContext);
        }

    }
}
