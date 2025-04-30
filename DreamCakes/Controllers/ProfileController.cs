using DreamCakes.Dtos;
using DreamCakes.Services;
using System.Web.Mvc;
using DreamCakes.Utilities;

namespace DreamCakes.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserService _userService;

        public ProfileController()
        {
            _userService = new UserService();
        }

        // Muestra la vista del perfil del usuario actualmente autenticado, obteniendo su información desde la sesión.
        public ActionResult Index()
        {
            // Obtener ID del usuario logueado desde la sesión
            var userId = (int)Session["ID_Usuario"];
            var profile = _userService.GetUserProfile(userId);
            return View(profile);
        }

        // Procesa la actualización del perfil del usuario si el modelo es válido.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProfileDto model)
        {
            if (ModelState.IsValid)
            {
                if (_userService.UpdateUserProfile(model))
                {
                    TempData["SuccessMessage"] = AuthErrorsUtility.PROFILE_UPDATE;
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", AuthErrorsUtility.PROFILE_UPDATE_ERROR);
            }
            return View(model);
        }
    }
}