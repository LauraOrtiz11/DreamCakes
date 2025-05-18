using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Delivery
{
    public class DeliveryPaymentResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal PendingAmount { get; set; }
        public int PaymentId { get; set; }

        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountReceived { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}