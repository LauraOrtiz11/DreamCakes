using System;
using System.Web.Mvc;
using System.Collections.Generic;
using DreamCakes.Services.Admin;
using DreamCakes.Dtos.Admin;

namespace DreamCakes.Controllers.Admin
{
    public class PromotionsController : Controller
    {
        private PromotionService service = new PromotionService();

        public ActionResult Index()
        {
            var promociones = service.GetPromotions();
            return View(promociones);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromotionDto dto)
        {
            if (ModelState.IsValid)
            {
                // ✅ Validación personalizada: la fecha de fin no puede ser anterior a la de inicio
                if (dto.EndDate < dto.StartDate)
                {
                    ModelState.AddModelError("EndDate", "La fecha de fin no puede ser anterior a la fecha de inicio.");
                    return View(dto);
                }

                if (dto.DiscountPer.ToString().Contains("."))
                {
                    ModelState.AddModelError("DiscountPer", "Use una coma (,) como separador decimal en el porcentaje de descuento.");
                }


                try
                {
                    service.AddPromotion(dto);
                    return RedirectToAction("Index");
                }
                catch (UnauthorizedAccessException)
                {
                    return View("~/Views/Error/Unauthorized.cshtml");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al crear la promoción: " + ex.Message);
                }
            }

            return View(dto);
        }


        public ActionResult Edit(int id)
        {
            try
            {
                var promo = service.GetPromotionById(id);
                if (promo == null) return View("~/Views/Error/NotFound.cshtml");
                return View(promo);
            }
            catch (Exception)
            {
                return View("~/Views/Error/General.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(PromotionDto dto)
        
        {
            if (ModelState.IsValid)
            {
                try
                {
                    service.UpdatePromotion(dto);
                    return RedirectToAction("Index");
                }
                catch (UnauthorizedAccessException)
                {
                    return View("~/Views/Error/Unauthorized.cshtml");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al editar la promoción: " + ex.Message);
                }
            }

            return View(dto);
        }

        public ActionResult Details(int id)
        {
            try
            {
                var promo = service.GetPromotionById(id);
                if (promo == null) return View("~/Views/Error/NotFound.cshtml");
                return View(promo);
            }
            catch (Exception)
            {
                return View("~/Views/Error/General.cshtml");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var promo = service.GetPromotionById(id);
                if (promo == null) return View("~/Views/Error/NotFound.cshtml");
                return View(promo);
            }
            catch (Exception)
            {
                return View("~/Views/Error/General.cshtml");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                service.DeletePromotion(id);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return View("~/Views/Error/Unauthorized.cshtml");
            }
            catch (KeyNotFoundException)
            {
                return View("~/Views/Error/NotFound.cshtml");
            }
            catch (Exception)
            {
                return View("~/Views/Error/General.cshtml");
            }
        }

        public ActionResult ToggleStatus(int id)
        {
            try
            {
                service.TogglePromotionStatus(id);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return View("~/Views/Error/Unauthorized.cshtml");
            }
            catch (Exception)
            {
                return View("~/Views/Error/General.cshtml");
            }
        }
    }
}

