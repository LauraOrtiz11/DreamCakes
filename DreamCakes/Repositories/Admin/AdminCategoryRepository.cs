using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Models;
using System.Collections.Generic;
using System.Linq;
using DreamCakes.Dtos;

namespace DreamCakes.Repositories.Admin
{
    public class AdminCategoryRepository
    {
        private readonly DreamCakesEntities _context;

        public AdminCategoryRepository()
        {
            _context = new DreamCakesEntities();
        }

        // Obtiene todas las categorías, incluyendo activas e inactivas.
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

        // Obtiene únicamente las categorías activas (Estado = true).
        public List<AdminCategoryDto> GetActiveCategories()
        {
            return _context.CATEGORIAs
                .Where(c => c.Estado)
                .Select(c => new AdminCategoryDto
                {
                    ID_Category = c.ID_Categoria,
                    CatName = c.Nom_Categ,
                    CatDescription = c.Descrip_Categ
                }).ToList();
        }
    }
}