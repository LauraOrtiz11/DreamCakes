using System.Collections.Generic;

namespace DreamCakes.Dtos.Client
{
    public class ProductDetailsDto : ProductDto
    {
        public List<ProductRatingDisplayDto> Ratings { get; set; } = new List<ProductRatingDisplayDto>();
    }
}