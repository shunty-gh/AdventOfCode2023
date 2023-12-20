namespace Shunty.AoC;

// https://adventofcode.com/2023/day/20 - Pulse Propagation

public class Day20 : AocDaySolver
{
    public int DayNumber => 20;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var q = new Queue<PulseQueueItem>();
        var modules = BuildModules(input, q);

        // Part 1
        Int64 p1_lo = 0, p1_hi = 0;
        for (var i = 0; i < 1000; i++)
        {
            var (lo, hi, _) = RunCycle(modules, q);
            p1_lo += lo;
            p1_hi += hi;
        }
        this.ShowDayResult(1, p1_lo * p1_hi);

        // Part 2
        // Reset the modules
        modules = BuildModules(input, q);
        // Build a Mermaid diagram to help with part 2
        var fdir = Path.GetDirectoryName(AocUtils.FindInputFile(DayNumber)) ?? "";
        var fn = Path.Combine(fdir, $"day{DayNumber:D2}-mermaid");
        ToMermaid(modules, fn);
        // Run the prog to output the Mermaid file, copy and paste the
        // content into https://mermaid.live/edit
        // This reveals that there is one `conjunction` module that feeds the
        // target "rx" module. In order for this module to send a Lo pulse, as
        // required by the puzzle, all the inputs to this module must be Hi.
        // Analysis of the input or the Mermaid diagram shows there are exactly
        // four conjunction inputs that feed this final conjunction module.
        // We know from trial and error and previous AoC puzzles that it will be
        // virtually impossible to run this to completion manually so presumably
        // each of those four modules have some form period at which they will
        // become Hi. We need to find the period for each one separately then
        // find the LCM.
        // As it happens the period values are prime numbers so we could just
        // multiply them together.

