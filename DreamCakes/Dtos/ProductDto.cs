using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DreamCakes.Dtos
{
    public class ProductDto
    {
        public int ID_Product { get; set; }
        public int ID_Category { get; set; }
        public string ProductName { get; set; }
        public string ProductDescri { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryName { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
    }
}