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
        private readonly DreamCakesEntities _context = new DreamCakesEntities();

        public USUARIO GetUserByEmail(string email)
        {
            return _context.USUARIOs
                .Include(u => u.ROL)
                .Include(u => u.ESTADO)
                .FirstOrDefault(u => u.Email == email);
        }

        public RegisterDto CreateUser(RegisterDto registerDto)
        {
            try
            {
                var newUser = new USUARIO
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

                _context.USUARIOs.Add(newUser);
                _context.SaveChanges();

                registerDto.Response = 1;
                registerDto.Message = AuthErrorsUtility.REGISTER_SUCCESS;
                registerDto.ID_Usuario = newUser.ID_Usuario;
                return registerDto;
            }
            catch (Exception ex)
            {
                registerDto.Response = 0;
                registerDto.Message = $"{AuthErrorsUtility.GENERAL_REGISTER_ERROR}: {ex.Message}";
                return registerDto;
            }
        }

        public bool EmailExists(string email)
        {
            return _context.USUARIOs.Any(u => u.Email == email);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}