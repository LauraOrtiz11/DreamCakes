using DreamCakes.Dtos;
using DreamCakes.Services;
using System.Web.Mvc;

namespace DreamCakes.Controllers
{
    [Authorize] // Requiere autenticación
    public class ProfileController : Controller
    {
        private readonly UserService _userService;

        public ProfileController()
        {
            _userService = new UserService();
        }

        public ActionResult Index()
        {
            // Obtener ID del usuario logueado desde la sesión
            var userId = (int)Session["ID_Usuario"];
            var profile = _userService.GetUserProfile(userId);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileDto model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.UpdateUserProfile(model))
                {
                    TempData["SuccessMessage"] = "Perfil actualizado correctamente";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error al actualizar el perfil");
            }
            return View(model);
        }
    }
}