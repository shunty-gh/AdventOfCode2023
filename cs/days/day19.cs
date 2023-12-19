namespace Shunty.AoC;

// https://adventofcode.com/2023/day/19 - Aplenty

public class Day19 : AocDaySolver
{
    public int DayNumber => 19;

    public async Task Solve()
    {
        //var input = (await AocUtils.GetDayLines(DayNumber, "test")).ToList();
        var input = (await AocUtils.GetDayLines(DayNumber)).ToList();
        var idx = input.ToList().IndexOf(string.Empty);
        var workflow = BuildWorkflow(input[..idx]);
        var partlist = BuildPartList(input[(idx+1)..]);

        var p1 = partlist.Aggregate(0, (acc, mp) => AcceptPart(workflow, mp) ? acc + mp.Rating : acc);
        this.ShowDayResult(1, p1);

        var p2 = SolvePart2(workflow);
        this.ShowDayResult(2, p2);
    }

    private Int64 SolvePart2(Dictionary<string, Workflow> wfd)
    {
        /* Start with a maximum range of 1..4000 for each category. Start at "in"
           and as we process each workflow we whittle down the category ranges
           that will and won't pass through each rule. Then add the adjusted
           ranges to the queue for further workflow processing.
           At the end we should have a list of acceptable range combinations.
         */

        var accepted = new List<RatingsRange>();
        var q = new Queue<(RatingsRange, string)>();
        q.Enqueue((new RatingsRange(), "in"));
        while (q.Count > 0)
        {
            var (rng, wfid) = q.Dequeue();
            if (wfid == "A" || wfid == "R")
            {
                if (wfid == "A")
                    accepted.Add(rng);
                continue;
            }

            var wf = wfd[wfid];
            RatingsRange nxt = new(rng);
            foreach (var rule in wf.Rules)
            {
                var op = rule.Op;
                switch (op)
                {
                    case RuleOperator.Accept:
                        accepted.Add(nxt);
                        break;
                    case RuleOperator.Reject:
                        break;
                    case RuleOperator.Goto:
                        q.Enqueue((nxt, rule.Next));
                        break;
                    case RuleOperator.lt:
                    case RuleOperator.gt:
                        // Split the current range into 2. One part that will
                        // match the rule and one part that should pass through
                        // to the next rule.
                        // Get a new sub-range that satisfies this rule. Add it to the queue.
                        var min = op == RuleOperator.gt
                            ? Math.Max(nxt[rule.CategoryId].Min, rule.Value+1)
                            : nxt[rule.CategoryId].Min;
                        var max = op == RuleOperator.gt
                            ? nxt[rule.CategoryId].Max
                            : Math.Min(nxt[rule.CategoryId].Max, rule.Value-1);
                        var r1 = new RatingsRange(nxt);
                        r1.SetRange(rule.CategoryId, min, max);
                        q.Enqueue((r1, rule.Next));

                        // Get a new sub-range to pass through to the next rule.
                        min = op == RuleOperator.gt
                            ? nxt[rule.CategoryId].Min
                            : Math.Max(nxt[rule.CategoryId].Min, rule.Value);
                        max = op == RuleOperator.gt
                            ? Math.Min(nxt[rule.CategoryId].Max, rule.Value)
                            : nxt[rule.CategoryId].Max;
                        nxt.SetRange(rule.CategoryId, min, max);
                        break;
                    default:
                        throw new Exception($"Unhandled rule operator {op}");
                }
            }
        }

        return accepted.Sum64(ar => ar.Combinations);
    }

    public record RatingRange(int Min = 1, int Max = 4000)
    {
        public int Count => Max < Min ? 0 : Max - Min + 1;
    }

    public record RatingsRange
    {
        private static readonly char[] RangeIds = ['x','m','a','s'];
        private readonly RatingRange[] _ranges;

        /// <summary>
        /// Create a new RatingsRange with default values (1..4000) for each
        /// of the X, M, A, S ratings ranges.
        /// </summary>
        public RatingsRange()
        {
            _ranges = new RatingRange[4];
            for (var i = 0; i < _ranges.Length; i++)
                _ranges[i] = new RatingRange();
        }

        public RatingsRange(RatingsRange src)
        {
            _ranges = new RatingRange[4];
            for (var i = 0; i < _ranges.Length; i++)
                _ranges[i] = src._ranges[i];
        }

        public RatingRange this[char index] => _ranges[Array.IndexOf(RangeIds, index)];

