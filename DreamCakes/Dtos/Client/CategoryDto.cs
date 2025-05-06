using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    public class CategoryDto
    {
        public int ID_Category { get; set; }
        public string CatName { get; set; }
        public string CatDescription { get; set; }
        public bool CatIsActive { get; set; }
        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}