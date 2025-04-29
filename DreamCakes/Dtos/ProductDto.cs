using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DreamCakes.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryName { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
    }
}