        public void SetRange(char rangeId, int min, int max)
        {
            _ranges[Array.IndexOf(RangeIds, rangeId)] = new RatingRange(min, max);
        }

        public Int64 Combinations =>
            (Int64)_ranges[0].Count * (Int64)_ranges[1].Count
            * (Int64)_ranges[2].Count * (Int64)_ranges[3].Count;
    }

    /// <summary>
    /// Run the given machine part through the full workflow to determine if
    /// it is acceptable or not.
    /// </summary>
    private bool AcceptPart(Dictionary<string, Workflow> wfd, MachinePart mp)
    {
        var wfid = "in";
        while (wfid != "A" && wfid != "R")
        {
            var wf = wfd[wfid];
            foreach (var rl in wf.Rules)
            {
                wfid = ProcessRule(rl, mp);
                if (!string.IsNullOrWhiteSpace(wfid))
                    break;
            }
        }
        return wfid == "A";
    }

    /// <summary>
    /// Check the machine part against the given workflow rule. If it matches
    /// the rule then return the next workflow id otherwise return ""/blank to
    /// indicate we need to continue on to the next available rule in the
    /// current workflow.
    /// </summary>
    /// <returns>The id of the next workflow to apply or an empty string if it
    /// does not match the rule.</returns>
    private string ProcessRule(WorkflowRule rule, MachinePart mp)
    {
        switch (rule.CategoryId)
        {
            case 'x':
                if ((rule.Op == RuleOperator.lt && mp.X < rule.Value)
                 || (rule.Op == RuleOperator.gt && mp.X > rule.Value))
                  return rule.Next;
                break;
            case 'm':
                if ((rule.Op == RuleOperator.lt && mp.M < rule.Value)
                 || (rule.Op == RuleOperator.gt && mp.M > rule.Value))
                  return rule.Next;
                break;
            case 'a':
                if ((rule.Op == RuleOperator.lt && mp.A < rule.Value)
                 || (rule.Op == RuleOperator.gt && mp.A > rule.Value))
                  return rule.Next;
                break;
            case 's':
                if ((rule.Op == RuleOperator.lt && mp.S < rule.Value)
                 || (rule.Op == RuleOperator.gt && mp.S > rule.Value))
                  return rule.Next;
                break;
            case '_':
                return rule.Next;
            default:
                throw new Exception($"Unhandled rule category {rule.CategoryId}");
        }
        return string.Empty;
    }

    private Dictionary<string, Workflow> BuildWorkflow(IEnumerable<string> input)
    {
        var result = new Dictionary<string, Workflow>();
        foreach (var ln in input)
        {
            var i = ln.IndexOf('{');
            var id = ln[..i];
            var rls = ln[(i+1)..^1].Split(',');
            var rules = new List<WorkflowRule>();
            foreach (var rl in rls)
            {
                var parts = rl.Split(':');
                if (parts.Length > 1)
                {
                    var cid = rl[0];
                    var op = rl[1] == '<'
                        ? RuleOperator.lt
                        : rl[1] == '>'
                            ? RuleOperator.gt
                            : throw new Exception($"Unknown rule operator {rl[0]}");
                    var val = int.Parse(parts[0][2..]);
                    var nx = parts[1];
                    rules.Add(new WorkflowRule(cid, op, val, nx));
                }
                else
                {
                    var op = parts[0] switch
                    {
                        "A" => RuleOperator.Accept,
                        "R" => RuleOperator.Reject,
                        _ => RuleOperator.Goto,
                    };
                    rules.Add(new WorkflowRule('_', op, 0, parts[0]));
                }
            }
            result.Add(id, new Workflow(id, rules.ToArray()));
        }
        return result;
    }

    private List<MachinePart> BuildPartList(IEnumerable<string> input)
    {
        var result = new List<MachinePart>();
        foreach (var ln in input)
        {
            var sp = ln[1..^1].Split(',').Select(s => int.Parse(s.Split('=')[1])).ToArray();
            result.Add(new MachinePart(sp[0], sp[1], sp[2], sp[3]));
        }
        return result;
    }

    private enum RuleOperator { gt, lt, Goto, Accept, Reject }
    private record MachinePart(int X, int M, int A, int S)
    {
        public int Rating => X + M + A + S;
    }
    private record Workflow(string Id, WorkflowRule[] Rules);
    private record WorkflowRule(char CategoryId, RuleOperator Op, int Value, string Next);
}
