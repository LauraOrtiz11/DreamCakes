using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos
{
    public class RegisterDto
    {
        public int ID_User { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserSName { get; set; } = string.Empty;
        public string PhoneNumb { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public int ID_State { get; set; }
        public int ID_Role { get; set; }

        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}