using System.Collections.Generic;

namespace DreamCakes.Dtos.Client
{
    public class ProductsByCategoryDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public Dictionary<string, List<ProductDto>> ProductsByCategory { get; set; } = new Dictionary<string, List<ProductDto>>();
        public int Response { get; set; }
        public string Message { get; set; }
    }
}
