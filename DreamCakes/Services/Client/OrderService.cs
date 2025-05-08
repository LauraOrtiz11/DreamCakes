using System;
using System.Collections.Generic;
using System.Linq;
using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Client;

namespace DreamCakes.Services.Client
{
    public class OrderService : IDisposable
    {
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;

        public OrderService()
        {
            _orderRepository = new OrderRepository();
            _productRepository = new ProductRepository();
        }

        public OrderResponseDto ValidatePromotionCode(string promotionCode)
        {
            try
            {
                var promotion = _orderRepository.GetPromotionByCode(promotionCode);

                if (promotion == null)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "Código de descuento no válido, por favor verificar"
                    };
                }

                return new OrderResponseDto
                {
                    Success = true,
                    Message = "Descuento aplicado correctamente",
                    Order = new OrderDto { PromotionCode = promotion.NameProm }
                };
            }
            catch (Exception ex)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Message = "Problemas internos, por favor intente de nuevo más tarde"
                };
            }
        }
        public OrderResponseDto AddToCart(List<OrderDetailDto> currentCart, int productId, int quantity)
        {
            try
            {
                // 1. Validación básica
                if (quantity < 1)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "La cantidad debe ser al menos 1",
                        ErrorLocation = "Service: Validación inicial"
                    };
                }

                // 2. Obtener producto
                var product = _orderRepository.GetProductById(productId);

                // 3. Validar stock
                if (quantity > product.Stock)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = $"No hay suficiente stock. Disponible: {product.Stock} unidades",
                        ErrorLocation = "Service: Validación de stock"
                    };
                }

                // 4. Lógica para añadir al carrito
                var cart = currentCart ?? new List<OrderDetailDto>();
                var existingItem = cart.FirstOrDefault(p => p.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.Subtotal = existingItem.UnitPrice * existingItem.Quantity;
                }
                else
                {
                    cart.Add(new OrderDetailDto
                    {
                        ProductId = productId,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = quantity,
                        Subtotal = product.Price * quantity
                    });
                }

                return new OrderResponseDto
                {
                    Success = true,
                    Message = $"{product.Name} añadido al carrito",
                    Order = new OrderDto { Details = cart }
                };
            }
            catch (Exception ex)
            {
                // Registrar el error completo
                System.Diagnostics.Debug.WriteLine($"ERROR EN AddToCart: {ex.ToString()}");

                return new OrderResponseDto
                {
                    Success = false,
                    Message = GetUserFriendlyErrorMessage(ex),
                    ErrorLocation = GetErrorLocation(ex),
                    TechnicalMessage = ex.Message
                };
            }
        }

        private string GetUserFriendlyErrorMessage(Exception ex)
        {
            if (ex.Message.Contains("no encontrado"))
                return "El producto solicitado no está disponible";
            if (ex.Message.Contains("base de datos"))
                return "Error al acceder a la información del producto";
            return "Ocurrió un error al procesar tu solicitud";
        }

        private string GetErrorLocation(Exception ex)
        {
            if (ex.Message.Contains("OrderRepository"))
                return "Repository";
            if (ex.Message.Contains("Service"))
                return "Service";
            return "Unknown";
        }

        public OrderResponseDto RemoveFromCart(List<OrderDetailDto> currentCart, int productId)
        {
            try
            {
                var cart = currentCart ?? new List<OrderDetailDto>();
                var itemToRemove = cart.FirstOrDefault(p => p.ProductId == productId);

                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                }

                return new OrderResponseDto
                {
                    Success = true,
                    Message = "Producto removido del carrito",
                    Order = new OrderDto { Details = cart }
                };
            }
            catch (Exception ex)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Message = $"Error al remover del carrito: {ex.Message}"
                };
            }
        }

        public OrderResponseDto CreateOrder(OrderDto order)
        {
            try
            {
                if (order.Details == null || !order.Details.Any())
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "El pedido no puede estar vacío"
                    };
                }

                // Apply promotion if exists
                if (!string.IsNullOrEmpty(order.PromotionCode))
                {
                    ApplyPromotion(order);
                }

                // Save order
                var orderId = _orderRepository.CreateOrder(order);

                return new OrderResponseDto
                {
                    Success = true,
                    Message = order.OrderType == "Programado" ?
                        "Pedido programado de manera correcta" : "Pedido realizado correctamente",
                    Order = order
                };
            }
            catch (Exception ex)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Message = "Error al procesar el pedido: " + ex.Message
                };
            }
        }

        private void ApplyPromotion(OrderDto order)
        {
            var promotion = _orderRepository.GetPromotionByCode(order.PromotionCode);
            if (promotion != null)
            {
                foreach (var detail in order.Details)
                {
                    detail.UnitPrice = detail.UnitPrice * (1 - (promotion.DiscountPer / 100));
                    detail.Subtotal = detail.UnitPrice * detail.Quantity;
                    detail.PromotionId = promotion.ID_Prom;
                }
                order.Total = order.Details.Sum(d => d.Subtotal);
            }
        }

        public void Dispose()
        {
            // Cleanup resources if needed
        }
    }
}