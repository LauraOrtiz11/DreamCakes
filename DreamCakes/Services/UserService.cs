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

        public ProfileDto GetUserProfile(int userId)
        {
            return _userRepository.GetUserProfile(userId);
        }

        public bool UpdateUserProfile(ProfileDto profileDto)
        {
            return _userRepository.UpdateUserProfile(profileDto);
        }
    }
}