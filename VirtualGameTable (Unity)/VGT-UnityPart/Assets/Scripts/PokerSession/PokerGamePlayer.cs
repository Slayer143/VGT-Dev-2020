using Assets.StaticInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.PokerSession
{
    public class PokerGamePlayer
    {
        public Guid PlayerId { get; set; }

        public int SeatPlace { get; set; }

        public int Status { get; set; }

        public int UserRole { get; set; }

        public int ChipsForGame { get; set; }

        public int NowChips { get; set; }

        public int Bet { get; set; }

        public PokerGamePlayer()
        {
            PlayerId = MainInformation.PlayerInformation.UserId;
            SeatPlace = 1;
            Status = 0;
            UserRole = 0;
            ChipsForGame = GetPlayersChips();
            NowChips = 0;
            Bet = 0;
        }

        public int GetPlayersChips()
        {
            var request = WebRequest.Create($"http://localhost:5000/api/users/info/chips/{MainInformation.PlayerInformation.UserId}");
            var response = request.GetResponse();
            string answer;

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    answer = reader.ReadToEnd();
                }
            }

            return Convert.ToInt32(answer);
        }
    }
}
