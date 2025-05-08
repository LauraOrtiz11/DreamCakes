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

        public List<AdminCategoryDto> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public List<AdminCategoryDto> GetActiveCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }

        public AdminCategoryDto GetCategoryById(int categoryId)
        {
            return _categoryRepository.GetCategoryById(categoryId);
        }

        public void CreateCategory(AdminCategoryDto catDto)
        {
            _categoryRepository.CreateCategory(catDto);
        }

        public void UpdateCategory(AdminCategoryDto catDto)
        {
            _categoryRepository.UpdateCategory(catDto);
        }

        public void DeleteCategory(int categoryId)
        {
            _categoryRepository.DeleteCategory(categoryId);
        }
    }
}
