using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core
{
    public interface IPlayingCardsModel
    {
        int CardId { get; set; }

        int CardValue { get; set; }

        int CardSuitId { get; set; }
    }
}
