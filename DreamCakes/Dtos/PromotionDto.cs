using System;


namespace DreamCakes.Models.DTO
{
    public class PromotionDTO
    {
        public int Id { get; set; }
        public string NombreProm { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool EstadoProm { get; set; }
    }
}