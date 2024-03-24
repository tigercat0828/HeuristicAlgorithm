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
    public AcceptanceAlgo Answer { get; private set; }

    double Timecost;

    public int averageSpan = 0;
    public int bestspan = 0;
    public int worstspan =0;

    public ScheduleSolver(string filename, AcceptanceMethod method, int instanceNum) {
        Dataname = Path.GetFileNameWithoutExtension(filename);
        Method = method;
        ExperienceName = $"{Dataname}_{method}";
        InstanceNum = instanceNum;
        Data = LoadFile(filename);
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

        AcceptanceAlgo best = instances.First();
        AcceptanceAlgo worst = instances.First();
        foreach (var problem in instances) {
            if (problem.Result.makespan < best.Result.makespan) {
                best = problem;
            }
            if(problem.Result.makespan > best.Result.makespan) {
                worst = problem;
            }
            averageSpan += problem.Result.makespan;
            Console.Write(problem.Result.makespan + ", ");
        }
        Console.WriteLine();
        averageSpan /= instances.Count;
        bestspan = best.Result.makespan;
        worstspan = worst.Result.makespan;

        Answer = best;
        sw.Stop();
        Timecost = sw.Elapsed.TotalSeconds;
        sw.Reset();
        
        
        Plot(ExperienceName , Data, best.Result.order);       // make figure
        WriteLineResult(Console.Out);
    }
    private void WriteReportFile(TextWriter target) {
        target.WriteLine($"{bestspan}/{averageSpan}/{worstspan},");
    }
    private void WriteLineResult(TextWriter target) {
        target.WriteLine($"        Method : {Method}");
        target.WriteLine($"   InstanceNum : {InstanceNum}");
        target.WriteLine($"    Init Order : [{string.Join(',', Answer.InitialOrder)}]");
        target.WriteLine($"    Best Order : [{string.Join(',', Answer.Result.order)}]");
        target.WriteLine($"best/avg/worst : {bestspan}/{averageSpan}/{worstspan}");
        target.WriteLine($"     Time cost : {Timecost:F2} seconds");
        target.WriteLine(new string('=', 80));
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
