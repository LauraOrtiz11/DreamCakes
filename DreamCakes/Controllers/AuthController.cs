using DreamCakes.Dtos;
using DreamCakes.Dtos.Admin;
using DreamCakes.Services;
using DreamCakes.Utilities;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;


namespace DreamCakes.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService = new AuthService();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = AuthErrorsUtility.INVALID_DATA;
                    TempData["ShowModal"] = "login";
                    return RedirectToAction("Index", "Home");
                }

                var result = _authService.Authenticate(loginDto);

                if (result.Response != 1)
                {
                    TempData["ErrorMessage"] = result.Message;
                    TempData["ShowModal"] = "login";
                    return RedirectToAction("Index", "Home");
                }

                // Configurar autenticación con Cookies
                CookieUtility.CreateAuthCookie(Response, result.Email, result.ID_Role);
                SessionManagerUtility.SetUserSession(Session, result);

                // Redirección según el rol a la vista principal de cada uno
                return RedirectToRole(result.ID_Role);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en Login: {ex}");
                TempData["ErrorMessage"] = AuthErrorsUtility.GENERAL_SERVER_ERROR;
                TempData["ShowModal"] = "login";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = AuthErrorsUtility.INVALID_DATA;
                    TempData["ShowModal"] = "register";
                    return RedirectToAction("Index", "Home");
                }

                var result = _authService.Register(registerDto);

                if (result.Response != 1)
                {
                    TempData["ErrorMessage"] = result.Message ?? AuthErrorsUtility.GENERAL_REGISTER_ERROR;
                    TempData["ShowModal"] = "register";
                    return RedirectToAction("Index", "Home");
                }

                TempData["SuccessMessage"] = AuthErrorsUtility.REGISTER_SUCCESS;
                TempData["ShowModal"] = "login";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en Register: {ex}");
                TempData["ErrorMessage"] = AuthErrorsUtility.GENERAL_SERVER_ERROR;
                TempData["ShowModal"] = "register";
                return RedirectToAction("General", "Error");
            }
        }

        public ActionResult Logout()
        {
            try
            {
                // Eliminar cookies de autenticación y datos de sesión
                CookieUtility.RemoveAuthCookie(Response);
                SessionManagerUtility.ClearUserSession(Session);

                // Prevenir el almacenamiento en caché y la navegación hacia atrás
                Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                Response.AddHeader("Pragma", "no-cache");
                Response.AddHeader("Cache-Control", "no-store");

                // Redirigir al inicio
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en Logout: {ex}");
                return RedirectToAction("General", "Error");
            }
        }


        private ActionResult RedirectToRole(int roleId)
        {
            try
            {
                switch (roleId)
                {
                    case 1:
                        return RedirectToAction("Index", "Home");
                    case 2: 
                        return RedirectToAction("Index", "Home");
                    case 3:
                        return RedirectToAction("Index", "Home");
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en RedirectToRole: {ex}");
                return RedirectToAction("General", "Error");
            }
        }
    }
}