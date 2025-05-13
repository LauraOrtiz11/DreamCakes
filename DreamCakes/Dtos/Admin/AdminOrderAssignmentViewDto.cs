using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Admin
{
    public class AdminOrderAssignmentViewDto
    {
        public List<AdminUnassignedOrderDto> UnassignedOrders { get; set; } = new List<AdminUnassignedOrderDto>();
        public List<AdminDeliveryUserDto> DeliveryUsers { get; set; } = new List<AdminDeliveryUserDto>();
        public string ErrorMessage { get; set; }
    }

}