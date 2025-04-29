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

        public List<CategoryDto> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public List<CategoryDto> GetActiveCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }
    }
}