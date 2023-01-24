using System;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Scripts.Authorization
{
    public class AuthorizationModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public AuthorizationModel(string login, string password)
        {
            Login = login;
            Password = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
