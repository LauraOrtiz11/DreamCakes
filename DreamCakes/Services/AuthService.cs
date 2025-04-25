using DreamCakes.Dtos;
using DreamCakes.Repositories;
using DreamCakes.Utilities;
using System;

namespace DreamCakes.Services
{
    public class AuthService
    {
        private readonly AuthRepository _authRepository = new AuthRepository();

        public LoginDto Authenticate(LoginDto loginDto)
        {
            try
            {
                var user = _authRepository.GetUserByEmail(loginDto.Email);

                if (user == null)
                {
                    loginDto.Response = 0;
                    loginDto.Message = AuthErrorsUtility.USER_NOT_FOUND;
                    return loginDto;
                }

                if (!EncryptUtility.Verify(loginDto.Contrasena, user.Contrasena))
                {
                    loginDto.Response = 0;
                    loginDto.Message = AuthErrorsUtility.INVALID_CREDENTIALS;
                    return loginDto;
                }

                if (user.ID_Estado != 1) // 1 = Activo
                {
                    loginDto.Response = 0;
                    loginDto.Message = AuthErrorsUtility.INACTIVE_ACCOUNT;
                    return loginDto;
                }

                // Autenticación exitosa
                loginDto.Response = 1;
                loginDto.Message = AuthErrorsUtility.LOGIN_SUCCESS;
                loginDto.RoleId = user.ID_Rol;
                return loginDto;
            }
            catch (Exception ex)
            {
                loginDto.Response = 0;
                loginDto.Message = $"{AuthErrorsUtility.GENERAL_ERROR}: {ex.Message}";
                return loginDto;
            }
        }

        public RegisterDto Register(RegisterDto registerDto)
        {
            try
            {
                if (_authRepository.EmailExists(registerDto.Email))
                {
                    registerDto.Response = 0;
                    registerDto.Message = AuthErrorsUtility.EMAIL_EXISTS;
                    return registerDto;
                }

                registerDto.Contrasena = EncryptUtility.Hash(registerDto.Contrasena);
                registerDto.ID_Estado = 1; // Activo por defecto
                registerDto.ID_Rol = 2; // Cliente por defecto

                var result = _authRepository.CreateUser(registerDto);
                return result;
            }
            catch (Exception ex)
            {
                registerDto.Response = 0;
                registerDto.Message = $"{AuthErrorsUtility.REGISTER_ERROR}: {ex.Message}";
                return registerDto;
            }
        }
    }
}