using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Delivery
{
    public class DeliveryOrderDetailDto : DeliveryAssignedOrderDto
    {
        public List<DeliveryOrderItemDto> Items { get; set; }
        public string AdditionalNotes { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public int StatusId { get; set; }
        public int? DeliveryUserId { get; set; }
       
    }
}