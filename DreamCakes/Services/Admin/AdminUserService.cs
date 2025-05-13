using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Admin;
using System.Collections.Generic;

namespace DreamCakes.Services.Admin
{
    public class AdminUserService
    {
        private readonly AdminUserRepository _repository = new AdminUserRepository();

        public List<AdminUserDto> GetAllUsers() => _repository.GetAllUsers();
        public AdminUserDto GetUserById(int id) => _repository.GetUserById(id);
        public void UpdateUser(AdminUserDto dto) => _repository.UpdateUser(dto);
        public void DeleteUser(int id) => _repository.DeleteUser(id);
    }
}
