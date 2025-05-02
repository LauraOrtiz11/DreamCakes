using System.Collections.Generic;
using DreamCakes.Dtos;

namespace DreamCakes.Dtos.Admin
{
    public class AdminCategoryDto
    {
        public int ID_Category { get; set; }
        public string CatName { get; set; }
        public string CatDescription { get; set; }
        public bool CatIsActive { get; set; }
    }
}