using System;
using System.Linq;
using System.Web.Mvc;
using DreamCakes.Repositories.Models;
using DreamCakes.Utilities;


namespace DreamCakes.Controllers
{
    public class AccountController : Controller
    {
        private readonly DreamCakesEntities _context = new DreamCakesEntities();

        // GET: Mostrar formulario para solicitar recuperación de contraseña
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Recibir email para envío de link de recuperación
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    ViewBag.MensajeError = "Por favor ingresa un correo electrónico.";
                    return View();
                }

                using (var db = new DreamCakesEntities())
                {
                    var usuario = db.USUARIOs.FirstOrDefault(u => u.Email == email);

                    if (usuario == null)
                    {
                        ViewBag.MensajeError = "El correo no está registrado en DreamCakes.";
                        return View();
                    }

                    // Construir enlace de restablecimiento
                    string resetUrl = Url.Action("ResetPassword", "Account", new { email = email }, protocol: Request.Url.Scheme);

                    string asunto = "🔐 Restablecer contraseña - DreamCakes";

                    string mensaje = $@"
<div style='font-family: Arial, sans-serif; background-color: #f9f5ff; padding: 30px; border-radius: 12px; max-width: 600px; margin: auto; box-shadow: 0 4px 12px rgba(0,0,0,0.05);'>
    <div style='text-align: center; margin-bottom: 25px;'>
        <img src='cid:dreamcakeslogo' alt='DreamCakes Logo' style='width: 150px; height: auto;' />
    </div>

    <h2 style='color: #483470; text-align: center;'>Hola {usuario.Nombres},</h2>

    <p style='font-size: 16px; color: #333; text-align: center;'>
        Hemos recibido una solicitud para restablecer tu contraseña. Si no fuiste tú, puedes ignorar este mensaje.
    </p>

    <div style='text-align: center; margin: 35px 0;'>
        <a href='{resetUrl}' 
           style='display: inline-block; background-color: #9B2020; color: white; padding: 14px 28px; text-decoration: none; border-radius: 30px; font-size: 16px; font-weight: bold;'>
            Restablecer contraseña
        </a>
    </div>

    

    <p style='font-size: 14px; color: #555; text-align: center; margin-top: 25px;'>
        Gracias por confiar en <strong>DreamCakes</strong> 💜
    </p>
</div>";


                    // Enviar email
                    EmailManagerUtility emailManager = new EmailManagerUtility();
                    string errorMessage;

                    bool enviado = emailManager.SendEmail(email, asunto, mensaje, out errorMessage);

                    if (enviado)
                    {
                        ViewBag.MensajeExito = "Se ha enviado un enlace de restablecimiento a tu correo.";
                    }
                    else
                    {
                        ViewBag.MensajeError = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.MensajeError = $"Ocurrió un error inesperado: {ex.Message}";
            }

            return View();
        }


        // GET: Mostrar formulario para restablecer contraseña
        [HttpGet]
        public ActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return HttpNotFound();

            var user = _context.USUARIOs.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return HttpNotFound();

            ViewBag.Email = email;
            return View();
        }

        // POST: Recibir nueva contraseña y actualizarla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Message = "Completa todos los campos.";
                ViewBag.Email = email;
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "Las contraseñas no coinciden.";
                ViewBag.Email = email;
                return View();
            }

            var user = _context.USUARIOs.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Usuario no encontrado.";
                ViewBag.Email = email;
                return View();
            }

            user.Contrasena = EncryptUtility.Hash(newPassword);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Index", "Home");
        }


    }
}
