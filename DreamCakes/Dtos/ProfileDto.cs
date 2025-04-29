namespace DreamCakes.Dtos
{
    public class ProfileDto
    {
        public int ID_Usuario { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public int ID_Rol { get; set; }
        public string NombreRol { get; set; }
        public string ImagenPerfil { get; set; } 
    }
}