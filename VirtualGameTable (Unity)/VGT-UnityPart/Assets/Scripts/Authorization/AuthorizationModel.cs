using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Authorization
{
    public class AuthorizationModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public AuthorizationModel(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
