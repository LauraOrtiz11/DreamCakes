using DreamCakes.Dtos;
using DreamCakes.Repositories;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services
{
    public class RegisterService
    {
        private readonly RegisterRepository registerRepository;

        public RegisterService()
        {
            registerRepository = new RegisterRepository();
        }

        public RegisterDto CreateUser(RegisterDto user)
        {
            RegisterDto response = new RegisterDto();

            try
            {
                // Encriptar contraseña
                string passwordHash = EncryptUtility.HashPassword(user.Contrasena);
                user.Contrasena = passwordHash;

                user.ID_Estado = 1;
                user.ID_Rol = 2;

                // Registrar en la base de datos
                bool resultRegister = registerRepository.CreateUser(user);

                if (resultRegister)
                {
                    response.Response = 1;
                    response.Message = "Usuario registrado correctamente.";
                }
                else
                {
                    response.Response = 0;
                    response.Message = "No se pudo registrar el usuario.";
                }
            }
            catch (Exception ex)
            {
                response.Response = 0;
                response.Message = "Error al registrar el usuario: " + ex.Message;
            }

            return response;
        }
    }
}
