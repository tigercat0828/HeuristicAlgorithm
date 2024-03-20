using ScottPlot;
using ScottPlot.TickGenerators;
using System.Globalization;
using System.Runtime.ExceptionServices;

public class SchedulingProblem {

    // n job, m machine
    public  int[][] Data {get;private set;}
    public int Jobs {get; private set;}
    public int Machines {get; private set;}
    public int IterativeTime { get; private set; }

    public int CurrentScore;    // lower value is better
    public int[] CurrentOrder;

    public readonly string Filename;
    static readonly string[] ColorHex = ["#eb4034", "#eba834", "#bdeb34", "#46eb34", "#34eb9b", "#34c9eb", "#3471eb", "#5e34eb", "#bd34eb"];
    static readonly int ColorNum = ColorHex.Length;
    
    public SchedulingProblem(string filename)
    {
        Filename = Path.GetFileNameWithoutExtension(filename);
        LoadData(filename);

    }
    private int[] InitialOrder()
    {
        // [0,1,2,3,4,5,6,7,8,9, ...]
        int[] order = new int[Jobs];
        for (int i = 0; i < Jobs; i++) order[i] = i;
        return order;
        // or random
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
    public int MeasureAndPlot(int[] jobOrder) {

        List<Bar> bars = new(Jobs * Machines);
        int[] machineTime = new int[Machines];
        foreach (int job in jobOrder) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < Machines; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + Data[job][mac];
                machineTime[mac] = start + Data[job][mac];
                bars.Add(new() { Position = mac + 1, ValueBase = start, Value = currentTime, FillColor = Color.FromHex(ColorHex[job % ColorNum]) });
            }
        }

        Plot myPlot = new();
        var barPlot = myPlot.Add.Bars(bars.ToArray());
        barPlot.Horizontal = true;


        NumericManual ticks = new();
        for (int i = 0; i < Machines; i++) ticks.AddMajor(i + 1, (i + 1).ToString());

        myPlot.Axes.Margins(left: 0);
        myPlot.Axes.Left.TickGenerator = ticks;
        myPlot.Axes.SetLimitsY(bottom: Machines + 1, top: 0); // reverse the axis

        myPlot.Title("Gantt");
        myPlot.XLabel("Time");
        myPlot.YLabel("Machine");
        myPlot.SavePng($"{Filename}_iter{IterativeTime}.png", 2100, 900);

        return machineTime[Machines - 1];
    }
    
    private List<int[]> GetNeighbors(int[] jobOrder)
    {
        List<int[]> neighbors = [];
        int length = jobOrder.Length;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (i == j) continue;
                int[] temp = jobOrder.ToArray();
                (temp[i], temp[j]) = (temp[j], temp[i]);
                neighbors.Add(temp);
            }
        }
        return neighbors;
    }
    private void Select(List<int[]> neighbors)
    {
        // Apply best-improve
        foreach (var jobOrder in neighbors)
        {
            int score = Measure(jobOrder);
            if(score < CurrentScore)
            {
                CurrentScore = score;
                CurrentOrder = jobOrder;
            }
        }
    }
    public (int score, int[] order) ApplyII(int iterativeTime){

        IterativeTime = iterativeTime;
        CurrentOrder = InitialOrder();
        CurrentScore = Measure(CurrentOrder);
        for (int i = 0; i < iterativeTime; i++)
        {
            List<int[]> neighbors = GetNeighbors(CurrentOrder);
            Select(neighbors);
        }
        return (CurrentScore, CurrentOrder);
    }
    public void ApplyTS() {}
    public void ApplySA() { }

    public void LoadData(string filename) {

        string[] lines = File.ReadAllLines(filename);
        string[] tokens = lines[0].Split(' ');
        // m x n
        Jobs = int.Parse(tokens[0]);
        Machines = int.Parse(tokens[1]);


        int[][] data = new int[Jobs][];
        for (int i = 0; i < Jobs; i++) data[i] = new int[Machines];

        for (int i = 1; i < lines.Length; i++) {

            string[] elements = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < elements.Length; t++) {
                data[t][i - 1] = int.Parse(elements[t]);
            }
        }
        Data = data;
    }
    public void PrintData() {
        Console.WriteLine($"jobs: {Jobs} machines: {Machines}\n\n");
        Console.WriteLine(" Job  StageInterval");
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
    }
}



