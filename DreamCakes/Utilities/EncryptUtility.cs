using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Utilities
{
    public class EncryptUtility
    {
        public static string HashPassword(string contrasena)
        {
            return BCrypt.Net.BCrypt.HashPassword(contrasena);
        }

    }
}