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
                    ID_Usuario = u.ID_Usuario,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Telefono = u.Telefono,
                    Direccion = u.Direccion,
                    Email = u.Email,
                    ID_Rol = u.ID_Rol,
                    
                }).FirstOrDefault();
        }

        // Actualiza el perfil de un usuario en la base de datos.
        public bool UpdateUserProfile(ProfileDto profileDto)
        {
            var user = _context.USUARIOs.Find(profileDto.ID_Usuario);
            if (user == null) return false;

            user.Nombres = profileDto.Nombres;
            user.Apellidos = profileDto.Apellidos;
            user.Telefono = profileDto.Telefono;
            user.Direccion = profileDto.Direccion;
            user.Email = profileDto.Email;

            _context.Entry(user).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }
    }
}