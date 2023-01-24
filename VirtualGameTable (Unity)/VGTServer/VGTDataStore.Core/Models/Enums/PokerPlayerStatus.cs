using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core.Models.Enums
{
    public enum PokerPlayerStatus
    {
        WaitingForStart = 0,
        ReadyForStart = 1,
        MakesMove = 2,
        WaitingForMove = 3,
        Fall = 4,
        Check = 5, 
        Win = 6,
        Lose = 7
    }
}
