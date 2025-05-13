using DreamCakes.Dtos;
using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using System;



namespace DreamCakes.Repositories.Admin
{
    public class AdminCategoryRepository
    {
        private readonly DreamCakesEntities _context;

        public AdminCategoryRepository()
        {
            _context = new DreamCakesEntities();
        }

        public List<AdminCategoryDto> GetAllCategories()
        {
            return _context.CATEGORIAs
                .Select(c => new AdminCategoryDto
                {
                    ID_Category = c.ID_Categoria,
                    CatName = c.Nom_Categ,
                    CatDescription = c.Descrip_Categ,
                    CatIsActive = c.Estado
                }).ToList();
        }

        public List<AdminCategoryDto> GetActiveCategories()
        {
            return _context.CATEGORIAs
                .Where(c => c.Estado)
                .Select(c => new AdminCategoryDto
                {
                    ID_Category = c.ID_Categoria,
                    CatName = c.Nom_Categ,
                    CatDescription = c.Descrip_Categ,
                    CatIsActive = c.Estado
                }).ToList();
        }

        public AdminCategoryDto GetCategoryById(int categoryId)
        {
            var category = _context.CATEGORIAs.Find(categoryId);
            if (category == null) return null;

            return new AdminCategoryDto
            {
                ID_Category = category.ID_Categoria,
                CatName = category.Nom_Categ,
                CatDescription = category.Descrip_Categ,
                CatIsActive = category.Estado
            };
        }

        public void CreateCategory(AdminCategoryDto catDto)
        {
            try
            {
                var newCategory = new CATEGORIA
                {
                    Nom_Categ = catDto.CatName,
                    Descrip_Categ = catDto.CatDescription,
                    Estado = catDto.CatIsActive
                };

                _context.CATEGORIAs.Add(newCategory);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errors = new List<string>();
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errors.Add($"Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                    }
                }

                throw new Exception("Validation failed: " + string.Join("; ", errors));
            }
        }



        public void UpdateCategory(AdminCategoryDto catDto)
        {
            var entity = _context.CATEGORIAs.Find(catDto.ID_Category);
            if (entity == null) return;

            entity.Nom_Categ = catDto.CatName;
            entity.Descrip_Categ = catDto.CatDescription;
            entity.Estado = catDto.CatIsActive;
            _context.SaveChanges();
        }

        public void DeleteCategory(int categoryId)
        {
            var entity = _context.CATEGORIAs.Find(categoryId);
            if (entity == null) return;

            _context.CATEGORIAs.Remove(entity);
            _context.SaveChanges();
        }
    }
}
