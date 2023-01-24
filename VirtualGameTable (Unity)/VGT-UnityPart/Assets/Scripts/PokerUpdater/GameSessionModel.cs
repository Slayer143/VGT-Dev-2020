using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.PokerUpdater
{
    public class GameSessionModel
    {
        public List<object> PokerGamePlayers { get; set; }

        public List<object> CardsOnTheTable { get; set; }

        public Guid SessionId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid GameId { get; set; }

        public int SessionStatusId { get; set; }

        public int RoomSize { get; set; }

        public string RoomName { get; set; }

        public string RoomPassword { get; set; }
    }
}
