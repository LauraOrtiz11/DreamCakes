using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Admin;
using System.Collections.Generic;

namespace DreamCakes.Services.Admin
{
    public class AdminCategoryService
    {
        private readonly AdminCategoryRepository _categoryRepository;

        public AdminCategoryService()
        {
            _categoryRepository = new AdminCategoryRepository();
        }

        // Obtiene la lista completa de categorías, incluyendo inactivas.
        public List<AdminCategoryDto> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        // Obtiene únicamente las categorías activas.
        public List<AdminCategoryDto> GetActiveCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }
    }
}