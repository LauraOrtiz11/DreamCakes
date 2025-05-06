using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    public class SearchResponseDto
    {
        public PromotionDto CurrentPromotion { get; set; }
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int Response { get; set; }
        public string Message { get; set; }
    }
}