namespace Shunty.AoC;

// https://adventofcode.com/2023/day/4 - Scratchcards

public class Day04 : AocDaySolver
{
    public int DayNumber => 4;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber);

        var cards = new Dictionary<int, CardStats>();
        var p1 = 0;
        foreach (var line in input)
        {
            var cardId = int.Parse(line.Split(':')[0].AsSpan(5).Trim());
            var nums = line.Split(':')[1].Split('|');
            var winningnums = nums[0].Trim().Split().Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => int.Parse(n.Trim()));
            var inhand = nums[1].Split().Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => int.Parse(n.Trim()));
            var wins = winningnums.Intersect(inhand).Count();

            p1 += wins <= 1 ? wins : 1 << (wins - 1);
            cards.Add(cardId, new(cardId, wins));
        }
        this.ShowDayResult(1, p1);

        /// *** Well, first attempt.. This works, but it's very slow. ***
        // var p2 = 0;
        // var maxCardId = cards.Values.Max(c => c.CardId);
        // var cardq = new Queue<CardStats>(cards.Values);
        // while (cardq.Count > 0)
        // {
        //     var card = cardq.Dequeue();
        //     p2++; // We only the need the total number of cards
        //     // Add card copies for further processing
        //     for (var i = 1; i <= card.WinningNums; i++)
        //     {
        //         var cidx = card.CardId + i;
        //         if (cidx > maxCardId)
        //             break;
        //         cardq.Enqueue(cards[cidx]);
        //     }
        // }
        // this.ShowDayResult(2, p2);

        // But... after deliberating over the Python version:
        // This is a waaay faster way of doing it (and would likely be
        // even quicker if we incorporate it into the P1 'for' loop and
        // do it all at once)
        //var maxCardId = cards.Values.Max(c => c.CardId);
        var cardCounts = new Dictionary<int, int>();
        foreach (var card in cards.Values)
        {
            cardCounts.SetOrIncrement(card.CardId, 1);
            foreach (var dx in Enumerable.Range(0, card.WinningNums))
            {
                var cidx = card.CardId + 1 + dx;
                // I believe that due to this section from the problem
                // description "(Cards will never make you copy a card past
                // the end of the table.)" that we don't need to check
                // for cidx > maxCardId
                // Although I didn't realise that originally.
                // if (cidx > maxCardId)
                //     break;
                cardCounts.SetOrIncrement(cidx, cardCounts[card.CardId]);
            }
        }
        var p2 = cardCounts.Values.Sum();
        this.ShowDayResult(2, p2);
    }
}

public record CardStats(int CardId, int WinningNums);
