using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Admin
{
    public class AdminProductPromotionDto
    {
        public int ProductId { get; set; }
        public int PromotionId { get; set; }
        public List<PromotionDto> AvailablePromotions { get; set; } = new List<PromotionDto>();
        public List<PromotionDto> CurrentPromotions { get; set; } = new List<PromotionDto>();
    }
}