using DreamCakes.Dtos;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DreamCakes.Utilities;
using System;
using System.Linq;

namespace DreamCakes.Utilities
{
    public class SessionAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session["ID_Usuario"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
        }
    }

    public static class SessionManagerUtility
    {
        // Establece los valores de la sesión del usuario a partir de un objeto LoginDto.
        public static void SetUserSession(HttpSessionStateBase session, LoginDto loginDto)
        {
            session["ID_Usuario"] = loginDto.ID_User;
            session["UserEmail"] = loginDto.Email;
            session["ID_Rol"] = loginDto.ID_Role;
            session["ID_Estado"] = loginDto.ID_State;
        }

        // Limpia todos los datos relacionados con el usuario de la sesión.
        public static void ClearUserSession(HttpSessionStateBase session)
        {
            session.Remove("ID_Usuario");
            session.Remove("UserEmail");
            session.Remove("ID_Rol");
            session.Remove("ID_Estado");
            session.Clear();
            session.Abandon();
        }
        public class SessionAuthorize : AuthorizeAttribute
        {
            public override void OnAuthorization(AuthorizationContext filterContext)
            {
                if (HttpContext.Current.Session["ID_Usuario"] == null)
                {
                    filterContext.Result = new RedirectResult("~/Auth/Login");
                }
            }
        }

        // Obtiene el ID del usuario actual desde la sesión.
        public static int? GetCurrentUserId(HttpSessionStateBase session)
        {
            return session["ID_Usuario"] as int?;
        }

        // Obtiene el rol del usuario actual desde la sesión.
        public static int? GetCurrentUserRole(HttpSessionStateBase session)
        {
            return session["ID_Rol"] as int?;
        }

        // Obtiene el estado del usuario actual desde la sesión.
        public static int? GetCurrentUserStatus(HttpSessionStateBase session)
        {
            return session["ID_Estado"] as int?;
        }
    }
}