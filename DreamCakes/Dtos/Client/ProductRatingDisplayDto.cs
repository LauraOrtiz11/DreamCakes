using System;

namespace DreamCakes.Dtos.Client
{
    public class ProductRatingDisplayDto
    {
        public string ClientName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}