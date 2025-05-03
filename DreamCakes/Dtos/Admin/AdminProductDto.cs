using System;
using System.Collections.Generic;

using DreamCakes.Dtos;

namespace DreamCakes.Dtos.Admin
{
    public class AdminProductDto 
    {
        public int ID_Product { get; set; }
        public int ID_Category { get; set; }
        public string ProdName { get; set; }
        public string ProdDescription { get; set; }
        public decimal ProdPrice { get; set; }
        public int ProdStock { get; set; }
        public string CategoryName { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
    }
}
