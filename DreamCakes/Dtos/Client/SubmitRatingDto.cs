namespace DreamCakes.Dtos.Client
{
    public class SubmitRatingDto
    {
        public int ProductID { get; set; }
        public int ClientID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}