using ScottPlot;
using ScottPlot.TickGenerators;
using System.Diagnostics;
namespace Heuristic;

public partial class Schedule {
    public string ExpName { get; private set; }
    public int[][] Data { get; private set; }
    public int[] InitialOrder { get; private set; }
    public int Jobs { get; private set; }
    public int Machines { get; private set; }
    public VariantMethod AppliedMethod { get; private set; }
    public int IterTime { get; private set; }
    public JobOrder Result => Current;
    public List<int> Spans { get; private set; } = [];
    private static readonly string[] ColorHex = ["#eb4034", "#eba834", "#bdeb34", "#46eb34", "#34eb9b", "#34c9eb", "#3471eb", "#5e34eb", "#bd34eb"];
    private static readonly int ColorNum = ColorHex.Length;
    private static readonly Random random = new();
    private List<int[]> neighbors = new(1000);
    public JobOrder Current { get; private set; }
    public Schedule(string expName)
    {
        ExpName = expName;
    }
    private int[] DefaultOrder = null!;
    public void SetInitialOrder(int[] order) {
        DefaultOrder = [.. order];
    }
    private JobOrder GenerateInitialOrder() {
        if (DefaultOrder == null) {
            int[] order = new int[Jobs];
            // Fisher-Yates Shuffle
            for (int i = 0; i < Jobs; i++) order[i] = i;
            for (int i = 0; i < Jobs; i++) {
                int t = random.Next(i + 1);
                (order[i], order[t]) = (order[t], order[i]);
            }
            int score = Measure(order);
            return new JobOrder(order, score);
        }
        else {
            int score = Measure(DefaultOrder);
            return new JobOrder(DefaultOrder, score);
        }
    }
    private void GetNeighbors(int[] order) {
        neighbors.Clear();
        for (int i = 0; i < order.Length; i++) {
            for (int j = 0; j < order.Length; j++) {
                if (i == j) continue;
                int[] temp = [.. order];
                (temp[i], temp[j]) = (temp[j], temp[i]);
                neighbors.Add(temp);
            }
        }
    }
    private int Select(List<int[]> neighbors) {

        // Apply best-improve
        foreach (int[] nei in neighbors) {
            int score = Measure(nei);
            if (score < Current.makespan) {
                Current.makespan = score;
                Current.order = nei;
            }
        }
        return Current.makespan;
    }
    public int Measure(int[] jobOrder) {

        int[] machineTime = new int[Machines];
        foreach (int job in jobOrder) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < Machines; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + Data[job][mac];
                machineTime[mac] = start + Data[job][mac];
            }
        }
        return machineTime[Machines - 1];

    }
    public static int Plot(string filename,int[][] data,int[] jobOrder) {
        int jobs = data.Length;
        int machines = data[0].Length;
        List<Bar> bars = new(jobs * machines);
        int[] machineTime = new int[machines];
        foreach (int job in jobOrder) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < machines; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + data[job][mac];
                machineTime[mac] = start + data[job][mac];
                bars.Add(new() { Position = mac + 1, ValueBase = start, Value = currentTime, FillColor = Color.FromHex(ColorHex[job % ColorNum]) });
            }
        }

        Plot myPlot = new();
        var barPlot = myPlot.Add.Bars(bars.ToArray());
        barPlot.Horizontal = true;

        NumericManual ticks = new();
        for (int i = 0; i < machines; i++) ticks.AddMajor(i + 1, (i + 1).ToString());

        myPlot.Axes.Margins(left: 0);
        myPlot.Axes.Left.TickGenerator = ticks;
        myPlot.Axes.SetLimitsY(bottom: machines + 1, top: 0); // reverse the axis

        myPlot.Title("Gantt");
        myPlot.XLabel("Time");
        myPlot.YLabel("Machine");
        myPlot.SavePng($"./Output/{filename}.png", 2100, 900);

        return machineTime[machines - 1];
    }
    public void Apply(VariantMethod method) {
        AppliedMethod = method;
    }
    public void Run() {
        switch (AppliedMethod) {
            case VariantMethod.II:
                IterativeImprove();
                break;
            case VariantMethod.SA:
                SimulateAnnealing();
                break;
            case VariantMethod.TS:
                TabuSearch();
                break;
            case VariantMethod.RND:
                break;
            default:
                break;
        }
    }
    public JobOrder IterativeImprove() {
        Spans.Clear();
        Current = GenerateInitialOrder();
        Spans.Add(Current.makespan);
        InitialOrder = [.. Current.order];

        int itertime = 0;
        int previousSpan;
        do {
            previousSpan = Current.makespan;
            GetNeighbors(Current.order);
            Select(neighbors);
            itertime++;
            Spans.Add(Current.makespan);
        } while (Current.makespan != previousSpan);
        IterTime = itertime;
        return Current;
    }
    public JobOrder TabuSearch() {
        throw new NotImplementedException();
    }
    public JobOrder SimulateAnnealing() {
        throw new NotImplementedException();
    }
    public JobOrder FullRandom() {
        throw new NotImplementedException();
    }
    public static int[][] LoadFile(string filename) {
        string[] lines = File.ReadAllLines(filename);
        string[] tokens = lines[0].Split(' ');
        int jobs = int.Parse(tokens[0]);
        int machines = int.Parse(tokens[1]);
        int[][] data = new int[jobs][];
        for (int i = 0; i < jobs; i++) data[i] = new int[machines];
        for (int i = 1; i < lines.Length; i++) {
            string[] elements = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < elements.Length; t++) {
                data[t][i - 1] = int.Parse(elements[t]);
            }
        }
        return data;
    }
    public void LoadData(int[][] data) {
        Jobs= data.Length;
        Machines = data[0].Length;
        Data = data; // shallow copy
    }
    public void PrintData() {
        Console.WriteLine($"jobs: {Jobs} machines: {Machines}\n\n");
        Console.WriteLine(" Job  Stage");
        for (int i = 0; i < Jobs; i++) {
            if (i % 2 == 0) Console.ForegroundColor = ConsoleColor.White;
            else Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.Write($"[{i + 1,2}] ");
            for (int j = 0; j < Machines; j++) {
                Console.Write($"{Data[i][j],3}");
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(new string('-', 80)); 
    }
    public void PrintExpResult() {
        PrintAndWriteResult(Console.Out);
    }
    public void WriteExpResult() {
        string file = $"./Output/{ExpName}_{AppliedMethod}.txt";
    
        string directory = Path.GetDirectoryName(file);
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        using StreamWriter writer = new(file, true);
        PrintAndWriteResult(writer);
    }
    private void PrintAndWriteResult(TextWriter target) {
        // II
        target.WriteLine($"         Method : {AppliedMethod}");
        target.WriteLine($" Iterative Time : {IterTime}");
        target.WriteLine($" Jobs, Machines : {Jobs}, {Machines}");
        target.WriteLine($"  Initial Order : [{string.Join(',', InitialOrder)}]");
        int count = Spans.Count;
        if(count > 6) {
            target.WriteLine($"      Makespans : [{Spans[0]}, {Spans[1]}, {Spans[2]}, ... , {Spans[count-3]},{Spans[count-2]},{Spans[count-1]}]");
        }
        else {
            target.WriteLine($"      Makespans : [{string.Join(',', Spans)}]");
        }
        target.WriteLine($"  Best Makespan : {Current.makespan} seconds");
        target.WriteLine($"   Result Order : [{string.Join(',', Current.order)}]");
        target.WriteLine(new string('-', 80));
    }
#if MT_METHOD
    public JobOrder ApplyII_MT(int iterativeTime) {
        
            current = Initial();
            for (int i = 0; i < iterativeTime; i++) {
                GetNeighbors(current.order);
                SelectMT(neighbors);
            }
            return current;
        }
    public void SelectMT(List<int[]> neighbors) {
            const int THREAD_NUMS = 10;
            Task<JobOrder>[] tasks = new Task<JobOrder>[10];
            int iterTimes = neighbors.Count;

            int iterationsPerThread = neighbors.Count / THREAD_NUMS;

            for (int i = 0; i < THREAD_NUMS; i++) {
                int start = i * iterationsPerThread;
                int end = (i == THREAD_NUMS - 1) ? neighbors.Count : (i + 1) * iterationsPerThread;
                tasks[i] = Task.Run(() => select(neighbors, current, start, end));
            }
            Task.WaitAll(tasks);

            foreach (var task in tasks) {
                if (task.Result.makespan < current.makespan) {
                    current = task.Result;
                }
            }
            //==================================================================================
            JobOrder select(List<int[]> neighbors, JobOrder currentBest, int start, int end) {
                JobOrder groupBest = new(currentBest);
                for (int i = start; i < end; i++) {
                    int score = Measure(neighbors[i]);
                    if (score < groupBest.makespan) {
                        groupBest.makespan = score;
                        groupBest.order = neighbors[i];
                    }
                }
                return groupBest;
            }
        }
#endif
}
/*
string dataname = "tai20_20_1";
int[][] data = SchedulingProblem.LoadFile($"./Dataset/{dataname}.txt");

SchedulingProblem problem = new($"{dataname}");
problem.LoadData(data);
problem.SetInitialOrder(Enumerable.Range(0, problem.Jobs - 1).ToArray());
problem.Apply(VariantMethod.II);    
problem.Run();

problem.PrintExpResult();
problem.WriteExpResult();
// make figure
problem.MeasureAndPlot(problem.Current.order);
 */