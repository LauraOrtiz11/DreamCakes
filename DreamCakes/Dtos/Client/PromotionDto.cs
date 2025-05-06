using System;

namespace DreamCakes.Dtos.Client
{
    public class PromotionDto
    {
        public int ID_Prom { get; set; }
        public string NameProm { get; set; }
        public decimal DiscountPer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool StateProm { get; set; } 
        public string DescriProm { get; set; }
        public int Response { get; set; }
        public string Message { get; set; }
    }
}