namespace DreamCakes.Dtos.Client
{
    public class ProductSearchResultDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public decimal ProductRating { get; set; }
        public int RatingCount { get; set; }
        public string CategoryName { get; set; }
        public string MainImageUrl { get; set; }
    }
}