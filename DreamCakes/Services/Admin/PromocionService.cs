using System;
using System.Collections.Generic;
using System.Linq;
using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Admin;
using DreamCakes.Repositories.Models;

namespace DreamCakes.Services.Admin
{
    // Clase que contiene la lógica de negocio para manejar promociones.
    public class PromotionService
    {
        // Instancia directa del repositorio de promociones.
        private PromotionRepository repository = new PromotionRepository();

        // Método que obtiene todas las promociones y las convierte en DTO.

        public List<PromotionDto> GetPromotions()
        {
            // Obtiene todas las promociones desde el repositorio.
            var promotions = repository.GetAll();

            // Convierte las entidades Promocione a PromotionDTO.
            return promotions.Select(p => new PromotionDto
            {
                ID_Prom = p.ID_Promocion,
                NameProm = p.Nombre_Prom,
                DiscountPer = p.Porc_Desc,
                StartDate = p.Fecha_Ini,
                EndDate = p.Fecha_Fin,
                StateProm = p.Estado,
                DescriProm = p.Descrip_Prom
            }).ToList();
        }

        // Método que obtiene una promoción específica por su Id.
        public PromotionDto GetPromotionById(int id)
        {
            // Busca la promoción usando el repositorio.
            var promo = repository.GetById(id);

            // Si no existe, devuelve null.
            if (promo == null) return null;

            // Convierte la entidad a DTO.

            return new PromotionDto
            {
                ID_Prom = promo.ID_Promocion,
                NameProm = promo.Nombre_Prom,
                DiscountPer = promo.Porc_Desc,
                StartDate = promo.Fecha_Ini,
                EndDate = promo.Fecha_Fin,
                StateProm = promo.Estado,
                DescriProm = promo.Descrip_Prom
            };
        }

        // Método para agregar una nueva promoción.
        public void AddPromotion(PromotionDto dto)
        {
            var promo = new PROMOCION
            {
                Nombre_Prom = dto.NameProm,
                Porc_Desc = dto.DiscountPer,
                Fecha_Ini = dto.StartDate,
                Fecha_Fin = dto.EndDate,
                Estado = dto.StateProm,
                Descrip_Prom = dto.DescriProm
            };

           
            repository.Add(promo);
        }



        // Método para actualizar una promoción existente.

        public void UpdatePromotion(PromotionDto dto)
        {
            var promo = new PROMOCION
            {
                ID_Promocion = dto.ID_Prom,
                Nombre_Prom = dto.NameProm,
                Porc_Desc = dto.DiscountPer,
                Fecha_Ini = dto.StartDate,
                Fecha_Fin = dto.EndDate,
                Estado = dto.StateProm,
                Descrip_Prom = dto.DescriProm
            };

            repository.Update(promo);
        }


        // Método para eliminar una promoción por Id.
        public void DeletePromotion(int id)
        {
            // Elimina la promoción usando el repositorio.
            repository.Delete(id);
        }

        // Método para cambiar el estado (activo/inactivo) de una promoción.
        public void TogglePromotionStatus(int id)
        {
            // Cambia el estado usando el repositorio.
            repository.ToggleStatus(id);
        }
    }
}
