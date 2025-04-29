using DreamCakes.Dtos;
using DreamCakes.Repositories;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services
{
    public class AuthService
    {
        private readonly AuthRepository authRepository = new AuthRepository();

        // Autentica a un usuario mediante su correo electrónico y contraseña.
        public LoginDto Authenticate(LoginDto loginDto)
        {
            try
            {
                var user = authRepository.GetUserByEmail(loginDto.Email);

                // Validar que existe el usuario 
                if (user == null)
                {
                    loginDto.Response = 0;
                    loginDto.Message = AuthErrorsUtility.USER_NOT_FOUND;
                    return loginDto;
                }

                // Validar contraseña 
                if (!EncryptUtility.Verify(loginDto.Contrasena, user.Contrasena))
                {
                    loginDto.Response = 0;
                    loginDto.Message = AuthErrorsUtility.INVALID_CREDENTIALS;
                    return loginDto;
                }
                 // Validar si esta activo o no el usuario 
                if (user.ID_Estado != 1) // 1 = Activo
                {
                    loginDto.Response = 0;
                    loginDto.Message = AuthErrorsUtility.INACTIVE_ACCOUNT;
                    return loginDto;
                }

                
                loginDto.Response = 1;
                loginDto.Message = "Autenticación exitosa";
                loginDto.ID_Usuario = user.ID_Usuario;
                loginDto.ID_Rol = user.ID_Rol;
                loginDto.ID_Estado = user.ID_Estado;

                return loginDto;
            }
            catch (Exception ex)
            {
                loginDto.Response = 0;
                loginDto.Message = $"{AuthErrorsUtility.GENERAL_ERROR}: {ex.Message}";
                return loginDto;
            }
        }

        // Registra un nuevo usuario si el correo no está en uso.
        public RegisterDto Register(RegisterDto registerDto)
        {
            try
            {
                //Verificar si ya existe una cuenta con el mismo correo 
                if (authRepository.EmailExists(registerDto.Email))
                {
                    registerDto.Response = 0;
                    registerDto.Message = AuthErrorsUtility.EMAIL_EXISTS;
                    return registerDto;
                }

                //Encriptar la contraseña
                registerDto.Contrasena = EncryptUtility.Hash(registerDto.Contrasena);
                registerDto.ID_Estado = 1; // Activo por defecto
                registerDto.ID_Rol = 2; // Cliente por defecto

                // Creación del Usuario 
                var result = authRepository.CreateUser(registerDto);
                return result;
            }
            catch (Exception ex)
            {
                registerDto.Response = 0;
                registerDto.Message = $"{AuthErrorsUtility.GENERAL_REGISTER_ERROR}: {ex.Message}";
                return registerDto;
            }
        }
    }
}