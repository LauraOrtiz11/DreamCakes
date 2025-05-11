using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    public class OrderDetailWithPromotionDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string PromotionCode { get; set; } // Nuevo campo para el código
        public int? PromotionId { get; set; }
        public decimal Subtotal { get; set; }
    }
}