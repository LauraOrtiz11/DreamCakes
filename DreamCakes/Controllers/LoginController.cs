using System.Web.Mvc;
using DreamCakes.Services;
using DreamCakes.Dtos;

namespace DreamCakes.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginService loginService;

        public LoginController()
        {
            loginService = new LoginService();
        }

        [HttpPost]
        public ActionResult Index(LoginDto loginDto)
        {
            var result = loginService.Login(loginDto);
            TempData["LoginMessage"] = result.Message;

            if (result.Response == 1)
            {
                Session["UserEmail"] = loginDto.Email;
                return RedirectToAction("Index", "Home"); // Inicio exitoso
            }

            // Aquí corregimos la redirección a la vista correcta
            return RedirectToAction("LoginIndex", "Login");
        }

        public ActionResult LoginIndex()
        {
            return View();
        }
    }
}
