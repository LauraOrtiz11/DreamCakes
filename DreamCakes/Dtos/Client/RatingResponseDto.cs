using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    
        public class RatingResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public decimal NewAverageRating { get; set; }
            public int Response { get; set; }
    
    }
}