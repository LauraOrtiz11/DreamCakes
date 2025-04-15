using System.Linq;
using System.Web.Mvc;
using DreamCakes.Repositories.Models;
using DreamCakes.Utilities;

namespace DreamCakes.Controllers
{
    public class LoginController : Controller
    {
        private DreamCakesEntities db = new DreamCakesEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Por favor ingrese el email y la contraseña.";
                return View();
            }

            var usuario = db.USUARIOs.FirstOrDefault(u => u.Email == email);
            if (usuario != null && BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
            {
                // Login exitoso
                Session["UsuarioID"] = usuario.ID_Usuario;
                Session["Nombre"] = usuario.Nombres;
                Session["Rol"] = usuario.ID_Rol;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email o contraseña incorrectos.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
