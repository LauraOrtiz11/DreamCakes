using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Delivery
{
    public class DeliveryPaymentRegisterDto
    {
        public int OrderId { get; set; }
        public int DeliveryUserId { get; set; }
        public decimal AmountReceived { get; set; }
        public bool IsFullPayment { get; set; }
    }
}