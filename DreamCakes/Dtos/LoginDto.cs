namespace DreamCakes.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        // Datos de Respuesta
        public int Response { get; set; }
        public string Message { get; set; }
        public int ID_User { get; set; }  
        public int ID_Role { get; set; }      
        public int ID_State { get; set; }   
    }
}