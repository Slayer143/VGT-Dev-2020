using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core
{
    public interface IVGTUserModel
    {
        string Login { get; set; }

        string Password { get; set; }

        string Email { get; set; }

        int Chips { get; set; }

        int RoleId { get; set; }
    }
}
