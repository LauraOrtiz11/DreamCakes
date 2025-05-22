using DreamCakes.Dtos.Client;
using DreamCakes.Repositories.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DreamCakes.Services.Client
{
    public class OrderService 
    {
        private readonly OrderRepository _orderRepository;
        private readonly EmailService _emailService;

        public OrderService()
        {
            _orderRepository = new OrderRepository();
            _emailService = new EmailService();

        }

        public OrderResponseDto ValidatePromotionForProduct(string promotionCode, int productId)
        {
            try
            {
                // Validar que el producto exista
                var product = _orderRepository.GetProductById(productId);
                if (product == null)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "Producto no encontrado",
                        ErrorLocation = "OrderService.ValidatePromotionForProduct"
                    };
                }

                // Obtener la promoción
                var promotion = _orderRepository.GetPromotionByCode(promotionCode);
                if (promotion == null)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "Código de promoción no válido",
                        ErrorLocation = "OrderService.ValidatePromotionForProduct"
                    };
                }

                // Verificar que la promoción aplique al producto
                var isValid = _orderRepository.ValidateProductPromotion(productId, promotion.ID_Prom);
                if (!isValid)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = $"La promoción '{promotion.NameProm}' no aplica para este producto",
                        ErrorLocation = "OrderService.ValidatePromotionForProduct"
                    };
                }

                return new OrderResponseDto
                {
                    Success = true,
                    Message = $"Promoción '{promotion.NameProm}' aplicada correctamente",
                    Promotion = promotion
                };
            }
            catch (Exception ex)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Message = "Error al validar la promoción",
                    TechnicalMessage = ex.Message,
                    ErrorLocation = "OrderService.ValidatePromotionForProduct"
                };
            }
        }

        public OrderResponseDto AddToCart(List<OrderDetailDto> currentCart, int productId, int quantity)
        {
            try
            {
                // Validar cantidad
                if (quantity <= 0)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "La cantidad debe ser mayor a cero",
                        ErrorLocation = "OrderService.AddToCart"
                    };
                }

                // Obtener producto
                var product = _orderRepository.GetProductById(productId);
                if (product == null)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "Producto no encontrado",
                        ErrorLocation = "OrderService.AddToCart"
                    };
                }

                // Verificar stock
                if (product.Stock < quantity)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = $"No hay suficiente stock. Disponible: {product.Stock}",
                        ErrorLocation = "OrderService.AddToCart"
                    };
                }

                // Buscar si el producto ya está en el carrito
                var existingItem = currentCart.FirstOrDefault(item => item.ProductId == productId);
                if (existingItem != null)
                {
                    // Actualizar cantidad
                    existingItem.Quantity += quantity;
                    existingItem.UnitPrice = product.Price;
                    existingItem.Subtotal = existingItem.Quantity * existingItem.UnitPrice;
                    existingItem.ProductName = product.Name;
                    existingItem.ProductImageUrl = product.MainImageUrl;
                }
                else
                {
                    // Agregar nuevo item
                    currentCart.Add(new OrderDetailDto
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.Price,
                        Subtotal = product.Price * quantity,
                        ProductName = product.Name,
                        ProductImageUrl = product.MainImageUrl
                    });
                }

                return new OrderResponseDto
                {
                    Success = true,
                    Message = "Producto agregado al carrito",
                    Order = new OrderDto { Details = currentCart }
                };
            }
            catch (Exception ex)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Message = "Error al agregar producto al carrito",
                    TechnicalMessage = ex.Message,
                    ErrorLocation = "OrderService.AddToCart"
                };
            }
        }

        public OrderResponseDto RemoveFromCart(List<OrderDetailDto> currentCart, int productId)
        {
            try
            {
                var itemToRemove = currentCart.FirstOrDefault(item => item.ProductId == productId);
                if (itemToRemove == null)
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "El producto no está en el carrito",
                        ErrorLocation = "OrderService.RemoveFromCart"
                    };
                }

                currentCart.Remove(itemToRemove);

                return new OrderResponseDto
                {
                    Success = true,
                    Message = "Producto eliminado del carrito",
                    Order = new OrderDto { Details = currentCart }
                };
            }
            catch (Exception ex)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Message = "Error al eliminar producto del carrito",
                    TechnicalMessage = ex.Message,
                    ErrorLocation = "OrderService.RemoveFromCart"
                };
            }
        }

        public OrderResponseDto CreateOrder(OrderDto order)
        {
            try
            {

                // Validaciones básicas
                if (order.Details == null || !order.Details.Any())
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "Cart is empty",
                        ErrorLocation = "OrderService.CreateOrder"
                    };
                }


                // Validar dirección para pedidos con entrega
                if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "Debe ingresar una dirección de entrega",
                        ErrorLocation = "OrderService.CreateOrder"
                    };
                }

                // Validar fecha para pedidos programados
                if (order.OrderType == "Programado" && order.DeliveryDate < DateTime.Now.AddHours(2))
                {
                    return new OrderResponseDto
                    {
                        Success = false,
                        Message = "La fecha de entrega debe ser al menos 2 horas después del momento actual",
                        ErrorLocation = "OrderService.CreateOrder"
                    };
                }

                // Validar stock actualizado
                foreach (var detail in order.Details)
                {
                    var product = _orderRepository.GetProductById(detail.ProductId);
                    if (product == null)
                    {
                        return new OrderResponseDto
                        {
                            Success = false,
                            Message = $"Producto ID {detail.ProductId} no encontrado",
                            ErrorLocation = "OrderService.CreateOrder"
                        };
                    }

                    if (product.Stock < detail.Quantity)
                    {
                        return new OrderResponseDto
                        {
                            Success = false,
                            Message = $"No hay suficiente stock de {product.Name}. Disponible: {product.Stock}",
                            ErrorLocation = "OrderService.CreateOrder"
                        };
                    }
                }

                // Crear la orden
                var orderId = _orderRepository.CreateOrder(order);
                // 2. Obtener correo del cliente
                

                // Retornar solo datos básicos (sin relaciones)
                return new OrderResponseDto
                {
                    Success = true,
                    Message = "Order created successfully",
                    OrderId = orderId,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    Total = order.Details.Sum(d => d.Subtotal)
                };

            
            }
            catch (Exception ex)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Message = "Error al crear la orden",
                    TechnicalMessage = ex.Message,
                    ErrorLocation = "OrderService.CreateOrder"
                };
            }
        }
        public OrderDto GetOrderById(int orderId)
        {
            return _orderRepository.GetOrderById(orderId);
        }
        public CartSummaryDto GetCartSummary(List<OrderDetailDto> cartItems)
        {
            return _orderRepository.GetCartSummary(cartItems);
        }
    }
}