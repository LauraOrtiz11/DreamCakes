using System.Web.Mvc;

namespace DreamCakes.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Unauthorized()
        {
            Response.StatusCode = 403;
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult General()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}