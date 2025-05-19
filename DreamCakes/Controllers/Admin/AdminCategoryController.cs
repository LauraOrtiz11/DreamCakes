using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DreamCakes.Dtos.Admin;
using DreamCakes.Services.Admin;
using DreamCakes.Utilities;

namespace DreamCakes.Controllers.Admin
{
    [RoleAuthorizeUtility(1)]
    public class AdminCategoryController : Controller
    {
        private readonly AdminCategoryService _categoryService = new AdminCategoryService();

        // GET: AdminCategory
        public ActionResult Index()
        {
            var categoryList = _categoryService.GetAllCategories();
            return View(categoryList);
        }

        // GET: AdminCategory/Details/5
        public ActionResult Details(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // GET: AdminCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCategoryDto catDto)
        {
            if (ModelState.IsValid)
            {
                _categoryService.CreateCategory(catDto);
                return RedirectToAction("Index");
            }
            return View(catDto);
        }

        // GET: AdminCategory/Edit/5
        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // POST: AdminCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminCategoryDto catDto)
        {
            if (ModelState.IsValid)
            {
                _categoryService.UpdateCategory(catDto);
                return RedirectToAction("Index");
            }
            return View(catDto);
        }

        // GET: AdminCategory/Delete/5
        public ActionResult Delete(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // POST: AdminCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _categoryService.DeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}
