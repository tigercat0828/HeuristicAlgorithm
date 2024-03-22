using ScottPlot;
using ScottPlot.TickGenerators;
using System.Diagnostics;
namespace Heuristic;
public class IterativeImprove : AcceptanceAlgo {
    public int[][] Data { get; private set; }
    public int[] InitialOrder { get; private set; }
    public int JobNum { get; private set; }
    public int MachineNum { get; private set; }
    public int IterTime { get; private set; }
    public List<int> Spans { get; private set; } = [];
    public JobOrder Result => current;

    public static readonly Random random = new();
    public List<int[]> neighbors = new(1000);
    public JobOrder current;
    public IterativeImprove(int[][] data, int[] initOrder = null!)
    :base(data, initOrder)
    {
        Data = data;
        JobNum = data.Length;
        MachineNum = data[0].Length;
        if(initOrder != null)
        {
            InitialOrder = initOrder.ToArray();
        }
    }
    private JobOrder GenerateInitialOrder()
    {
        if (InitialOrder == null) {
            int[] order = new int[JobNum];
            // Fisher-Yates Shuffle
            for (int i = 0; i < JobNum; i++) order[i] = i;
            for (int i = 0; i < JobNum; i++)
            {
                int t = random.Next(i + 1);
                (order[i], order[t]) = (order[t], order[i]);
            }
            int score = Measure(order);
            InitialOrder = order.ToArray();
            return new JobOrder(order, score);
        }
        else
        {
            int score = Measure(InitialOrder);
            return new JobOrder(InitialOrder, score);
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
            if (score < current.makespan) {
                current.makespan = score;
                current.order = nei;
            }
        }
        return current.makespan;
    }
    public int Measure(int[] jobOrder) {
        int[] machineTime = new int[MachineNum];
        foreach (int job in jobOrder) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < MachineNum; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + Data[job][mac];
                machineTime[mac] = start + Data[job][mac];
            }
        }
        return machineTime[MachineNum - 1];
    }
    public JobOrder Run() {
        Spans.Clear();
        
        current = GenerateInitialOrder();
       

        Spans.Add(current.makespan);

        int itertime = 0;
        int previousSpan;
        do
        {
            previousSpan = current.makespan;
            GetNeighbors(current.order);
            Select(neighbors);
            itertime++;
            Spans.Add(current.makespan);
        } while (current.makespan != previousSpan);
        IterTime = itertime;
        return current;
    }
    public void PrintExpResult() {
        PrintAndWriteResult(Console.Out);
    }
    public void WriteExpResult(string filename) {
        string file = $"./Output/{filename}_II.txt";
    
        string directory = Path.GetDirectoryName(file);
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        using StreamWriter writer = new(file, true);
        PrintAndWriteResult(writer);
    }
    public void PrintAndWriteResult(TextWriter target) {
        // II
        target.WriteLine($"         Method : II");
        target.WriteLine($" Iterative Time : {IterTime}");
        target.WriteLine($" Jobs, Machines : {JobNum}, {MachineNum}");
        target.WriteLine($"  Initial Order : [{string.Join(',', InitialOrder)}]");
        int count = Spans.Count;
        if(count > 6) {
            target.WriteLine($"      Makespans : [{Spans[0]}, {Spans[1]}, {Spans[2]}, ... , {Spans[count-3]},{Spans[count-2]},{Spans[count-1]}]");
        }
        else {
            target.WriteLine($"      Makespans : [{string.Join(',', Spans)}]");
        }
        target.WriteLine($"  Best Makespan : {current.makespan} seconds");
        target.WriteLine($"   Result Order : [{string.Join(',', current.order)}]");
        target.WriteLine(new string('-', 80));
    }
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