using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Admin
{
    public class AdminAssignOrderDto
    {
        public int OrderId { get; set; }
        public int DeliveryUserId { get; set; }
    }
}