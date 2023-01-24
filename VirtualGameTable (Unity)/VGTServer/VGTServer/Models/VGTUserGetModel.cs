using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGTDataStore.Core;

namespace VGTServer.Models
{
    public class VGTUserGetModel : IVGTUserModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public int Chips { get; set; }

        public int RoleId { get; set; }

        public VGTUserGetModel(VGTUser user)
        {
            Login = user.Login;
            Password = user.Password;
            Email = user.Email;
            Chips = user.Chips;
            RoleId = user.RoleId;
        }
    }
}
