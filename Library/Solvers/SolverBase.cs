namespace Library.Solvers;

/// <summary>
/// A abstract class host different heuristic method II, SA, TS, or RND
/// </summary>
public abstract class SolverBase {

    protected static readonly Random random = new();
    public int[][] Data { get; private set; }
    public int JobNum { get; private set; }
    public int MachineNum { get; private set; }
    public List<int> SpanList { get; private set; }
    public JobSche Result { get; protected set; }
    public SolverBase() { }

    /// <summary>
    /// perform the solve problem procedure 
    /// </summary>
    public abstract JobSche Run(JobSche init = null!);
    protected void EnsureDataLoaded() {
        if (Data is null) {
            throw new InvalidOperationException("Data is not loaded");
        }
    }
    public void SetData(int[][] data) {
        Data = data;
        JobNum = data.Length;
        MachineNum = data.First().Length;
        SpanList = [];
    }

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


    public JobSche InitialSolution() {
        int[] order = new int[JobNum];
        // Fisher-Yates Shuffle
        for (int i = 0; i < JobNum; i++) order[i] = i;
        for (int i = 0; i < JobNum; i++) {
            int t = random.Next(i + 1);
            (order[i], order[t]) = (order[t], order[i]);
        }
        JobSche sche = new() {
            Order = order,
            Makespan = Evaluate(order)
        };
        return sche;
    }

}