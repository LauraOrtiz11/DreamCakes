using DreamCakes.Dtos;
using DreamCakes.Repositories;
using System.Collections.Generic;

namespace DreamCakes.Services
{
    public class CategoryService
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
        }

        // Obtiene la lista completa de categorías, incluyendo inactivas.
        public List<CategoryDto> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        // Obtiene únicamente las categorías activas.
        public List<CategoryDto> GetActiveCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }
    }
}