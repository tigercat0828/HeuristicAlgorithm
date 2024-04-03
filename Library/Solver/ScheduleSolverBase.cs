using ScottPlot;

namespace Library.Solver;

/// <summary>
/// A abstract class host different heuristic method II, SA, TS, or RND
/// </summary>
public abstract class ScheduleSolverBase {

    public readonly int[][] Data;
    public readonly int JobNum;
    public readonly int MachineNum;
    protected static readonly Random random = new();
    public JobSche Result { get; protected set; }
    public ScheduleSolverBase(int[][] data) {
        Data = data;
        JobNum = data.Length;
        MachineNum = data.First().Length;
    }

    /// <summary>
    /// perform the solve problem procedure 
    /// </summary>
    public abstract JobSche Run(JobSche init = null!);
    public abstract JobSche RunMultiInstance(int instance);
    protected abstract JobSche Select(List<JobSche> neighbors, JobSche localBest);

    /// <summary>
    /// measure makespan of order
    /// </summary>
    public int Evaluate(int[] order) {
        int[] machineTime = new int[MachineNum];
        foreach (int job in order) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < MachineNum; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + Data[job][mac];
                machineTime[mac] = start + Data[job][mac];
            }
        }
        return machineTime[MachineNum - 1];
    }
    public List<Bar> GetGhattBars(JobSche sche = null) {
        if (sche is null) {
            sche = Result;
        }
        List<Bar> bars = new(JobNum * MachineNum);
        int[] machineTime = new int[MachineNum];
        foreach (int job in sche.order) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < MachineNum; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + Data[job][mac];
                machineTime[mac] = start + Data[job][mac];
                bars.Add(new() {
                    Position = mac + 1,
                    ValueBase = start,
                    Value = currentTime,
                    FillColor = ScottPlot.Color.FromHex(Figure.ColorHex[job % Figure.ColorNum])
                });
            }
        }
        return bars;
    }
    public List<JobSche> Neighbors(JobSche sche) {
        // swap all (i,j) 
        List<JobSche> neighbors = [];
        int[] order = sche.order;

        for (int i = 0; i < order.Length; i++) {
            for (int j = i + 1; j < order.Length; j++) {
                int[] temp = [.. order];
                (temp[i], temp[j]) = (temp[j], temp[i]);

                JobSche newSche = new(temp, Evaluate(temp));
                neighbors.Add(newSche);
            }
        }
        return neighbors;
    }
    public JobSche InitialSolution() {
        int[] order = new int[JobNum];
        // Fisher-Yates Shuffle
        for (int i = 0; i < JobNum; i++) order[i] = i;
        for (int i = 0; i < JobNum; i++) {
            int t = random.Next(i + 1);
            (order[i], order[t]) = (order[t], order[i]);
        }
        JobSche sche = new() {
            order = order,
            makespan = Evaluate(order)
        };
        return sche;
    }
    public void CheckData() {
        Console.WriteLine($"jobs: {JobNum} machines: {MachineNum}");
        for (int i = 0; i < Data.Length; i++) {
            Console.WriteLine($"{i + 1}, [{string.Join(", ", Data[i])}]");
        }
    }
}