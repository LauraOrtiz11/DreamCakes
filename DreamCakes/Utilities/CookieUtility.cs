using System;
using System.Web;
using System.Web.Security;

namespace DreamCakes.Utilities
{
    public static class CookieUtility
    {

        // Crea una cookie de autenticación con el email y rol del usuario
        public static void CreateAuthCookie(HttpResponseBase response, string email, int ID_Rol)
        {
            // Crear ticket de autenticación con los datos del usuario
            var authTicket = new FormsAuthenticationTicket(
                version: 1,
                name: email,
                issueDate: DateTime.Now,
                expiration: DateTime.Now.AddMinutes(30),
                isPersistent: false,
                userData: ID_Rol.ToString(),
                cookiePath: FormsAuthentication.FormsCookiePath);

            // Encriptar el ticket 
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            // Crear la cookie HTTP con el ticket encriptado
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                Domain = FormsAuthentication.CookieDomain
            };

            response.Cookies.Add(authCookie);
        }


        // Elimina la cookie de autenticación del navegador del usuario
        public static void RemoveAuthCookie(HttpResponseBase response)
        {
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1),
                Domain = FormsAuthentication.CookieDomain
            };

            // Agregar la cookie a la respuesta para borrarla del cliente
            response.Cookies.Add(cookie);
        }


        // Obtiene el ID de rol del usuario desde la cookie de autenticación
        public static int? GetUserRoleFromCookie(HttpRequestBase request)
        {
            var authCookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return null;

            try
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (int.TryParse(authTicket.UserData, out int ID_Rol))
                {
                    return ID_Rol;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}