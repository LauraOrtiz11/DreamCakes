using System;
using System.Web.Mvc;
using DreamCakes.Dtos;
using DreamCakes.Services;

namespace DreamCakes.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginService loginService = new LoginService();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginDto loginDto)
        {
            try
            {
               
                var result = loginService.Login(loginDto);

                if (result.Response == 1)
                {
                    Session["UserEmail"] = loginDto.Email;
                    TempData["LoginMessage"] = result.Message;
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = result.Message;
                TempData["ShowModal"] = "login";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en login: {ex.Message}");
                TempData["ErrorMessage"] = "Error al iniciar sesión. Intente nuevamente.";
                TempData["ShowModal"] = "login";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}