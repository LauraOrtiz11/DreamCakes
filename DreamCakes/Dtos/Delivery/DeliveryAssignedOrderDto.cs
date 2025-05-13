using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Delivery
{
    public class DeliveryAssignedOrderDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public string Status { get; set; }
        public DateTime EstimatedDeliveryTime { get; set; }
        public decimal TotalAmount { get; set; }
    }
}