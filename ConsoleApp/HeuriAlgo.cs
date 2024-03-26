
namespace Heuristic;


public abstract class HeuriAlgo {
    public int[][] Data { get; private set; }
    public int[] InitialOrder { get; private set; }
    public int JobNum { get; private set; }
    public int MachineNum { get; private set; }
    public List<int> Spans { get; private set; } = [];
    public JobOrder Result => current;
    protected static readonly Random random = new();
    protected List<int[]> neighbors = new(1000);
    protected JobOrder current;
    public int IterTime { get; protected set; }
    public HeuriAlgo(int[][] data, int[] initOrder = null!) {
        Data = data;
        JobNum = data.Length;
        MachineNum = data[0].Length;
        if (initOrder != null) {
            InitialOrder = initOrder.ToArray();
        }
    }
    protected JobOrder GenerateInitialOrder() {
        if (InitialOrder == null) {
            int[] order = new int[JobNum];
            // Fisher-Yates Shuffle
            for (int i = 0; i < JobNum; i++) order[i] = i;
            for (int i = 0; i < JobNum; i++) {
                int t = random.Next(i + 1);
                (order[i], order[t]) = (order[t], order[i]);
            }
            int score = Measure(order);
            InitialOrder = order.ToArray();
            return new JobOrder(order, score);
        }
        else {
            int score = Measure(InitialOrder);
            return new JobOrder(InitialOrder, score);
        }
    }
    protected void GetNeighbors(int[] order) {
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
    protected int Select(List<int[]> neighbors) {
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
    public abstract JobOrder Run();
}