        // Find the module that feeds the rx module then find the modules that
        // feed the feeder:
        var feedsrx = modules.First(kvp => kvp.Value.Outputs.Contains("rx")).Key;
        var feeders = modules.Where(kvp => kvp.Value.Outputs.Contains(feedsrx))
            .Select(kvp => kvp.Key)
            .ToList();
        // Find the period for each feeder
        var periods = new List<Int64>();
        foreach (var k in feeders)
        {
            // Reset the modules
            modules = BuildModules(input, q);

            int idx = 0;
            while (idx < 8000) // An arbitrary value that we hope will cover the periods for each module
            {
                idx++;
                var (_, _, watchfound) = RunCycle(modules, q, k);
                if (watchfound)
                {
                    periods.Add(idx);
                    break;
                }
            }
        }
        System.Diagnostics.Debug.Assert(periods.Count == 4);
        long p2 = periods.LCM();
        this.ShowDayResult(2, p2);
    }

    private (Int64 Lo, Int64 Hi, bool foundWatch) RunCycle(Dictionary<string, ModuleBase> modules, Queue<PulseQueueItem> q, string watchFor = "")
    {
        Int64 lo_p = 0, hi_p = 0;
        var watchfound = false;
        q.Enqueue(new PulseQueueItem("", "broadcaster", Pulse.Lo));
        while (q.Count > 0)
        {
            var pqi = q.Dequeue();
            if (pqi.Pulse == Pulse.Lo)
                lo_p += 1;
            else
                hi_p += 1;

            if (!modules.ContainsKey(pqi.Recipient))
            {
                continue;
            }
            var m = modules[pqi.Recipient];
            m.ProcessPulse(pqi.Sender, pqi.Pulse);

            // For part 2 - watch one of the final feeder modules
            if (m.Id == watchFor && m.ModuleType == ModuleType.Conjunction)
            {
                // What we are looking for is when the watched module sends a Hi
                // pulse. ie when not all of its inputs are high
                if (m is ConjunctionModule cm && cm is not null && !cm.AllHigh)
                {
                    watchfound = true;
                }
            }

        }
        return (lo_p, hi_p, watchfound);
    }

    private Dictionary<string, ModuleBase> BuildModules(IList<string> input, Queue<PulseQueueItem> q)
    {
        var modules = new Dictionary<string, ModuleBase>();
        foreach (var line in input)
        {
            var mod = CreateModuleFromInputLine(line, q);
            if (mod is not null)
                modules.Add(mod.Id, mod);
        }
        foreach (var (k,m) in modules)
        {
            foreach (var r in m.Outputs)
            {
                if (modules.ContainsKey(r))
                {
                    var m2 = modules[r];
                    m2.AddInput(m.Id);
                }
            }
        }
        return modules;
    }

    private ModuleBase? CreateModuleFromInputLine(string line, Queue<PulseQueueItem> queue)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var sp = line.Split(" -> ");
        string key;
        var mt = ModuleType.Unknown;
        switch (sp[0][0])
        {
            case '#':
                return null;
            case 'b':
                key = sp[0];
                mt = ModuleType.Broadcaster;
                break;
            case '%':
                key = sp[0][1..];
                mt = ModuleType.FlipFlop;
                break;
            case '&':
                key = sp[0][1..];
                mt = ModuleType.Conjunction;
                break;
            default:
                throw new Exception($"Unknown first character {sp[0][0]}");
        }
        ModuleBase result = mt switch
        {
            ModuleType.FlipFlop => new FlipFlopModule(key, queue),
            ModuleType.Conjunction => new ConjunctionModule(key, queue),
            ModuleType.Broadcaster => new BroadcasterModule(key, queue),
            _ => throw new Exception($"Unhandled module type {mt}"),
        };
        var recips = sp[1].Split(", ");
        foreach (var recip in recips)
        {
            result.AddOutput(recip.Trim());
        }
        return result;
    }

    private abstract class ModuleBase
    {
        protected List<string> _outputs = new List<string>();
        protected List<string> _inputs = new List<string>();
        protected Queue<PulseQueueItem> _queue;

        public string Id { get; }

        public ModuleType ModuleType { get; protected set; } = ModuleType.Unknown;

        public IReadOnlyList<string> Outputs => _outputs.AsReadOnly();
        public IReadOnlyList<string> Inputs => _inputs.AsReadOnly();

        public ModuleBase(string id, Queue<PulseQueueItem> queue)
        {
            Id = id;
            _queue = queue;
        }

        public abstract int ProcessPulse(string from, Pulse pulse);

        protected void SendPulse(Pulse pulse)
        {
            foreach (var output in _outputs)
            {
                _queue.Append(new PulseQueueItem(Id, output, pulse));
            }
        }

        public virtual void AddOutput(string id)
        {
            _outputs.Add(id);
        }

        public virtual void AddInput(string id)
        {
            _inputs.Add(id);
        }
    }

    private class BroadcasterModule : ModuleBase
    {
        public BroadcasterModule(string id, Queue<PulseQueueItem> queue)
            : base(id, queue)
        {
            ModuleType = ModuleType.Broadcaster;
        }

        public override int ProcessPulse(string from, Pulse pulse)
        {
            // Broadcaster sends any pulse on to all its outputs
            foreach (var ouput in _outputs)
            {
                _queue.Enqueue(new PulseQueueItem(Id, ouput, pulse));
            }
            return _outputs.Count;
        }
    }

    private class FlipFlopModule : ModuleBase
    {
        private bool _state = false;

        public FlipFlopModule(string id, Queue<PulseQueueItem> queue)
            : base(id, queue)
        {
            ModuleType = ModuleType.FlipFlop;
        }

        public override int ProcessPulse(string from, Pulse pulse)
        {
            // If flip-flop receives a Hi pulse then ignore it
            // If it receives a Lo pulse then flip state and send a pulse
            if (pulse == Pulse.Hi)
                return 0;
            _state = !_state;
            var p = _state ? Pulse.Hi : Pulse.Lo;
            foreach (var output in _outputs)
            {
                _queue.Enqueue(new PulseQueueItem(Id, output, p));
            }
            return _outputs.Count;
        }
    }

    private class ConjunctionModule : ModuleBase
    {
        private List<Pulse> _inputStates = [];
        public bool AllHigh => _inputStates.All(p => p == Pulse.Hi);

        public ConjunctionModule(string id, Queue<PulseQueueItem> queue)
            : base(id, queue)
        {
            ModuleType = ModuleType.Conjunction;
        }

        public override int ProcessPulse(string from, Pulse pulse)
        {
            // Update pulse state for input then send Lo if all inputs are Hi
            // otherwise Hi
            var index = _inputs.IndexOf(from);
            _inputStates[index] = pulse;

            var p = (pulse == Pulse.Lo || _inputStates.Any(p => p == Pulse.Lo))
                ? Pulse.Hi
                : Pulse.Lo;
            foreach (var output in _outputs)
            {
                _queue.Enqueue(new PulseQueueItem(Id, output, p));
            }
            return _outputs.Count;
        }

        public override void AddInput(string id)
        {
            base.AddInput(id);
            _inputStates.Add(Pulse.Lo);
        }
    }

    private void ToMermaid(Dictionary<string, ModuleBase> modules, string filename)
    {
        var lines = new List<string>();
        lines.Add("flowchart TD");
        foreach (var (_,m) in modules)
        {
            foreach (var s in m.Outputs)
            {
                var ln = $"{m.Id} --> {s}";
                if (s == "rx")
                {
                    ln += "(((\"`fa:fa-bullseye **RX** fa:fa-bullseye`\"))):::rx";
                }
                else
                {
                    var m2 = modules[s];
                    if (m2.ModuleType == ModuleType.FlipFlop)
                    {
                        ln += $"{{%{s}}}";
                    }
                    else if (m2.ModuleType == ModuleType.Conjunction)
                    {
                        ln += $"(&{s})";
                    }
                }
                // 'broadcaster' lines at the top, but after the 'flowchart' definition line
                if (m.Id == "broadcaster")
                    lines.Insert(1, ln);
                else
                    lines.Add(ln);
            }
        }
        // Add a final style for the rx node
        lines.Add("classDef rx fill:yellow");

        File.WriteAllLines(filename, lines);
    }

    private record PulseQueueItem(string Sender, string Recipient, Pulse Pulse);
    private enum Pulse { Lo, Hi, }
    private enum ModuleType { Unknown, Broadcaster, FlipFlop, Conjunction, Output }
}
