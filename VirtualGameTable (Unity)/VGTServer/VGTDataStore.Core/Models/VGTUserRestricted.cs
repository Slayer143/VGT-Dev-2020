using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core
{
    public class VGTUserRestricted
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public int Chips { get; set; }

        public int RoleId { get; set; }
    }
}
