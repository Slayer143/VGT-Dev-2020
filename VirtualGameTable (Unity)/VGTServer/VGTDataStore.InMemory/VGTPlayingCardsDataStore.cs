using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VGTDataStore.Core;
using VGTDataStore.Core.Models;
using VGTDataStore.Core.Models.Enums;

namespace VGTDataStore.InMemory
{
    public class VGTPlayingCardsDataStore : IPlayingCardsDataStore
    {
        private string _connectionString = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=VGTBD;Integrated Security=true;";

        public List<PlayingCards> Cards { get; set; }

        public Dictionary<Guid, List<PlayingCards>> PlayerCards { get; set; }

        public Dictionary<Guid, List<Result>> PlayerResults { get; set; }

        public VGTPlayingCardsDataStore()
        {
            PlayerResults = new Dictionary<Guid, List<Result>>();

            GetCards();
        }

        public async void FormCards(List<GameSessionUsers> users)
        {
            PlayerCards = new Dictionary<Guid, List<PlayingCards>>();

            var cardsCopy = Cards;

            foreach (var user in users)
            {
                if (user.UserRoleId != UserRolesForPoker.Stickman)
                    PlayerCards.Add(user.UserId, RandomCards(cardsCopy, 2));
                if (user.UserRoleId == UserRolesForPoker.Stickman)
                    PlayerCards.Add(user.UserId, RandomCards(cardsCopy, 5));
            }

            PlayerResults.Add(users.First().SessionId, await Task.Run(() => PrepareResults(PlayerCards)));
        }

        private List<PlayingCards> RandomCards(List<PlayingCards> cardsCopy, int iterations)
        {
            var cardsPack = new List<PlayingCards>();

            var rand = new Random();

            int index;

            for (int i = 0; i < iterations; i++)
            {
                do
                {
                    index = rand.Next(0, cardsCopy.Count);

                    if (cardsCopy.Count > index)
                    {
                        cardsPack.Add(cardsCopy[index]);
                        cardsCopy.RemoveAt(index);
                    }

                } while (cardsCopy.Count <= index);
            }

            return cardsPack;
        }

        public async void GetCards()
        {
            await Task.Run(async () =>
            {
                Cards = new List<PlayingCards>();

                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var res = await connection.QueryAsync<PlayingCards>("SELECT * FROM PlayingCards");

                    foreach (var card in res)
                    {
                        Cards.Add(card);
                    }
                }

                Cards = await FisherYatesShuffle(Cards);
            });
        }

        public async Task<List<PlayingCards>> FisherYatesShuffle(List<PlayingCards> cards)
        {
            await Task.Run(() =>
            {
                var rand = new Random();

                for (int i = 0; i < cards.Count - 1; i++)
                {
                    int swapIndex = rand.Next(i, cards.Count);

                    var temp = cards[i];

                    cards[i] = cards[swapIndex];
                    cards[swapIndex] = temp;
                }
            });

            return cards;
        }

