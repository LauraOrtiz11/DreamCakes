using System;
using System.Collections.Generic;
using System.Linq;
using DreamCakes.Models.DTO;
using DreamCakes.Repositories;
using DreamCakes.Repositories.Models;

namespace DreamCakes.Services
{
    // Clase que contiene la lógica de negocio para manejar promociones.
    public class PromotionService
    {
        // Instancia directa del repositorio de promociones.
        private PromotionRepository repository = new PromotionRepository();

        // Método que obtiene todas las promociones y las convierte en DTO.
        public List<PromotionDTO> GetPromotions()
        {
            // Obtiene todas las promociones desde el repositorio.
            var promociones = repository.GetAll();

            // Convierte las entidades Promocione a PromotionDTO.
            return promociones.Select(p => new PromotionDTO
            {
                Id = p.ID_Promocion,
                NombreProm = p.Nombre_Prom,
                PorcentajeDescuento = p.Porc_Desc,
                FechaInicio = p.Fecha_Ini,
                FechaFin = p.Fecha_Fin,
                EstadoProm = p.Estado
            }).ToList();
        }

        // Método que obtiene una promoción específica por su Id.
        public PromotionDTO GetPromotionById(int id)
        {
            // Busca la promoción usando el repositorio.
            var promo = repository.GetById(id);

            // Si no existe, devuelve null.
            if (promo == null) return null;

            // Convierte la entidad a DTO.
            return new PromotionDTO
            {
                Id = promo.ID_Promocion,
                NombreProm = promo.Nombre_Prom,
                PorcentajeDescuento = promo.Porc_Desc,
                FechaInicio = promo.Fecha_Ini,
                FechaFin = promo.Fecha_Fin,
                EstadoProm = promo.Estado
            };
        }

        // Método para agregar una nueva promoción.
        public void AddPromotion(PromotionDTO dto)
        {
            var promo = new PROMOCION   
            {
                Nombre_Prom = dto.NombreProm,
                Porc_Desc = dto.PorcentajeDescuento,
                Fecha_Ini = dto.FechaInicio,
                Fecha_Fin = dto.FechaFin,
                Estado = dto.EstadoProm
            };

            repository.Add(promo);
        }


        // Método para actualizar una promoción existente.
        public void UpdatePromotion(PromotionDTO dto)
        {
            var promo = new PROMOCION
            {
                ID_Promocion = dto.Id,
                Nombre_Prom = dto.NombreProm,
                Porc_Desc = dto.PorcentajeDescuento,
                Fecha_Ini = dto.FechaInicio,
                Fecha_Fin = dto.FechaFin,
                Estado = dto.EstadoProm
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
