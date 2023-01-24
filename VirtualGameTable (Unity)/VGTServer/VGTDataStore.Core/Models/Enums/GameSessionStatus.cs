using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core.Models.Enums
{
    public enum GameSessionStatus
    {
        NotStaffed = 0,
        Staffed = 1,
        BetRound = 2,
        FirstRound = 3,
        SecondRound = 4,
        ThirdRound = 5,
        Finished = 6
    }
}
