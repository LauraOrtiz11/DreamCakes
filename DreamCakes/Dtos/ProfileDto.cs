namespace DreamCakes.Dtos
{
    public class ProfileDto
    {
        public int ID_User { get; set; }
        public string UserNames { get; set; }
        public string UserSecNames { get; set; }
        public string PhoneNum { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public int ID_Role { get; set; }
        public string RoleName { get; set; }
        public string ProfileImage { get; set; } 
    }
}