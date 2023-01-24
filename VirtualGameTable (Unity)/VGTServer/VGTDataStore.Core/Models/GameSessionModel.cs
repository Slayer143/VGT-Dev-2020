using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;
using VGTDataStore.Core.Interfaces;
using VGTDataStore.Core.Models.Enums;

namespace VGTDataStore.Core.Models
{
    public class GameSessionModel : IGameSessionModel
    {
        public Guid SessionId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Guid GameId { get; set; }

        public GameSessionStatus SessionStatusId { get; set; }

        public int RoomSize { get; set; }
        public string RoomName { get; set; }
        public string RoomPassword { get; set; }

        public GameSessionModel()
        { 
        }

        public GameSessionModel(Guid gameId, int roomSize, string lobbyName, string lobbyPassword)
        {
            SessionId = Guid.NewGuid();
            StartDate = DateTimeOffset.Now;
            GameId = gameId;
            SessionStatusId = GameSessionStatus.NotStaffed;
            RoomSize = roomSize;
            RoomName = lobbyName;
            RoomPassword = lobbyPassword;
        }
    }
}
