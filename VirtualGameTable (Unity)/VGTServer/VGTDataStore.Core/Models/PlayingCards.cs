using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core
{
    public class PlayingCards : IPlayingCardsModel
    {
        public int CardId { get ; set ; }

        public int CardValue { get ; set ; }

        public int CardSuitId { get ; set ; }

    }
}
