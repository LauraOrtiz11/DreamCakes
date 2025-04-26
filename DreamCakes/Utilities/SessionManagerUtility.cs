using DreamCakes.Dtos;
using System.Web;

namespace DreamCakes.Utilities
{
    public static class SessionManagerUtility
    {
        public static void SetUserSession(HttpSessionStateBase session, LoginDto loginDto)
        {
            session["ID_Usuario"] = loginDto.ID_Usuario;
            session["UserEmail"] = loginDto.Email;
            session["ID_Rol"] = loginDto.ID_Rol;
            session["ID_Estado"] = loginDto.ID_Estado;
        }

        public static void ClearUserSession(HttpSessionStateBase session)
        {
            session.Remove("ID_Usuario");
            session.Remove("UserEmail");
            session.Remove("ID_Rol");
            session.Remove("ID_Estado");
            session.Clear();
            session.Abandon();
        }

        public static int? GetCurrentUserId(HttpSessionStateBase session)
        {
            return session["ID_Usuario"] as int?;
        }

        public static int? GetCurrentUserRole(HttpSessionStateBase session)
        {
            return session["ID_Rol"] as int?;
        }

        public static int? GetCurrentUserStatus(HttpSessionStateBase session)
        {
            return session["ID_Estado"] as int?;
        }
    }
}