using ScottPlot;
using ScottPlot.TickGenerators;

public class SchedulingProblem {

    // n job, m machine
    public int[][] data;
    public int jobs;
    public int machines;
    public int[] result;
    public int bestScore;
    public string filename;
    
    static string[] ColorHex = ["#eb4034", "#eba834", "#bdeb34", "#46eb34", "#34eb9b", "#34c9eb", "#3471eb", "#5e34eb", "#bd34eb"];
    static int ColorNum = ColorHex.Length;

    
    public SchedulingProblem(string filename)
    {
        this.filename = Path.GetFileNameWithoutExtension(filename);
        Load(filename);
        result = new int[jobs];
    }

    public int Measure(int[] jobOrder) {
        
        int[] machineTime = new int[machines];
        foreach (int job in jobOrder) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < machines; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + data[job][mac];
                machineTime[mac] = start + data[job][mac];
            }
        }
        return machineTime[machines - 1];
    }
    public int MeasureAndPlot(int[] jobOrder) {

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
        myPlot.SavePng($"{filename}.png", 2100, 900);

        return machineTime[machines - 1];
    }
    
    public void ApplyII(){}
    public void ApplyTS() {}
    public void ApplySA() { }

    public void Load(string filename) {

        string[] lines = File.ReadAllLines(filename);
        string[] tokens = lines[0].Split(' ');
        // m x n
        jobs = int.Parse(tokens[0]);
        machines = int.Parse(tokens[1]);


        int[][] data = new int[jobs][];
        for (int i = 0; i < jobs; i++) data[i] = new int[machines];

        for (int i = 1; i < lines.Length; i++) {

            string[] elements = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < elements.Length; t++) {
                data[t][i - 1] = int.Parse(elements[t]);
            }
        }
        this.data =  data;
    }
    public void Print() {
        Console.WriteLine($"jobs: {jobs} machines: {machines}\n");
        for (int i = 0; i < jobs; i++) {
            if (i % 2 == 0) Console.ForegroundColor = ConsoleColor.White;
            else Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.Write($"[{i + 1,2}] ");
            for (int j = 0; j < machines; j++) {
                Console.Write($"{data[i][j],3}");
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
}



