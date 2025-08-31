namespace HeuristicAlgorithm.Core.Solvers;



public abstract class SolverBase {
    public SolverBase() {

    }
    public int[][] Data { get; private set; }               // the dataset table
    public int JobNum { get; private set; }                 // number of jobs
    public int MachineNum { get; private set; }             // number of machines
    public List<int> SpanHistory { get; protected set; }    // record the span during the solving process
    public Schedule Result { get; protected set; }          // the result solution
    public abstract Schedule Run(Schedule? initSolution = null!);
    public void LoadDataset(int[][] data) {
        Data = data;
        JobNum = data.First().Length;
        MachineNum = data.Length;
        SpanHistory = [];
    }
    public int Evaluate(int[] order) {

        int[] machineTime = new int[MachineNum];
        foreach (int job in order) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < MachineNum; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + Data[mac][job];
                machineTime[mac] = start + Data[mac][job];
            }
        }
        return machineTime[MachineNum - 1];
    }

    public Schedule InitialSolution() {
        int[] order = new int[JobNum];
        // Fisher-Yates Shuffle
        for (int i = 0; i < JobNum; i++) order[i] = i;
        Random random = new();
        for (int i = 0; i < JobNum; i++) {
            int t = random.Next(i + 1);
            (order[i], order[t]) = (order[t], order[i]);
        }
        Schedule sche = new() {
            order = order,
            makespan = Evaluate(order)
        };
        return sche;
    }
}


