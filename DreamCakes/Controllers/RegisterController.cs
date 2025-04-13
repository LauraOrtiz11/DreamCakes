using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DreamCakes.Services;
using DreamCakes.Dtos;

namespace DreamCakes.Controllers
{
    public class RegisterController : Controller
    {
        private readonly RegisterService registerService;

        public RegisterController()
        {
            registerService = new RegisterService();
        }
        [HttpPost]
        public ActionResult Index(RegisterDto userDto)
        {
            var response = registerService.CreateUser(userDto);
            TempData["RegisterMessage"] = response.Message;
            return RedirectToAction("Index", "Home");
        }

    }
}