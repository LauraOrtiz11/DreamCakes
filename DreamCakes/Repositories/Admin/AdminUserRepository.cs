using DreamCakes.Repositories.Models;
using DreamCakes.Dtos.Admin;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace DreamCakes.Repositories.Admin
{
    public class AdminUserRepository
    {
        private readonly DreamCakesEntities _context;

        public AdminUserRepository()
        {
            _context = new DreamCakesEntities();
        }

        public List<AdminUserDto> GetAllUsers()
        {
            return _context.USUARIOs.Select(u => new AdminUserDto
            {
                UserID = u.ID_Usuario,
                UserFirstName = u.Nombres,
                UserLastName = u.Apellidos,
                UserPhone = u.Telefono,
                UserAddress = u.Direccion,
                UserEmail = u.Email,
                UserPassword = u.Contrasena,
                UserStatusID = u.ID_Estado,
                UserRoleID = u.ID_Rol
            }).ToList();
        }

        public AdminUserDto GetUserById(int id)
        {
            var user = _context.USUARIOs.Find(id);
            if (user == null) return null;

            return new AdminUserDto
            {
                UserID = user.ID_Usuario,
                UserFirstName = user.Nombres,
                UserLastName = user.Apellidos,
                UserPhone = user.Telefono,
                UserAddress = user.Direccion,
                UserEmail = user.Email,
                UserPassword = user.Contrasena,
                UserStatusID = user.ID_Estado,
                UserRoleID = user.ID_Rol
            };
        }

        public bool UpdateUser(AdminUserDto dto)
        {
            try
            {
                var user = _context.USUARIOs.Find(dto.UserID);
                if (user == null) return false;

                user.Nombres = dto.UserFirstName;
                user.Apellidos = dto.UserLastName;
                user.Telefono = dto.UserPhone;
                user.Direccion = dto.UserAddress;
                user.Email = dto.UserEmail;
                user.ID_Estado = dto.UserStatusID;
                user.ID_Rol = dto.UserRoleID;

                _context.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Debug.WriteLine($"[ERROR] Propiedad: {validationError.PropertyName} - Mensaje: {validationError.ErrorMessage}");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] General: {ex.Message}");
                return false;
            }
        }

        public bool DeleteUser(int id)
        {
            try
            {
                var user = _context.USUARIOs.Find(id);
                if (user == null) return false;

                _context.USUARIOs.Remove(user);
                _context.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Debug.WriteLine($"[ERROR] Propiedad: {validationError.PropertyName} - Mensaje: {validationError.ErrorMessage}");
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] General: {ex.Message}");
                return false;
            }
        }
    }
}
