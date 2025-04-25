using DreamCakes.Dtos;
using DreamCakes.Services;
using DreamCakes.Utilities;
using System;
using System.Web.Mvc;
using System.Web.Security;  

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
                    TempData["ErrorMessage"] = result.Message ?? AuthErrorsUtility.GENERAL_AUTH_ERROR;
                    TempData["ShowModal"] = "login";
                    return RedirectToAction("Index", "Home");
                }

                // Manejo de sesión y cookies
                CookieUtility.CreateAuthCookie(Response, result.Email, result.RoleId.ToString());
                SessionManagerUtility.SetUserSession(Session, result);

                return RedirectToRole(result.RoleId.Value);
            }
            catch (Exception ex)
            {
                // Log del error (implementar según tu sistema de logging)
                System.Diagnostics.Trace.TraceError($"Error en Login: {ex.ToString()}");

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
                // Log del error
                System.Diagnostics.Trace.TraceError($"Error en Register: {ex.ToString()}");

                TempData["ErrorMessage"] = AuthErrorsUtility.GENERAL_SERVER_ERROR;
                TempData["ShowModal"] = "register";
                return RedirectToAction("General", "Error");
            }
        }

        public ActionResult Logout()
        {
            try
            {
                CookieUtility.RemoveAuthCookie(Response);
                SessionManagerUtility.ClearUserSession(Session);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Trace.TraceError($"Error en Logout: {ex.ToString()}");

                // Aunque falle el logout, redirigimos al home
                return RedirectToAction("General", "Error");
            }
        }

        private ActionResult RedirectToRole(int roleId)
        {
            try
            {
                switch (roleId)
                {
                    case 1: return RedirectToAction("Promotion", "Admin");
                    case 3: return RedirectToAction("Dashboard", "Delivery");
                    default: return RedirectToAction("Dashboard", "Client");
                }
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Trace.TraceError($"Error en RedirectToRole: {ex.ToString()}");

                // Redirigir a página genérica si falla la redirección específica
                return RedirectToAction("General", "Error");
            }
        }
    }
}