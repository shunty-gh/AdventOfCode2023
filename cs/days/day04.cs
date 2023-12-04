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
            var wins = winningnums.Intersect(inhand).ToList();

            var score = wins.Count <= 1 ? wins.Count : 1 << (wins.Count - 1);
            p1 += score;
            cards.Add(cardId, new(cardId, wins.Count));
        }
        this.ShowDayResult(1, p1);

        var p2 = 0;
        var maxCardId = cards.Values.Max(c => c.CardId);
        var cardq = new Queue<CardStats>(cards.Values);
        while (cardq.Count > 0)
        {
            var card = cardq.Dequeue();
            p2++; // We only the need the total number of cards
            // Add card copies for further processing
            for (var i = 1; i <= card.WinningNums; i++)
            {
                var cidx = card.CardId + i;
                if (cidx > maxCardId)
                    break;
                cardq.Enqueue(cards[cidx]);
            }
        }
        this.ShowDayResult(2, p2);
    }
}

public record CardStats(int CardId, int WinningNums);
