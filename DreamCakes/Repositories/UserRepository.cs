using DreamCakes.Dtos;
using DreamCakes.Repositories.Models;
using System.Data.Entity;
using System.Linq;

namespace DreamCakes.Repositories
{
    public class UserRepository
    {
        private readonly DreamCakesEntities _context;

        public UserRepository()
        {
            _context = new DreamCakesEntities();
        }

        // Obtiene el perfil de un usuario por su ID.
        public ProfileDto GetUserProfile(int userId)
        {
            return _context.USUARIOs
                .Where(u => u.ID_Usuario == userId)
                .Select(u => new ProfileDto
                {
                    ID_User = u.ID_Usuario,
                    UserNames = u.Nombres,
                    UserSecNames = u.Apellidos,
                    PhoneNum = u.Telefono,
                    Address = u.Direccion,
                    Email = u.Email,
                    ID_Role = u.ID_Rol,
                    
                }).FirstOrDefault();
        }

        // Actualiza el perfil de un usuario en la base de datos.
        public bool UpdateUserProfile(ProfileDto profileDto)
        {
            var user = _context.USUARIOs.Find(profileDto.ID_User);
            if (user == null) return false;

            user.Nombres = profileDto.UserNames;
            user.Apellidos = profileDto.UserSecNames;
            user.Telefono = profileDto.PhoneNum;
            user.Direccion = profileDto.Address;
            user.Email = profileDto.Email;

            _context.Entry(user).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }
    }
}