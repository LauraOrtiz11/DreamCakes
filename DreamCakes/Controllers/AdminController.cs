using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DreamCakes.Utilities;
namespace DreamCakes.Controllers
{
    [RoleAuthorizeUtility(1)]
    public class AdminController : Controller
    {

        public ActionResult Promotion()
        {
            return View();
        }

        public ActionResult Catalog()
        {
            return View();
        }

        public ActionResult Category()
        {
            return View();
        }

        public ActionResult Inventory()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }
    }
}