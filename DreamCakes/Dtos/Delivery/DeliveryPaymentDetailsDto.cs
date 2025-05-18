using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Delivery
{
    public class DeliveryPaymentDetailsDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
    }
}