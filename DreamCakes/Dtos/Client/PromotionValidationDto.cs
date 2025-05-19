using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    public class PromotionValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int? PromotionId { get; set; }
    }
}