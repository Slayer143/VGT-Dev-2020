using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGTDataStore.Core;

namespace VGTServer.Answers
{
    public class PokerGameSessionAnswer
    {
        public PokerGamePlayer Player { get; set; }

        public Guid GameSessionId { get; set; }

        public Dictionary<Guid, List<PlayingCards>> UsersCards { get; set; }

        public string Message { get; set; }

        public PokerGameSessionAnswer(
            PokerGamePlayer player,
            Guid sessionId,
            string message,
            Dictionary<Guid, List<PlayingCards>> cards = null)
        {
            Player = player;
            GameSessionId = sessionId;
            Message = message;
            UsersCards = cards;
        }
    }
}
