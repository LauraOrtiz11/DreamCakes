using DreamCakes.Dtos;
using DreamCakes.Repositories;

namespace DreamCakes.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
        }

        // Obtiene el perfil de un usuario a partir de su ID.
        public ProfileDto GetUserProfile(int userId)
        {
            return _userRepository.GetUserProfile(userId);
        }

        // Actualiza la información del perfil de un usuario.
        public bool UpdateUserProfile(ProfileDto profileDto)
        {
            return _userRepository.UpdateUserProfile(profileDto);
        }
    }
}