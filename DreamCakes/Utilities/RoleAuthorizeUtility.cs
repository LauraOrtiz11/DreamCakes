using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace DreamCakes.Utilities
{
    public class RoleAuthorizeUtility : AuthorizeAttribute
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorizeUtility(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Primero verifica autenticación básica
            if (!base.AuthorizeCore(httpContext))
                return false;

            try
            {
                var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null) return false;

                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket == null || string.IsNullOrEmpty(authTicket.UserData))
                    return false;

                return int.TryParse(authTicket.UserData, out int userRole) &&
                       _allowedRoles.Contains(userRole);
            }
            catch
            {
                // En caso de error con la cookie, considerar no autorizado
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext?.HttpContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Usuario autenticado pero sin permisos (403)
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "Unauthorized" }
                    });
            }
            else
            {
                // Usuario no autenticado (redirigir a login)
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}