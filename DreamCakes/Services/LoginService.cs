using DreamCakes.Dtos;
using DreamCakes.Repositories;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services
{
    public class LoginService
    {
        private readonly LoginRepository loginRepository;

        public LoginService()
        {
            loginRepository = new LoginRepository();
        }

        public LoginDto Login(LoginDto loginDto)
        {
            LoginDto response = new LoginDto();

            try
            {
                var user = loginRepository.GetUserByEmail(loginDto.Email);

                if (user == null)
                {
                    response.Response = 0;
                    response.Message = "El usuario no existe.";
                    return response;
                }

                bool passwordValid = EncryptUtility.VerifyPassword(loginDto.Contrasena, user.Contrasena);

                if (passwordValid)
                {
                    response.Response = 1;
                    response.Message = "Inicio de sesión exitoso.";
                }
                else
                {
                    response.Response = 0;
                    response.Message = "Contraseña incorrecta.";
                }
            }
            catch (Exception ex)
            {
                response.Response = 0;
                response.Message = "Error en el login: " + ex.Message;
            }

            return response;
        }
    }
}
