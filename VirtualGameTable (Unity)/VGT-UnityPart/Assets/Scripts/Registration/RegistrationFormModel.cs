using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace Assets.Scripts.Registration
{
    public class RegistrationFormModel
    {
        private string _login { get; }

        public string Login
        {
            get {return _login;}

            set {Login = _login;}
        }

        private string _password { get; }

        public string Password
        {
            get { return _password; }

            set { Password = _password; }
        }

        private string _passwordRepeat { get; }

        public string PasswordRepeat
        {
            get { return _passwordRepeat; }

            set { PasswordRepeat = _passwordRepeat; }
        }

        private string _email { get; }

        public string Email
        {
            get {return _email;}

            set { Email = _email; }
        }

        public RegistrationFormModel(string login, string password, string passwordRepeat, string email)
        {
            _login = login;
            _password = password;
            _passwordRepeat = passwordRepeat;
            _email = email;
        }

        public bool CheckPass()
        {
            return _password == _passwordRepeat;
        }

        public bool CheckEmail()
        {
            //try
            //{
            //    var email = new System.Net.Mail.MailAddress("VGT-message");
            //    return true;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}


            return Regex.Match(_email, "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", RegexOptions.IgnoreCase).Success;
        }
    }
}
