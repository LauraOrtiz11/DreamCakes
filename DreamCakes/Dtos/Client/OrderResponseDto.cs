using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Dtos;
namespace DreamCakes.Dtos.Client
{
    public class OrderResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public OrderDto Order { get; set; }
        public string ErrorLocation { get; set; } // "Repository", "Service", "Controller"
        public string TechnicalMessage { get; set; }
    }
}