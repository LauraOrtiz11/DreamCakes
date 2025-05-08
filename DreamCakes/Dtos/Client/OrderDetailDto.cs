using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Dtos;
namespace DreamCakes.Dtos.Client
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? PromotionId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

}