namespace Shunty.AoC;

// https://adventofcode.com/2023/day/7 - Camel Cards

public class Day07 : AocDaySolver
{
    public int DayNumber => 7;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber);
        var hands = input.Select(ln => new Hand(ln.Split(' ')[0], int.Parse(ln.Split(' ')[1]))).ToList();

        // P1
        SortHands(hands, false);
        this.ShowDayResult(1, GetWinnings(hands));

        // P2
        SortHands(hands, true);
        this.ShowDayResult(1, GetWinnings(hands));
    }

    private int GetWinnings(IList<Hand> hands)
    {
        var hcount = hands.Count;
        return  hands.Select((h,i) => (hcount - i) * h.Bid)
            .Sum();
    }

    private List<Hand> SortHands(List<Hand> hands, bool withJoker)
    {
        hands.Sort((a,b) =>
        {
            // We want a descending sort order, so a < b => +1
            if (a.HandType(withJoker) < b.HandType(withJoker))
                return 1;
            else if (a.HandType(withJoker) > b.HandType(withJoker))
                return -1;
            else
            {
                var ranking = withJoker ? CardRankJoker : CardRank;
                for (var i = 0; i < 5; i++)
                {
                    if (ranking[a.Cards[i]] < ranking[b.Cards[i]])
                        return 1;
                    else if (ranking[a.Cards[i]] > ranking[b.Cards[i]])
                        return -1;
                }
                return 0;
            }
        });
        return hands;
    }

    private readonly Dictionary<char,int> CardRank = new() {
        { 'A', 12 }, { 'K', 11 }, { 'Q', 10 }, { 'J', 9  }, { 'T', 8  }, { '9', 7  },
        { '8', 6  }, { '7', 5  }, { '6', 4  }, { '5', 3  }, { '4', 2  }, { '3', 1  }, { '2', 0  },
    };
    private readonly Dictionary<char,int> CardRankJoker = new() {
        { 'A', 12 }, { 'K', 11 }, { 'Q', 10 }, { 'T', 9  }, { '9', 8  }, { '8', 7  },
        { '7', 6  }, { '6', 5  }, { '5', 4  }, { '4', 3  }, { '3', 2  }, { '2', 1  }, { 'J', 0  },
    };
}

public enum HandType
{
    None, HighCard, OnePair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind,
}

public static class HandTypeExtensions
{
    public static HandType ToHandType(this string cards, bool withJoker = false)
    {
        var cardGroups = cards.ToCharArray()
            .GroupBy(c => c)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Count());

        var gc = cardGroups.Count();
        var result = HandType.None;
        if (gc == 1)
            result = HandType.FiveOfAKind;
        else if (gc == 2)
        {
            if (cardGroups.Any(g => g.Value == 4))
                result = HandType.FourOfAKind;
            else
                result = HandType.FullHouse;
        }
        else if (gc == 4)
            result = HandType.OnePair;
        else if (gc == 5)
        {
            result = HandType.HighCard;
        }
        else // 3 groups
        {
            if (cardGroups.Any(g => g.Value == 3))
                result = HandType.ThreeOfAKind;
            else
                result = HandType.TwoPair;
        }

        // See if we can 'upgrade' the hand
        if (withJoker && cardGroups.ContainsKey('J') && result != HandType.FiveOfAKind)
        {
            var jcount = cardGroups['J'];
            if (result == HandType.FourOfAKind) // jcount could be 1 OR 4. Either way we can convert to 5 of a kind
                result = HandType.FiveOfAKind;
            else if (result == HandType.FullHouse) // jcount could be 2 or 3. Both can convert to 5 of a kind
                result = HandType.FiveOfAKind;
            else if (result == HandType.ThreeOfAKind) // jcount could be 1 or 3 => both yield 4 of a kind (better than full house)
                result = HandType.FourOfAKind;
            else if (result == HandType.TwoPair)
            {
                if (jcount == 1)
                    result = HandType.FullHouse;
                else if (jcount == 2)
                    result = HandType.FourOfAKind;
            }
            else if (result == HandType.OnePair) // jcount can be 1 or 2 = > either way the best we can get is 3 of a kind
                result = HandType.ThreeOfAKind;
            else if (result == HandType.HighCard) // jcount can only be 1
                result = HandType.OnePair;
        }

        if (result == HandType.None)
            throw new Exception($"Invalid hand type [{cards}]");
        return result;
    }
}

public record Hand
{
    private HandType _handType;
    private HandType _handTypeJoker;
    public string Cards { get; init; }
    public int Bid { get; init; }
    public HandType HandType(bool withJoker = false) => withJoker ? _handTypeJoker : _handType;

    public Hand(string cards, int bid)
    {
        Cards = cards;
        Bid = bid;
        _handType = cards.ToHandType();
        _handTypeJoker = cards.ToHandType(true);
    }
}
