using System;

namespace DreamCakes.Utilities
{
    public class EncryptUtility
    {
        public static string Hash(string contrasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

        public static bool Verify(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }

    }
}
