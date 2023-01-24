using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core.Interfaces
{
    public interface IGameSessionModel
    {
        Guid SessionId { get; set; }

        int RoomSize { get; set; }
        string RoomName { get; set; }
        string RoomPassword { get; set; }

        DateTimeOffset StartDate { get; set; }

        DateTimeOffset EndDate { get; set; }

        Guid GameId { get; set; }
    }
}
