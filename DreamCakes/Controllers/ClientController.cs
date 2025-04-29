using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DreamCakes.Utilities;

namespace DreamCakes.Controllers
{
        [RoleAuthorizeUtility(2)] 
        public class ClientController : Controller
        {
            public ActionResult Catalog()
            {
                return View();
            }
        }
    
}