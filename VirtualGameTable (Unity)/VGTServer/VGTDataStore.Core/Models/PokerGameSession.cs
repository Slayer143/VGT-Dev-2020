using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using VGTDataStore.Core.Models.Enums;

namespace VGTDataStore.Core.Models
{
    public class PokerGameSession : GameSessionModel
    {
        public List<PokerGamePlayer> PokerGamePlayers { get; set; }

        public List<PlayingCards> CardsOnTheTable { get; set; }

        public PokerGameSession()
        {
        }

        public PokerGameSession(int roomSize, string roomName, string roomPassword, PokerGamePlayer player)
            :base(new Guid("ea255d1d-51e0-43d3-b985-6e2bb779879f"), roomSize, roomName, roomPassword)
        {
            PokerGamePlayers = new List<PokerGamePlayer>();
            PokerGamePlayers.Add(player);
        }

        //public FoolGameSession(FoolGamePlayer player, int roomSize)
        //    : base(new Guid("ea255d1d-51e0-43d3-b985-6e2bb779879f"), roomSize)
        //{
        //    FoolGamePlayers = new List<FoolGamePlayer>();
        //    FoolGamePlayers.Add(player);
        //}

        //public bool AddPlayer(FoolGamePlayer player)
        //{
        //    if (FoolGamePlayers.Count < base.RoomSize)
        //    {
        //        FoolGamePlayers.Add(player);

        //        return true;
        //    }

        //    return false;
        //}

        //public bool RemovePlayer(Guid userId)
        //{
        //    foreach (var player in FoolGamePlayers)
        //    {
        //        if(player.PlayerId == userId)
        //        {
        //            FoolGamePlayers.Remove(player);

        //            return true;
        //        }
        //    }

        //    return false;
        //}
    }
}
