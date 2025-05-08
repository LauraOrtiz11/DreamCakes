using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Dtos;
namespace DreamCakes.Dtos.Client
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int DeliveryUserId { get; set; }
        public int StatusId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Total { get; set; }
        public string PromotionCode { get; set; }
        public List<OrderDetailDto> Details { get; set; } = new List<OrderDetailDto>();

        public decimal DiscountAmount
        {
            get
            {
                if (Details == null) return 0;
                return Details.Where(d => d.PromotionId.HasValue)
                             .Sum(d => d.UnitPrice * d.Quantity * d.PromotionId.Value / 100);
            }
        }
    }
   
}