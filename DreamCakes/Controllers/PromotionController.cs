using System;
using System.Web.Mvc;
using DreamCakes.Services;
using DreamCakes.Models.DTO;

namespace DreamCakes.Controllers
{
    // Controlador encargado de manejar las vistas y acciones relacionadas con promociones.
    public class PromotionsController : Controller
    {
        // Instancia directa del servicio de promociones.
        private PromotionService service = new PromotionService();

        // Acción que muestra la lista de promociones.
        public ActionResult Index()
        {
            // Obtiene la lista de promociones desde el servicio.
            var promociones = service.GetPromotions();

            // Envía la lista a la vista.
            return View(promociones);
        }

        // Acción que muestra el formulario de creación de promoción.
        public ActionResult Create()
        {
            // Devuelve la vista vacía para crear una promoción.
            return View();
        }

        // Acción que recibe los datos del formulario de creación.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromotionDTO dto)
        {
            // Verifica que los datos del formulario sean válidos.
            if (ModelState.IsValid)
            {
                // Agrega la promoción usando el servicio.
                service.AddPromotion(dto);

                // Redirige a la lista de promociones.
                return RedirectToAction("Index");
            }

            // Si hay errores, vuelve a mostrar el formulario con los datos ingresados.
            return View(dto);
        }

        // Acción que muestra el formulario de edición de una promoción específica.
        public ActionResult Edit(int id)
        {
            // Obtiene la promoción a editar usando el servicio.
            var promo = service.GetPromotionById(id);

            // Si no existe, devuelve error 404.
            if (promo == null) return HttpNotFound();

            // Devuelve la vista con los datos de la promoción.
            return View(promo);
        }

        // Acción que recibe los datos del formulario de edición.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PromotionDTO dto)
        {
            // Verifica que los datos del formulario sean válidos.
            if (ModelState.IsValid)
            {
                // Actualiza la promoción usando el servicio.
                service.UpdatePromotion(dto);

                // Redirige a la lista de promociones.
                return RedirectToAction("Index");
            }

            // Si hay errores, vuelve a mostrar el formulario con los datos ingresados.
            return View(dto);
        }

        // Acción que muestra los detalles de una promoción específica.
        public ActionResult Details(int id)
        {
            // Obtiene la promoción usando el servicio.
            var promo = service.GetPromotionById(id);

            // Si no existe, devuelve error 404.
            if (promo == null) return HttpNotFound();

            // Devuelve la vista con los detalles de la promoción.
            return View(promo);
        }

        // Acción que muestra la confirmación para eliminar una promoción.
        public ActionResult Delete(int id)
        {
            // Obtiene la promoción a eliminar.
            var promo = service.GetPromotionById(id);

            // Si no existe, devuelve error 404.
            if (promo == null) return HttpNotFound();

            // Devuelve la vista de confirmación de eliminación.
            return View(promo);
        }

        // Acción que procesa la eliminación de una promoción.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Elimina la promoción usando el servicio.
            service.DeletePromotion(id);

            // Redirige a la lista de promociones.
            return RedirectToAction("Index");
        }

        // Acción para cambiar el estado activo/inactivo de una promoción.
        public ActionResult ToggleStatus(int id)
        {
            // Cambia el estado de la promoción usando el servicio.
            service.TogglePromotionStatus(id);

            // Redirige a la lista de promociones.
            return RedirectToAction("Index");
        }
    }
}
