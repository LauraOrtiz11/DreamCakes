using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DreamCakes.Utilities;

namespace DreamCakes.Controllers
{
        // GET: Client
        
        [RoleAuthorizeUtility(2)] 
        public class ClientController : Controller
        {
            public ActionResult Dashboard()
            {
                return View();
            }

        public ActionResult Catalog()
        {
            return View();
        }
    }
    
}