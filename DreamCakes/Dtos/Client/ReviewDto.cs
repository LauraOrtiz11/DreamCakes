using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    public class ReviewDto
    {
        public int ID_Review { get; set; }
        public int ID_Client { get; set; }
        public string ClientName { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Response { get; set; }
        public string Message { get; set; }
    }

    public class ProductReviewsResponseDto
{
    public List<ReviewDto> Reviews { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int Response { get; set; }
    public string Message { get; set; }
}
}