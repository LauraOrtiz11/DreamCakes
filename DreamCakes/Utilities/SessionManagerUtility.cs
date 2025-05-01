using DreamCakes.Dtos;
using System.Web;

namespace DreamCakes.Utilities
{
    public static class SessionManagerUtility
    {
        // Establece los valores de la sesión del usuario a partir de un objeto LoginDto.
        public static void SetUserSession(HttpSessionStateBase session, LoginDto loginDto)
        {
            session["ID_User"] = loginDto.ID_User;
            session["UserEmail"] = loginDto.Email;
            session["ID_Role"] = loginDto.ID_Role;
            session["ID_State"] = loginDto.ID_State;
        }

        // Limpia todos los datos relacionados con el usuario de la sesión.
        public static void ClearUserSession(HttpSessionStateBase session)
        {
            session.Remove("ID_User");
            session.Remove("UserEmail");
            session.Remove("ID_Role");
            session.Remove("ID_State");
            session.Clear();
            session.Abandon();
        }

        // Obtiene el ID del usuario actual desde la sesión.
        public static int? GetCurrentUserId(HttpSessionStateBase session)
        {
            return session["ID_User"] as int?;
        }

        // Obtiene el rol del usuario actual desde la sesión.
        public static int? GetCurrentUserRole(HttpSessionStateBase session)
        {
            return session["ID_Role"] as int?;
        }

        // Obtiene el estado del usuario actual desde la sesión.
        public static int? GetCurrentUserStatus(HttpSessionStateBase session)
        {
            return session["ID_State"] as int?;
        }
    }
}