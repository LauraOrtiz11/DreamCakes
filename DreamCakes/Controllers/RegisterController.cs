using System;
using System.Web.Mvc;
using DreamCakes.Dtos;
using DreamCakes.Services;

namespace DreamCakes.Controllers
{
    public class RegisterController : Controller
    {
        private readonly RegisterService registerService = new RegisterService();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterIndex(RegisterDto userDto)
        {
            try
            {
                
                var response = registerService.CreateUser(userDto);

                if (response.Response == 1)
                {
                    TempData["SuccessMessage"] = "¡Registro exitoso! Por favor inicie sesión.";
                    TempData["ShowModal"] = "login"; // Mostrar login después de registro exitoso
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = response.Message;
                TempData["ShowModal"] = "register";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error en registro: {ex.Message}");
                TempData["ErrorMessage"] = "Ocurrió un error inesperado. Por favor intente nuevamente.";
                TempData["ShowModal"] = "register";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CheckEmail(string email)
        {
            try
            {
                

                bool exists = registerService.EmailExists(email);
                return Json(new
                {
                    success = true,
                    exists = exists,
                    message = exists ? "El correo ya está registrado" : "Correo disponible"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error verificando email: {ex.Message}");
                return Json(new { success = false, message = "Error al verificar el email" });
            }
        }
    }
}