namespace DreamCakes.Dtos
{
    public class LoginDto
    {
        // Datos de entrada
        public string Email { get; set; }
        public string Contrasena { get; set; }

        // Datos de salida
        public int Response { get; set; } // 1=éxito, 0=error
        public string Message { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }
}