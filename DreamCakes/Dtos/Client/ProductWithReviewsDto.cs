using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    public class ProductWithReviewsDto : ProductDto
    {
        // Usamos 'new' para eliminar las advertencias
        public new List<ReviewDto> Reviews { get; set; }
        public new bool CanReview { get; set; }
    }
}