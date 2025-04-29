namespace DreamCakes.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Contrasena { get; set; }

        // Datos de Respuesta
        public int Response { get; set; }
        public string Message { get; set; }
        public int ID_Usuario { get; set; }  
        public int ID_Rol { get; set; }      
        public int ID_Estado { get; set; }   
    }
}