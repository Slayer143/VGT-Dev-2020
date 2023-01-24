using System;
using System.Collections.Generic;
using System.Text;
using VGTDataStore.Core.Models.Enums;

namespace VGTDataStore.Core.Models
{
    public class GameSessionUsers
    {
        public Guid SessionId { get; set; }

        public Guid UserId { get; set; }

        public PokerPlayerStatus PlayerStatusId { get; set; }

        public int SeatPlace { get; set; }

        public UserRolesForPoker UserRoleId { get; set; }

        public int StartingChips { get; set; }

        public int NowChips { get; set; }

        public int Bet { get; set; }

        public GameSessionUsers()
        {
        }

        public GameSessionUsers(
            Guid sessionId, 
            Guid userId, 
            int place, 
            UserRolesForPoker role, 
            int startingChips,
            int nowChips,
            int bet)
        {
            SessionId = sessionId;
            UserId = userId;
            SeatPlace = place;
            PlayerStatusId = PokerPlayerStatus.WaitingForStart;
            UserRoleId = role;
            StartingChips = startingChips;
            NowChips = nowChips;
            Bet = bet;
        }
    }
}
