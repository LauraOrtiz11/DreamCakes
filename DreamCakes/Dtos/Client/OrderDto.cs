// === DTO: OrderDto.cs ===
using System;
using System.Collections.Generic;

namespace DreamCakes.Dtos.Client
{
    public class OrderDto
    {
        public int ID_Order { get; set; }
        public int ClientId { get; set; }
        public int? DeliveryUserId { get; set; } = null;
        public int StatusId { get; set; } = 3; // Por defecto Pendiente
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string OrderType { get; set; } // "Inmediato" o "Programado"
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Total { get; set; }
        public List<OrderDetailDto> Details { get; set; }
        public string PromotionCode { get; set; } // Solo para referencia
    }

    public class OrderDetailDto
    {
        public int ID_Detail { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int? PromotionId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal OriginalSubtotal => UnitPrice * Quantity;
        public decimal DiscountAmount => OriginalSubtotal - Subtotal;
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
    }

    public class OrderResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TechnicalMessage { get; set; }
        public string ErrorLocation { get; set; }
        public int? OrderId { get; set; }
        public PromotionDto Promotion { get; set; }
        public OrderDto Order { get; set; }
    }

    public class CartSummaryDto
    {
        public decimal Subtotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
        public string PromotionCode { get; set; }
    }
}
