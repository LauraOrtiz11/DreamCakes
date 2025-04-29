using DreamCakes.Dtos;
using DreamCakes.Repositories.Models;
using System.Collections.Generic;
using System.Linq;

namespace DreamCakes.Repositories
{
    public class CategoryRepository
    {
        private readonly DreamCakesEntities _context;

        public CategoryRepository()
        {
            _context = new DreamCakesEntities();
        }

        // Obtiene todas las categorías, incluyendo activas e inactivas.
        public List<CategoryDto> GetAllCategories()
        {
            return _context.CATEGORIAs
                .Select(c => new CategoryDto
                {
                    CategoryId = c.ID_Categoria,
                    Name = c.Nom_Categ,
                    Description = c.Descrip_Categ,
                    IsActive = c.Estado
                }).ToList();
        }

        // Obtiene únicamente las categorías activas (Estado = true).
        public List<CategoryDto> GetActiveCategories()
        {
            return _context.CATEGORIAs
                .Where(c => c.Estado)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.ID_Categoria,
                    Name = c.Nom_Categ,
                    Description = c.Descrip_Categ
                }).ToList();
        }
    }
}