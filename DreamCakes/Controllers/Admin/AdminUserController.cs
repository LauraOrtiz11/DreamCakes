using DreamCakes.Dtos.Admin;
using DreamCakes.Services.Admin;
using System.Web.Mvc;
using DreamCakes.Utilities;

namespace DreamCakes.Controllers.Admin
{
    [RoleAuthorizeUtility(1)]
    public class AdminUserController : Controller
    {
        private readonly AdminUserService _service = new AdminUserService();

        public ActionResult Index()
        {
            var users = _service.GetAllUsers();
            return View(users);
        }
        public ActionResult Details(int id)
        {
            var user = _service.GetUserById(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }
        public ActionResult Edit(int id)
        {
            var user = _service.GetUserById(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(AdminUserDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            _service.UpdateUser(dto);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var user = _service.GetUserById(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            _service.DeleteUser(id);
            return RedirectToAction("Index");
        }
    }
}
