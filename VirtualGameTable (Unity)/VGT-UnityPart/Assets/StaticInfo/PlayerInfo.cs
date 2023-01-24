using Assets.AnswerModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.StaticInfo
{
    public class PlayerInfo
    {
        public Guid UserId { get; set; }

        public string Login { get; set; }

        public int Chips { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public PlayerInfo(Guid userId)
        {
            UserId = userId;

            var info = GetInfo(userId);

            Login = info.Login;

            Chips = info.Chips;

            Email = info.Email;

            RoleId = info.RoleId;
        }

        public PlayerInfo()
        {}

        private InfoAnswerModel GetInfo(Guid userId)
        {
            var request = WebRequest.Create($"http://localhost:5000/api/users/info/{userId}");
            var response = request.GetResponse();
            string answer;

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    answer = reader.ReadToEnd();
                }
            }

            return JsonConvert.DeserializeObject<InfoAnswerModel>(answer);
        }
    }
}
