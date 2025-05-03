using System;

namespace DreamCakes.Dtos.Client
{
    public class ProductRatingDto
    {
        public int RatingID { get; set; }
        public string ClientName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}