using System;

namespace DreamCakes.Utilities
{
    public class EncryptUtility
    {
        public static string HashPassword(string contrasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

        public static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }

    }
}
