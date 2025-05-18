using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DreamCakes.Repositories.Delivery;
using DreamCakes.Dtos.Delivery;


namespace DreamCakes.Services.Delivery

{
    public class DeliveryHistoryService
    {
        private readonly DeliveryHistoryRepository _repository;

        public DeliveryHistoryService()
        {
            _repository = new DeliveryHistoryRepository();
        }

        public List<DeliveryHistoryDto> GetDeliveryHistory(DeliveryHistoryFilterDto filter)
        {
            try
            {
                return _repository.GetDeliveryHistory(
                    filter.DeliveryUserId,
                    filter.StartDate,
                    filter.EndDate);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}