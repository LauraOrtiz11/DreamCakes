// DreamCakes/Utilities/EmailUtility.cs
using DreamCakes.Repositories.Models;
using System.Linq;

namespace DreamCakes.Utilities
{
    public static class EmailUtility
    {
        public static bool EmailExists(string email)
        {
            using (var context = new DreamCakesEntities())
            {
                return context.USUARIOs.Any(u => u.Email == email);
            }
        }
    }
}