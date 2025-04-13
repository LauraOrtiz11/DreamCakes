using DreamCakes.Dtos;
using DreamCakes.Repositories.Models; 
using System;
using System.Linq;

namespace DreamCakes.Repositories
{
    public class RegisterRepository
    {
        private readonly DreamCakesEntities context;

        public RegisterRepository()
        {
            context = new DreamCakesEntities();
        }

        public bool CreateUser(RegisterDto userDto)
        {
            try
            {
                var user = new USUARIO
                {
                    Nombres = userDto.Nombres,
                    Apellidos = userDto.Apellidos,
                    Telefono = userDto.Telefono,
                    Direccion = userDto.Direccion,
                    Email = userDto.Email,
                    Contrasena = userDto.Contrasena,   
                    ID_Estado = userDto.ID_Estado,
                    ID_Rol = userDto.ID_Rol
                };

                // Agregar el usuario al DbSet USUARIO
                context.USUARIOs.Add(user);

                // Guardar los cambios en la base de datos
                context.SaveChanges();

                return true;  // Registro exitoso
            }
            catch (Exception ex)
            {
                // Manejo de errores 
                Console.WriteLine("Error al registrar usuario: " + ex.Message);
                return false;
            }
        }
    }
}
