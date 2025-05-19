using DreamCakes.Dtos;
using DreamCakes.Repositories.Models;
using DreamCakes.Utilities;
using System;
using System.Data.Entity;
using System.Linq;

namespace DreamCakes.Repositories
{
    public class AuthRepository : IDisposable
    {
        private readonly DreamCakesEntities context = new DreamCakesEntities();


        public AuthRepository()
        {
            context = new DreamCakesEntities();
        }

        //Obtiene un usuario por su email, incluyendo (ROL y ESTADO)
        public USUARIO GetUserByEmail(string email)
        {
            return context.USUARIOs
                .Include(u => u.ROL)
                .Include(u => u.ESTADO)
                .FirstOrDefault(u => u.Email == email);
        }

        //Crear nuevo usuario en base de datos 
        public RegisterDto CreateUser(RegisterDto registerDto)
        {
            try
            {
                var User = new USUARIO
                {
                    Nombres = registerDto.Nombres,
                    Apellidos = registerDto.Apellidos,
                    Telefono = registerDto.Telefono,
                    Direccion = registerDto.Direccion,
                    Email = registerDto.Email,
                    Contrasena = registerDto.Contrasena,
                    ID_Estado = registerDto.ID_Estado,
                    ID_Rol = registerDto.ID_Rol
                };

                context.USUARIOs.Add(User);
                context.SaveChanges();

                registerDto.Response = 1;
                registerDto.Message = AuthErrorsUtility.REGISTER_SUCCESS;
                registerDto.ID_Usuario = User.ID_Usuario;
                return registerDto;
            }
            catch (Exception ex)
            {
                registerDto.Response = 0;
                registerDto.Message = $"{AuthErrorsUtility.GENERAL_REGISTER_ERROR}: {ex.Message}";
                return registerDto;
            }
        }

        // Verifica si un email ya está registrado en la base de datos
        public bool EmailExists(string email)
        {
            return context.USUARIOs.Any(u => u.Email == email);
        }

        // Libera los recursos del contexto de Entity Framework
        public void Dispose()
        {
            context.Dispose();
        }
    }
}