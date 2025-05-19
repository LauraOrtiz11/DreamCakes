using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Delivery
{
    public class DeliveryOrderStatusUpdateDto
    {
        public int OrderId { get; set; }
        public int DeliveryUserId { get; set; }
        public int NewStatusId { get; set; }
    }
}