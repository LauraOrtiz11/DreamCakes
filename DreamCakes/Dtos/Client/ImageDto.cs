namespace DreamCakes.Dtos.Client
{
    public class ImageDto
    {
        public int ID_Image { get; set; }
        public int ID_Product { get; set; }
        public string ImgName { get; set; }
        public string ImgUrl { get; set; }
        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
