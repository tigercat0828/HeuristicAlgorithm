using ScottPlot;
using ScottPlot.TickGenerators;
using Spectre.Console;
using System.Diagnostics;
using System.Threading;

namespace Heuristic;

public class ScheduleSolver {
    public string ExperienceName { get; set; }
    public string Dataname { get; private set; }
    public int[][] Data { get; private set; }
    public int JobNum { get; private set; }
    public int MachineNum { get; private set; }
    public int InstanceNum { get; private set; }
   
    public AcceptanceMethod Method { get; private set; } = AcceptanceMethod.II;
    private List<AcceptanceAlgo> instances;
    public ScheduleSolver(string filename, AcceptanceMethod method, int instanceNum) {
        Dataname = Path.GetFileNameWithoutExtension(filename);
        InstanceNum = instanceNum;
        Data = LoadFile(filename);
        Method = method;
        BuildInstances();
    }
    private void BuildInstances() {
        instances = new(InstanceNum);
        IterativeImprove origin = new(Data, Enumerable.Range(0, Data.Length).ToArray());
        instances.Add(origin);

        for (int i = 0; i < InstanceNum; i++) {
            IterativeImprove sche = new(Data);
            instances.Add(sche);
        }
    }
    public void Run() {


        Stopwatch sw = new();
        sw.Start();

        // multi-thread
        Parallel.For(0, instances.Count, i => { 
            instances[i].Run(); 
        });
        //for (int i = 0; i < schedules.Count; i++) schedules[i].Run();

        best = instances.First();
        foreach (var problem in instances) {
            if (problem.Result.makespan < best.Result.makespan) {
                best = problem;
            }
        }
        sw.Stop();
        double timecost = sw.Elapsed.TotalSeconds;
        sw.Reset();
        Plot(ExperienceName , Data, best.Result.order);       // make figure
        PrintAndWriteResult(Console.Out);
    }
    AcceptanceAlgo best;
    double timecost;
    private void PrintAndWriteResult(TextWriter target) {
        target.WriteLine($"Init Order : [{string.Join(',', best.InitialOrder)}]");
        target.WriteLine($"Best Order : [{string.Join(',', best.Result.order)}]");
        target.WriteLine($"  Makespan : {best.Result.makespan} seconds");
        target.WriteLine($"    Method : {Method}");
        target.WriteLine($" Time cost : {timecost:F2} seconds");
        target.WriteLine($"InstanceNum: {InstanceNum}");
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
    private static readonly string[] ColorHex = ["#eb4034", "#eba834", "#bdeb34", "#46eb34", "#34eb9b", "#34c9eb", "#3471eb", "#5e34eb", "#bd34eb"];
    private static readonly int ColorNum = ColorHex.Length;
    public static int Plot(string filename, int[][] data, int[] jobOrder) {
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
                bars.Add(new() { Position = mac + 1, ValueBase = start, Value = currentTime, FillColor = ScottPlot.Color.FromHex(ColorHex[job % ColorNum]) });
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
        string path = $"";
        if (!Directory.Exists("./Output")) {
            Directory.CreateDirectory("./Output");
        }
        myPlot.SavePng($"./Output/{filename}.png", 2100, 900);

        return machineTime[machines - 1];
    }
    public void PrintData() {
        Console.WriteLine($"jobs: {JobNum} machines: {MachineNum}\n\n");
        Console.WriteLine(" Job  Stage");
        for (int i = 0; i < JobNum; i++) {
            if (i % 2 == 0) Console.ForegroundColor = ConsoleColor.White;
            else Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.Write($"[{i + 1,2}] ");
            for (int j = 0; j < MachineNum; j++) {
                Console.Write($"{Data[i][j],3}");
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(new string('-', 80));
    }
}
