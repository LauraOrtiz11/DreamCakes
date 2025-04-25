using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos
{
    public class RegisterDto
    {
        public int? ID_Usuario { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;

        public int ID_Estado { get; set; }
        public int ID_Rol { get; set; }

        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}