        public List<Result> PrepareResults(Dictionary<Guid, List<PlayingCards>> valuesToCalculate)
        {
            var results = new List<Result>();

            var deckCards = valuesToCalculate.FirstOrDefault(x => x.Value.Count > 2).Value;

            foreach (var item in valuesToCalculate)
            {
                if (item.Value.Count != deckCards.Count)
                {
                    var cardPack = item.Value.Union(deckCards).OrderByDescending(x => x.CardValue);

                    byte strength = 0;

                    var arr = new int[10];

                    arr[0] = IsHighCard();
                    arr[1] = IsOnePairCards(new List<PlayingCards>(cardPack));
                    arr[2] = IsTwoPairCards(new List<PlayingCards>(cardPack));
                    arr[3] = IsCardsSet(new List<PlayingCards>(cardPack));
                    arr[4] = IsCardsStraight(new List<int>(cardPack.Select(x => x.CardValue).Distinct()));
                    arr[5] = IsCardsFlush(new List<PlayingCards>(cardPack));
                    arr[6] = IsCardsFullHouse(new List<PlayingCards>(cardPack));
                    arr[7] = IsCardsFourOfAKind(new List<PlayingCards>(cardPack));
                    arr[8] = IsCardsStraightFlush(new List<PlayingCards>(cardPack));
                    arr[9] = IsCardsRoyalFlush(new List<PlayingCards>(cardPack.Where(x => x.CardValue > 9)));

                    var priority = arr.Max();

                    switch (priority)
                    {
                        case 1:
                            {
                                for (var i = 0; i < 5; i++)
                                {
                                    strength += Convert.ToByte(cardPack.ElementAt(i).CardValue);
                                }

                                results.Add(new Result(item.Key, strength, 1));
                            }
                            break;
                        case 2:
                            {
                                var notKicker = new PlayingCards();

                                foreach (var card in cardPack)
                                {
                                    if (cardPack.FirstOrDefault(x => x.CardValue == card.CardValue) != null)
                                    {
                                        notKicker = card;

                                        strength += Convert.ToByte(card.CardValue * 2);
                                    }
                                }

                                for (var i = 0; i < 3; i++)
                                {
                                    if (cardPack.ElementAt(i).CardValue == notKicker.CardValue)
                                        i -= 1;
                                    else
                                        strength += Convert.ToByte(cardPack.ElementAt(i).CardValue);
                                }

                                results.Add(new Result(item.Key, strength, 2));
                            }
                            break;
                        case 3:
                            {
                                var notKickerOne = new PlayingCards();
                                var notKickerTwo = new PlayingCards();

                                foreach (var card in cardPack)
                                {
                                    if (cardPack.FirstOrDefault(x => x.CardValue == card.CardValue) != null)
                                    {
                                        notKickerOne = card;

                                        strength += Convert.ToByte(card.CardValue * 2);
                                    }
                                }

                                foreach (var card in cardPack)
                                {
                                    if (cardPack.FirstOrDefault(x => x.CardValue == card.CardValue
                                        && x.CardValue != notKickerOne.CardValue) != null)
                                    {
                                        notKickerTwo = card;

                                        strength += Convert.ToByte(card.CardValue * 2);
                                    }
                                }

                                for (var i = 0; i < 1; i++)
                                {
                                    if (cardPack.ElementAt(i).CardValue == notKickerOne.CardValue
                                        || cardPack.ElementAt(i).CardValue == notKickerTwo.CardValue)
                                        i -= 1;
                                    else
                                        strength += Convert.ToByte(cardPack.ElementAt(i).CardValue);
                                }

                                results.Add(new Result(item.Key, strength, 3));
                            }
                            break;
                        case 4:
                            {
                                var notKickerOne = new PlayingCards();
                                var notKickerTwo = new PlayingCards();
                                var notKickerThree = new PlayingCards();

                                foreach (var card in cardPack)
                                {
                                    if (cardPack.FirstOrDefault(x => x.CardValue == card.CardValue) != null)
                                    {
                                        notKickerOne = cardPack.FirstOrDefault(x => x.CardValue == card.CardValue);
                                        notKickerTwo = card;
                                        strength += Convert.ToByte(card.CardValue * 2);

                                        if (cardPack.FirstOrDefault(x => x.CardValue == card.CardValue
                                            && x.CardId != notKickerOne.CardId
                                            && x.CardId != notKickerTwo.CardId) != null)
                                        {
                                            notKickerThree = cardPack.FirstOrDefault(x => x.CardValue == card.CardValue
                                            && x.CardId == notKickerOne.CardId
                                            && x.CardId == notKickerTwo.CardId);

                                            strength += Convert.ToByte(notKickerThree.CardValue);
                                        }
                                        else
                                        {
                                            if (notKickerThree == null)
                                            {
                                                notKickerOne = null;
                                                notKickerTwo = null;
                                                notKickerThree = null;
                                                strength = 0;
                                            }
                                        }
                                    }
                                }

                                for (var i = 0; i < 2; i++)
                                {
                                    if (cardPack.ElementAt(i).CardId == notKickerOne.CardId
                                        || cardPack.ElementAt(i).CardId == notKickerTwo.CardId
                                        || cardPack.ElementAt(i).CardId == notKickerThree.CardId)
                                        i--;
                                    else
                                        strength += Convert.ToByte(cardPack.ElementAt(i).CardValue);
                                }

                                results.Add(new Result(item.Key, strength, 4));
                            }
                            break;
                        case 5:
                            {
                                int counter = 0;

                                var straightCardPack = cardPack.Select(x => x.CardValue).Distinct();

                                if (straightCardPack.ElementAt(6) == 2
                                    && straightCardPack.ElementAt(5) == 3
                                    && straightCardPack.ElementAt(4) == 4
                                    && straightCardPack.ElementAt(3) == 5
                                    && straightCardPack.ElementAt(0) == 14
                                    && straightCardPack.ElementAt(2) != 6)
                                {
                                    strength += 2 + 3 + 4 + 5 + 1;
                                    results.Add(new Result(item.Key, strength, 5));
                                }
                                else
                                {
                                    for (int i = 0; i < straightCardPack.Count(); i++)
                                    {
                                        if (i > 0)
                                        {
                                            if (straightCardPack.ElementAt(i - 1) - straightCardPack.ElementAt(i) == 1
                                                && counter < 5)
                                            {
                                                counter++;

                                                strength += Convert.ToByte(straightCardPack.ElementAt(i));
                                            }
                                        }
                                        else
                                        {
                                            counter++;

                                            strength += Convert.ToByte(straightCardPack.ElementAt(i));
                                        }
                                    }

                                    results.Add(new Result(item.Key, strength, 5));
                                }
                            }
                            break;
                        case 6:
                            {
                                var cardsPackSortedForFlush = cardPack.OrderByDescending(x => x.CardSuitId).ThenByDescending(x => x.CardValue);

                                var diamondsFlush = cardsPackSortedForFlush.Where(x => x.CardSuitId == 1);
                                var heartsFlush = cardsPackSortedForFlush.Where(x => x.CardSuitId == 1);
                                var spadesFlush = cardsPackSortedForFlush.Where(x => x.CardSuitId == 1);
                                var clubsFlush = cardsPackSortedForFlush.Where(x => x.CardSuitId == 1);

                                if (diamondsFlush.Count() >= 5)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        strength += Convert.ToByte(diamondsFlush.ElementAt(i).CardValue);
                                    }
                                }

                                else if (heartsFlush.Count() >= 5)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        strength += Convert.ToByte(heartsFlush.ElementAt(i).CardValue);
                                    }
                                }

                                else if (spadesFlush.Count() >= 5)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        strength += Convert.ToByte(spadesFlush.ElementAt(i).CardValue);
                                    }
                                }

                                else if (clubsFlush.Count() >= 5)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        strength += Convert.ToByte(clubsFlush.ElementAt(i).CardValue);
                                    }
                                }

                                results.Add(new Result(item.Key, strength, 6));
                            }
                            break;
                        case 7:
                            {
                                var cardsSet = new List<PlayingCards>();

                                foreach (var card in cardPack)
                                {
                                    if (cardPack.Where(x => x.CardValue == card.CardValue).Count() == 3)
                                    {
                                        if (cardsSet.Count == 0)
                                        {
                                            cardsSet = cardPack.Where(x => x.CardValue == card.CardValue).ToList();
                                        }
                                        else
                                        {
                                            if (cardPack.Where(x => x.CardValue == card.CardValue).ToList()[0].CardValue > cardsSet[0].CardValue)
                                            {
                                                cardsSet = cardPack.Where(x => x.CardValue == card.CardValue).ToList();
                                            }
                                        }
                                    }
                                }

                                var cardsPair = new List<PlayingCards>();

                                foreach (var card in cardPack)
                                {
                                    if (cardPack.Where(x => x.CardValue == card.CardValue
                                    && x.CardValue != cardsSet[0].CardValue).Count() == 2)
                                    {
                                        if (cardsPair.Count == 0)
                                        {
                                            cardsPair = cardPack
                                                .Where(x => x.CardValue == card.CardValue
                                                && x.CardValue != cardsSet[0].CardValue)
                                                .ToList();
                                        }
                                        else
                                        {
                                            if (cardPack.Where(x => x.CardValue == card.CardValue).ToList()[0].CardValue > cardsPair[0].CardValue)
                                            {
                                                cardsPair = cardPack.Where(x => x.CardValue == card.CardValue).ToList();
                                            }
                                        }
                                    }
                                }

                                strength = Convert.ToByte(cardsSet.Sum(x => x.CardValue) + cardsPair.Sum(x => x.CardValue));

                                results.Add(new Result(item.Key, strength, 7));
                            }
                            break;
                        case 8:
                            {
                                var fourOfAKind = new List<PlayingCards>();

                                foreach (var card in cardPack)
                                {
                                    if (cardPack.Where(x => x.CardValue == card.CardValue).Count() == 4)
                                        fourOfAKind = cardPack.Where(x => x.CardValue == card.CardValue).ToList();
                                }

                                PlayingCards kicker = null;

                                foreach (var card in cardPack)
                                {
                                    if (card.CardValue != fourOfAKind[0].CardValue)
                                    {
                                        if (kicker == null)
                                            kicker = card;
                                        else
                                        {
                                            if (kicker.CardValue < card.CardValue)
                                                kicker = card;
                                        }
                                    }
                                }

                                strength = Convert.ToByte(fourOfAKind.Sum(x => x.CardValue) + kicker.CardValue);

                                results.Add(new Result(item.Key, strength, 8));
                            }
                            break;
                        case 9:
                            {
                                var diamondsCards = cardPack.Where(x => x.CardSuitId == 1).OrderByDescending(x => x.CardValue).ToList();
                                var heartsCards = cardPack.Where(x => x.CardSuitId == 2).OrderByDescending(x => x.CardValue).ToList();
                                var spadesCards = cardPack.Where(x => x.CardSuitId == 3).OrderByDescending(x => x.CardValue).ToList();
                                var clubsCards = cardPack.Where(x => x.CardSuitId == 4).OrderByDescending(x => x.CardValue).ToList();

                                var straightFlushCards = new List<PlayingCards>();

                                if (diamondsCards.Count >= 5)
                                {
                                    if (IsLowerStraight(diamondsCards))
                                        results.Add(new Result(item.Key, (1 + 2 + 3 + 4 + 5), 9));
                                    else
                                        straightFlushCards = FindSraightFlushCards(diamondsCards);
                                }

                                else if (heartsCards.Count >= 5)
                                {
                                    if (IsLowerStraight(heartsCards))
                                        results.Add(new Result(item.Key, (1 + 2 + 3 + 4 + 5), 9));
                                    else
                                        straightFlushCards = FindSraightFlushCards(heartsCards);
                                }

                                else if (spadesCards.Count >= 5)
                                {
                                    if (IsLowerStraight(spadesCards))
                                        results.Add(new Result(item.Key, (1 + 2 + 3 + 4 + 5), 9));
                                    else
                                        straightFlushCards = FindSraightFlushCards(spadesCards);
                                }

                                else if (clubsCards.Count >= 5)
                                {
                                    if (IsLowerStraight(clubsCards))
                                        results.Add(new Result(item.Key, (1 + 2 + 3 + 4 + 5), 9));
                                    else
                                        straightFlushCards = FindSraightFlushCards(clubsCards);
                                }

                                strength = Convert.ToByte(straightFlushCards.Sum(x => x.CardValue));

                                results.Add(new Result(item.Key, strength, 9));
                            }
                            break;
                        case 10:
                            {
                                results.Add(new Result(item.Key, byte.MaxValue, 10));
                            }
                            break;
                    }
                }
            }

            return results;
        }

        private bool IsLowerStraight(List<PlayingCards> playingCards)
        {
            return playingCards.FirstOrDefault(x => x.CardValue == 2) != null
                                        && playingCards.FirstOrDefault(x => x.CardValue == 3) != null
                                        && playingCards.FirstOrDefault(x => x.CardValue == 4) != null
                                        && playingCards.FirstOrDefault(x => x.CardValue == 5) != null
                                        && playingCards.FirstOrDefault(x => x.CardValue == 14) != null
                                        && playingCards.FirstOrDefault(x => x.CardValue == 6) == null;
        }

        private List<PlayingCards> FindSraightFlushCards(List<PlayingCards> playingCards)
        {
            var previous = new PlayingCards();
            var result = new List<PlayingCards>();

            for (int i = 0; i < playingCards.Count(); i++)
            {
                if (i > 0)
                {
                    if (previous.CardValue - playingCards.ElementAt(i).CardValue == 1
                        && playingCards.ElementAt(i).CardValue <= 10)
                    {
                        previous = playingCards.ElementAt(i);
                        result.Add(previous);
                    }
                    else if (previous.CardValue - playingCards.ElementAt(i).CardValue != 1
                        && playingCards.ElementAt(i).CardValue <= 10)
                    {
                        previous = playingCards.ElementAt(i);
                        result.Clear();
                        result.Add(previous);
                    }
                }
                else
                {
                    previous = playingCards.ElementAt(i);
                    result.Add(previous);
                }
            }

            return result;
        }

        private int IsHighCard()
        {
            return 1;
        }

        private int IsOnePairCards(List<PlayingCards> unitedCards)
        {
            bool hasPairsOnDeck = false;

            foreach (var item in unitedCards)
            {
                if (unitedCards.FirstOrDefault(x => x.CardValue == item.CardValue) != null)
                    hasPairsOnDeck = true;
            }

            return hasPairsOnDeck ? 2 : 0;
        }

        private int IsTwoPairCards(List<PlayingCards> unitedCards)
        {
            bool hasFirstPairOnDeck = false;

            var firstPairCard = new PlayingCards();

            foreach (var item in unitedCards)
            {
                if (unitedCards.FirstOrDefault(x => x.CardValue == item.CardValue) != null)
                {
                    hasFirstPairOnDeck = true;
                    firstPairCard = item;
                }
            }

            bool hasSecondPairOnDeck = false;

            foreach (var item in unitedCards)
            {
                if (unitedCards.FirstOrDefault(x => x.CardValue == item.CardValue && x.CardValue != firstPairCard.CardValue) != null)
                    hasSecondPairOnDeck = true;
            }

            return hasFirstPairOnDeck && hasSecondPairOnDeck ? 3 : 0;
        }

        private int IsCardsSet(List<PlayingCards> unitedCards)
        {
            bool isCardSet = false;

            foreach (var item in unitedCards)
            {
                if (unitedCards.Where(x => x.CardValue == item.CardValue).Count() == 3)
                    isCardSet = true;
            }

            return isCardSet ? 4 : 0;
        }

        private int IsCardsStraight(List<int> unitedCardsValues)
        {
            var isCardsStraight = 0;

            if (unitedCardsValues.Contains(2)
                   && unitedCardsValues.Contains(3)
                   && unitedCardsValues.Contains(4)
                   && unitedCardsValues.Contains(5)
                   && unitedCardsValues.Contains(14)
                   && !unitedCardsValues.Contains(6))
                isCardsStraight = 5;
            else
            {
                var previous = 0;

                for (int i = 0; i < unitedCardsValues.Count(); i++)
                {
                    if (i > 0)
                    {
                        if (previous - unitedCardsValues.ElementAt(i) == 1)
                        {
                            previous = unitedCardsValues.ElementAt(i);
                            isCardsStraight++;
                        }
                        else if (previous - unitedCardsValues.ElementAt(i) != 1)
                        {
                            previous = unitedCardsValues.ElementAt(i);
                            isCardsStraight--;
                        }
                    }
                    else
                    {
                        isCardsStraight++;
                        previous = unitedCardsValues.ElementAt(i);
                    }
                }
            }

            return isCardsStraight == 5 ? 5 : 0;
        }

        private int IsCardsFlush(List<PlayingCards> unitedCards)
        {
            return unitedCards.Where(x => x.CardSuitId == 1).Count() >= 5
                || unitedCards.Where(x => x.CardSuitId == 2).Count() >= 5
                || unitedCards.Where(x => x.CardSuitId == 3).Count() >= 5
                || unitedCards.Where(x => x.CardSuitId == 4).Count() >= 5
                ? 6
                : 0;
        }

        private int IsCardsFullHouse(List<PlayingCards> unitedCards)
        {
            bool isCardsSet = false;

            var setCard = new PlayingCards();

            foreach (var item in unitedCards)
            {
                if (unitedCards.Where(x => x.CardValue == item.CardValue).Count() == 3)
                {
                    setCard = item;
                    isCardsSet = true;
                }
            }

            bool isCardsPair = false;

            foreach (var item in unitedCards)
            {
                if (unitedCards.FirstOrDefault(x => x.CardValue == item.CardValue && x.CardValue != setCard.CardValue) != null)
                    isCardsPair = true;
            }

            return isCardsSet && isCardsPair ? 7 : 0;
        }

        private int IsCardsFourOfAKind(List<PlayingCards> unitedCards)
        {
            var isCardsFourOfAKind = false;

            foreach (var item in unitedCards)
            {
                if (unitedCards.Where(x => x.CardValue == item.CardValue).Count() == 4)
                    isCardsFourOfAKind = true;
            }

            return isCardsFourOfAKind ? 8 : 0;
        }

        private int IsCardsStraightFlush(List<PlayingCards> unitedCards)
        {
            var isCardsStraightFlush = 0;

            var diamondsCards = unitedCards.Where(x => x.CardSuitId == 1).OrderByDescending(x => x.CardValue);
            var heartsCards = unitedCards.Where(x => x.CardSuitId == 2).OrderByDescending(x => x.CardValue);
            var spadesCards = unitedCards.Where(x => x.CardSuitId == 3).OrderByDescending(x => x.CardValue);
            var clubsCards = unitedCards.Where(x => x.CardSuitId == 4).OrderByDescending(x => x.CardValue);


            if (diamondsCards.Count() >= 5)
            {
                if (IsLowerStraight(new List<PlayingCards>(diamondsCards)))
                    isCardsStraightFlush = 5;
                else
                    isCardsStraightFlush = StraightFlushCalculator(diamondsCards);
            }

            if (heartsCards.Count() >= 5)
            {
                if (IsLowerStraight(new List<PlayingCards>(heartsCards)))
                    isCardsStraightFlush = 5;
                else
                    isCardsStraightFlush = StraightFlushCalculator(heartsCards);
            }

            if (spadesCards.Count() >= 5)
            {
                if (IsLowerStraight(new List<PlayingCards>(spadesCards)))
                    isCardsStraightFlush = 5;
                else
                    isCardsStraightFlush = StraightFlushCalculator(spadesCards);
            }

            if (clubsCards.Count() >= 5)
            {
                if (IsLowerStraight(new List<PlayingCards>(clubsCards)))
                    isCardsStraightFlush = 5;
                else
                    isCardsStraightFlush = StraightFlushCalculator(clubsCards);
            }

            return isCardsStraightFlush == 5 ? 9 : 0;
        }

        private int IsCardsRoyalFlush(List<PlayingCards> unitedCards)
        {
            return unitedCards.Where(x => x.CardSuitId == 1).Count() == 5
                || unitedCards.Where(x => x.CardSuitId == 2).Count() == 5
                || unitedCards.Where(x => x.CardSuitId == 3).Count() == 5
                || unitedCards.Where(x => x.CardSuitId == 4).Count() == 5
                ? 10
                : 0;
        }

        private int StraightFlushCalculator(IOrderedEnumerable<PlayingCards> playingCards)
        {
            var previous = new PlayingCards();
            var result = 0;

            for (int i = 0; i < playingCards.Count(); i++)
            {
                if (i > 0)
                {
                    if (previous.CardValue - playingCards.ElementAt(i).CardValue == 1
                        && playingCards.ElementAt(i).CardValue <= 10)
                    {
                        previous = playingCards.ElementAt(i);
                        result++;
                    }
                    else if (previous.CardValue - playingCards.ElementAt(i).CardValue != 1
                        && playingCards.ElementAt(i).CardValue <= 10)
                    {
                        previous = playingCards.ElementAt(i);
                        result--;
                    }
                }
                else
                {
                    result++;
                    previous = playingCards.ElementAt(i);
                }
            }

            return result;
        }
    }
}
