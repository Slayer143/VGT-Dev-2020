using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.StaticInfo
{
    public class SessionInfo
    {
        public Guid NowSession { get; set; }

        public int SeatPlace { get; set; }

        public int Status { get; set; }

        public int UserRole { get; set; }

        public int ChipsForGame { get; set; }

        public int NowChips { get; set; }

        public SessionInfo(
            Guid sessionId,
            int seatPlace,
            int status,
            int userRole,
            int chipsForGame,
            int nowChips)
        {
            NowSession = sessionId;
            SeatPlace = seatPlace;
            Status = status;
            UserRole = userRole;
            ChipsForGame = chipsForGame;
            NowChips = nowChips;
        }

        public SessionInfo()
        {}
    }
}
