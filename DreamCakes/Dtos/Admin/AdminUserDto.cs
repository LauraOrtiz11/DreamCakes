using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Admin
{
    public class AdminUserDto
    {
        public int UserID { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhone { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public int UserStatusID { get; set; }
        public int UserRoleID { get; set; }

        // Propiedades calculadas para mostrar en las vistas
        public string UserStatusName
        {
            get
            {
                return UserStatusID == 1 ? "Activo" : "Inactivo";
            }
        }


        public string UserRoleName
        {
            get
            {
                if (UserRoleID == 1) return "Administrador";
                if (UserRoleID == 2) return "Cliente";
                if (UserRoleID == 3) return "Domiciliario";
                return "Desconocido";
            }
        }

    }


}