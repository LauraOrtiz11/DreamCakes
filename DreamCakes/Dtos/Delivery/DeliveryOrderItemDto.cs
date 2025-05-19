using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Delivery
{
    public class DeliveryOrderItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
    public class DeliveryOrderListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TechnicalMessage { get; set; }
        public List<DeliveryAssignedOrderDto> Orders { get; set; }
    }

    public class DeliveryOrderDetailResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TechnicalMessage { get; set; }
        public DeliveryOrderDetailDto Order { get; set; }
    }
}