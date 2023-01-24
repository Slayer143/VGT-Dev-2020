using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AnswerModels
{
    public class InGameUsersAnswerModel
    {
        public Guid SessionId { get; set; }

        public Guid UserId { get; set; }

        public int PlayerStatusId { get; set; }

        public int SeatPlace { get; set; }

        public int UserRoleId { get; set; }

        public int StartingChips { get; set; }

        public int NowChips { get; set; }

        public int Bet { get; set; }

        public void Update(InGameUsersAnswerModel model)
        {
            if (this != null)
            {
                PlayerStatusId = model.PlayerStatusId;
                SeatPlace = model.SeatPlace;
                UserRoleId = model.UserRoleId;
                StartingChips = model.StartingChips;
                NowChips = model.NowChips;
                Bet = model.Bet;
            }
        }
    }
}
