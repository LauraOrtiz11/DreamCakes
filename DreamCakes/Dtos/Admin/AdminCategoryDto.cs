using System.ComponentModel.DataAnnotations;

namespace DreamCakes.Dtos.Admin
{
    public class AdminCategoryDto
    {
        public int ID_Category { get; set; }

        [Display(Name = "Nombre de la Categoría")]
        public string CatName { get; set; }

        [Display(Name = "Descripción de la Categoría")]
        public string CatDescription { get; set; }

        [Display(Name = "¿Está Activa?")]
        public bool CatIsActive { get; set; }
    }
}
