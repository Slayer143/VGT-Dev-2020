using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VGTDataStore.Core;

namespace VGTServer.Models
{
    public class VGTUserCreateModel
    {
        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        public VGTUserCreateModel()
        { 
        }

        public VGTUserCreateModel(VGTUserRestricted user)
        {
            Login = user.Login;
            Password = user.Password;
            Email = user.Email;
        }

        public VGTUserRestricted ToVGTUser()
        {
            return new VGTUserRestricted
            {
                Login = Login,
                Email = Email,
                Password = Password
            };
        }
    }
}
