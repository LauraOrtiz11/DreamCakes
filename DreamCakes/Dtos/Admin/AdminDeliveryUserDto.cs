using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Admin
{
    public class AdminDeliveryUserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }
}