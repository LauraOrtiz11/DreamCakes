using DreamCakes.Repositories.Models;
using System.Linq;

namespace DreamCakes.Repositories
{
    public class LoginRepository
    {
        private readonly DreamCakesEntities _context;

        public LoginRepository()
        {
            _context = new DreamCakesEntities();
        }

        public USUARIO GetUserByEmail(string email)
        {
            return _context.USUARIOs.FirstOrDefault(u => u.Email == email);
        }
    }
}
