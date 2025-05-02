namespace DreamCakes.Dtos.Client
{
    public class ProductSearchCriteriaDto
    {
        public int? CategoryID { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string ProductName { get; set; }
        public bool InStock { get; set; } = true;
        public decimal? MinRating { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
    }
}