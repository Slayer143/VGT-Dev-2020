using System;
using System.Diagnostics;

namespace VGTDataStore.Core
{
    public class VGTUser
    {
        public Guid UserId { get; set; }
        public string Login { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public int Chips { get; set; }

        public int RoleId { get; set; }

        public VGTUser()
        {
        }

        public VGTUser(
            Guid userId,
            string login,
            string password,
            string email,
            int roleId)
        {
            UserId = userId;
            Login = login;
            Password = password;
            Email = email;
            Chips = 1000;
            RoleId = roleId;
        }

        public VGTUser(VGTUserRestricted user)
        {
            UserId = Guid.NewGuid();
            Login = user.Login;
            Password = user.Password;
            Email = user.Email;
            Chips = user.Chips;
            RoleId = user.RoleId;
        }
    }
}
