using System;
using System.Collections.Generic;
using System.Text;
using VGTDataStore.Core.Models.Enums;

namespace VGTDataStore.Core
{
    public class PokerGamePlayer
    {
        public Guid PlayerId { get; set; }

        public int SeatPlace { get; set; }

        public PokerPlayerStatus Status { get; set; }

        public UserRolesForPoker UserRole { get; set; }

        public int ChipsForGame { get; set; }

        public int NowChips { get; set; }

        public int Bet { get; set; }

        public PokerGamePlayer(
            Guid userId,
            int place,
            UserRolesForPoker role,
            int chipsForGame,
            int nowChips,
            int bet)
        {
            PlayerId = userId;
            SeatPlace = place;
            Status = PokerPlayerStatus.WaitingForStart;
            UserRole = role;
            ChipsForGame = chipsForGame;
            NowChips = nowChips;
            Bet = bet;
        }
    }
}
