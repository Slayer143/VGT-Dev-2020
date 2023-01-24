using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Registration 
{
    public class RegistrationUserModel
    {
        public string Login { get; }

        public string Password { get; }

        public string Email { get; }

        public RegistrationUserModel(string login, string password, string email)
        {
            Login = login;
            Password = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
            Email = email;
        }
    }
}
