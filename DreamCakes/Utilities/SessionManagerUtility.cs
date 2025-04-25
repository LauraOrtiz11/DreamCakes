using DreamCakes.Dtos;
using System.Web;

namespace DreamCakes.Utilities
{
    public static class SessionManagerUtility
    {
        public static void SetUserSession(HttpSessionStateBase session, LoginDto loginDto)
        {
            session["UserId"] = loginDto.UserId;
            session["UserEmail"] = loginDto.Email;
            session["UserRole"] = loginDto.RoleId;
        }

        public static void ClearUserSession(HttpSessionStateBase session)
        {
            session.Clear();
            session.Abandon();
        }

        public static int? GetCurrentUserId(HttpSessionStateBase session)
        {
            return session["UserId"] as int?;
        }

        public static int? GetCurrentUserRole(HttpSessionStateBase session)
        {
            return session["UserRole"] as int?;
        }
    }
}