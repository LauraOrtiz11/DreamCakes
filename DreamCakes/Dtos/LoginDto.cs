namespace DreamCakes.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Contrasena { get; set; }

        // Respuesta
        public int Response { get; set; }
        public string Message { get; set; }
    }
}
