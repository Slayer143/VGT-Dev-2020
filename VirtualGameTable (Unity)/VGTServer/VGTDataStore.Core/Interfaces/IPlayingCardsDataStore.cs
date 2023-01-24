using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VGTDataStore.Core.Models;

namespace VGTDataStore.Core
{
    public interface IPlayingCardsDataStore
    {
        List<PlayingCards> Cards { get; set; }

        Dictionary<Guid, List<PlayingCards>> PlayerCards { get; set; }

        Dictionary<Guid, List<Result>> PlayerResults { get; set; }

        void GetCards();

        void FormCards(List<GameSessionUsers> users);

        Task<List<PlayingCards>> FisherYatesShuffle(List<PlayingCards> cards);

        List<Result> PrepareResults(Dictionary<Guid, List<PlayingCards>> valuesToCalculate);
    }
}
