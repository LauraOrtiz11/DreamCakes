using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Repositories.Delivery;
using DreamCakes.Dtos.Delivery;

namespace DreamCakes.Services.Delivery
{
    public class DeliveryOrderService
    {
        private readonly DeliveryOrderRepository _repository;

        public DeliveryOrderService()
        {
            _repository = new DeliveryOrderRepository();
        }
        public bool IsOrderFullyPaid(int orderId)
        {
            return _repository.IsOrderFullyPaid(orderId);
        }
        

        public decimal GetAmountPaid(int orderId)
        {
            return _repository.GetAmountPaid(orderId);
        }
        public DeliveryOrderListResponse GetAssignedOrders(int deliveryUserId, string statusFilter = null)
        {
            try
            {
                var orders = _repository.GetAssignedOrders(deliveryUserId, statusFilter);

                return new DeliveryOrderListResponse
                {
                    Success = true,
                    Orders = orders,
                    Message = orders.Any() ? "" : "No hay pedidos asignados"
                };
            }
            catch (Exception ex)
            {
                return new DeliveryOrderListResponse
                {
                    Success = false,
                    Message = "Error al cargar los pedidos",
                    TechnicalMessage = ex.Message
                };
            }
        }

        public DeliveryOrderDetailResponse GetOrderDetails(int orderId, int deliveryUserId)
        {
            try
            {
                var order = _repository.GetOrderDetails(orderId, deliveryUserId);

                if (order == null)
                {
                    return new DeliveryOrderDetailResponse
                    {
                        Success = false,
                        Message = "Pedido no encontrado o no asignado"
                    };
                }

                return new DeliveryOrderDetailResponse
                {
                    Success = true,
                    Order = order
                };
            }
            catch (Exception ex)
            {
                return new DeliveryOrderDetailResponse
                {
                    Success = false,
                    Message = "Error al cargar los detalles del pedido",
                    TechnicalMessage = ex.Message
                };
            }
        }
    }


   
}