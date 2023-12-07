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
        this.ShowDayResult(2, GetWinnings(hands));
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
            HandType at = a.HType(withJoker), bt = b.HType(withJoker);
            if (at == bt)
            {
                var ranking = withJoker ? CardRankJoker : CardRank;
                for (var i = 0; i < 5; i++)
                {
                    if (a.Cards[i] != b.Cards[i])
                        return ranking.IndexOf(a.Cards[i]) < ranking.IndexOf(b.Cards[i]) ? 1 : -1;
                }
                return 0;
            }
            return at < bt ? 1 : -1;
        });
        return hands;
    }

    private const string CardRank = "23456789TJQKA";
    private const string CardRankJoker = "J23456789TQKA";
}

public enum HandType
{
    None, HighCard, OnePair, TwoPair, ThreeOfAKind, FullHouse, FourOfAKind, FiveOfAKind,
}

public record Hand
{
    private HandType _handTypeWithoutJoker;
    private HandType _handTypeWithJoker;
    public string Cards { get; init; }
    public int Bid { get; init; }
    public HandType HType(bool withJoker = false) => withJoker ? _handTypeWithJoker : _handTypeWithoutJoker;

    public Hand(string cards, int bid)
    {
        Cards = cards;
        Bid = bid;
        (_handTypeWithoutJoker, _handTypeWithJoker) = GetHandTypes(cards);
    }

    private (HandType withoutJoker, HandType withJoker) GetHandTypes(string cards)
    {
        var cardGroups = cards.ToCharArray()
            .GroupBy(c => c)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Count());

        var gc = cardGroups.Count();
        HandType resultWithout = gc switch
        {
            1 => HandType.FiveOfAKind,
            2 => cardGroups.Any(g => g.Value == 4 ) ? HandType.FourOfAKind : HandType.FullHouse,
            3 => cardGroups.Any(g => g.Value == 3) ? HandType.ThreeOfAKind : HandType.TwoPair,
            4 => HandType.OnePair,
            5 => HandType.HighCard,
            _ => throw new Exception($"Invalid hand type [{cards}]"),
        };

        // See if we can upgrade the hand if there any 'joker's in the hand
        var resultWith = resultWithout;
        if (cardGroups.ContainsKey('J') && resultWith != HandType.FiveOfAKind)
        {
            var jcount = cardGroups['J'];
            resultWith = resultWith switch
            {
                HandType.FourOfAKind => HandType.FiveOfAKind,  // jcount could be 1 OR 4. Both can convert to 5 of a kind
                HandType.FullHouse => HandType.FiveOfAKind,    // jcount could be 2 or 3. Both can convert to 5 of a kind
                HandType.ThreeOfAKind => HandType.FourOfAKind, // jcount could be 1 or 3. Both yield 4 of a kind (better than full house)
                HandType.TwoPair => jcount == 2 ? HandType.FourOfAKind : HandType.FullHouse,
                HandType.OnePair => HandType.ThreeOfAKind,     // jcount could be 1 or 2. Either way, the best we can get is 3 of a kind
                HandType.HighCard => HandType.OnePair,         // jcount can only be 1
                _ => resultWith,
            };
        }
        return (resultWithout, resultWith);
    }
}
