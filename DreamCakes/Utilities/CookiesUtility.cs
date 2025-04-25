using System;
using System.Web;
using System.Web.Security;

namespace DreamCakes.Utilities
{
    public static class CookieUtility
    {
        public static void CreateAuthCookie(HttpResponseBase response, string email, string roleId)
        {
            var authTicket = new FormsAuthenticationTicket(
                version: 1,
                name: email,
                issueDate: DateTime.Now,
                expiration: DateTime.Now.AddMinutes(30),
                isPersistent: false,
                userData: roleId,
                cookiePath: FormsAuthentication.FormsCookiePath);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            response.Cookies.Add(authCookie);
        }

        public static void RemoveAuthCookie(HttpResponseBase response)
        {
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            response.Cookies.Add(cookie);
        }
    }
}