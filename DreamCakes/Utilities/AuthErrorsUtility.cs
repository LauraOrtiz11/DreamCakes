using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Utilities
{
    public static class AuthErrorsUtility
    {
 
        // Mensajes de validación
        public const string INVALID_DATA = "Por favor complete todos los campos correctamente";
        public const string GENERAL_ERROR = "Error en el proceso";

        // Mensajes de autenticación
        public const string GENERAL_AUTH_ERROR = "Error al iniciar sesión. Intente nuevamente.";
        public const string USER_NOT_FOUND = "Usuario no encontrado";
        public const string INVALID_CREDENTIALS = "Credenciales incorrectas";
        public const string INACTIVE_ACCOUNT = "Cuenta inactiva";
        public const string LOGIN_SUCCESS = "Inicio de sesión exitoso";

        // Mensajes de registro 
        public const string REGISTER_SUCCESS = "¡Registro exitoso! Por favor inicie sesión.";
        public const string GENERAL_REGISTER_ERROR = "Error al registrar el usuario.";
        public const string EMAIL_EXISTS = "El correo ya está registrado";

        // Errores generales
        public const string GENERAL_SERVER_ERROR = "Error en el servidor. Por favor intente más tarde.";
    }
}