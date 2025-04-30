using System;


namespace DreamCakes.Models.DTO
{
    public class PromotionDTO
    {
        public int ID_Prom { get; set; }
        public string NameProm { get; set; }
        public decimal DiscountPer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool StateProm { get; set; }
        public string DescriProm { get; set; }
    }
}