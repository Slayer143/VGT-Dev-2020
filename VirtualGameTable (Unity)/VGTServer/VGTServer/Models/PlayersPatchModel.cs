using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGTDataStore.Core.Models.Enums;

namespace VGTServer.Models
{
    public class PlayersPatchModel
    {
        public Guid UserId { get; set; }

        public PokerPlayerStatus PlayerStatus { get; set; }

        public int StartingChips { get; set; }

        public int NowChips { get; set; }
    }
}
