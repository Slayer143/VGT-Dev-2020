using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VGTServer.Models
{
    public class InfoAnswerModel
    {
        public string Login { get; set; }

        public int Chips { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public InfoAnswerModel(string login, int chips, string email, int roleId)
        {
            Login = login;
            Chips = chips;
            Email = email;
            RoleId = roleId;
        }
    }
}
