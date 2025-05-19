using System.Collections.Generic;

namespace DreamCakes.Dtos.Client
{
    public class ProductDto
    {
        public int ID_Product { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public string MainImageUrl { get; set; }

        public int Stock { get; set; }
        public decimal AvgRating { get; set; }
        public int ID_Category { get; set; }
        public CategoryDto Category { get; set; }
        public string CategoryName { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        public virtual List<ReviewDto> Reviews { get; set; }
        public virtual bool CanReview { get; set; }
        public int TotalReviews { get; set; }
        public int Response { get; set; }
        public string Message { get; set; }


    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string AltText { get; set; }
    }
}