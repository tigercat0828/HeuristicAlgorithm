using Library.Solvers;
using ScottPlot;
using ScottPlot.TickGenerators;

namespace Library.IO;
/// <summary>
/// A class for making figure
/// </summary>
public class Figure {
    public static readonly string[] ColorHex = ["#eb4034", "#eba834", "#bdeb34", "#46eb34", "#34eb9b", "#34c9eb", "#3471eb", "#5e34eb", "#bd34eb"];
    public static readonly int ColorNum = ColorHex.Length;
    public Plot Plot { get; private set; } = new();

    public Figure(string title, string xLabel, string yLabel) {
        Plot.Title(title);
        Plot.XLabel(xLabel);
        Plot.YLabel(yLabel);
    }
    /// <summary>
    /// Plot a group of (x,y) corrdinate
    /// </summary>
    public void ScatterChart<T>(List<T> xs, List<T> ys) {
        Plot.Add.Scatter(xs, ys);
    }
    /// <summary>
    /// Plot a group of (x,y) corrdinate
    /// </summary>
    public void ScatterChart<T>(T[] xs, T[] ys) {
        Plot.Add.Scatter(xs, ys);
    }
    /// <summary>
    /// Plot multiple group of coordinate (x, y)
    /// </summary>
    public void ScatterChart<T>(List<T[]> xss, List<T[]> yss) {
        if (xss.Count != yss.Count) {
            throw new Exception("Not consistent length");
        }
        Plot plot = new();
        for (int i = 0; i < xss.Count; i++) {
            plot.Add.Scatter(xss[i], yss[i]);
        }
    }

    public void GanttChart(IEnumerable<Bar> bars, int instanceNum) {
        var barPlot = Plot.Add.Bars(bars);
        barPlot.Horizontal = true;

        NumericManual ticks = new();
        for (int i = 0; i < instanceNum; i++) ticks.AddMajor(i + 1, (i + 1).ToString());
        Plot.Axes.Margins(left: 0);
        Plot.Axes.Left.TickGenerator = ticks;
        Plot.Axes.SetLimitsY(bottom: instanceNum + 1, top: 0); // reverse the axis
    }

    public void GanttChart(int[][] data, JobSche sche) {
        int instanceNum = data.First().Length;
        var bars = GetGhattBars(data, sche);
        GanttChart(bars, instanceNum);
    }
    /// <summary>
    /// save the figure into an image, only support .png file
    /// </summary>
    public void SaveFigure(string filename, int width = 1600, int height = 900) {
        Plot.SavePng(filename, width, height);
    }
    private static List<Bar> GetGhattBars(int[][] data, JobSche sche) {
        if (data.Length != sche.Order.Length) {
            throw new Exception("Data and the order length not match");
        }
        int jobs = data.Length;
        int machines = data.First().Length;
        List<Bar> bars = new(jobs * machines);
        int[] machineTime = new int[machines];
        foreach (int job in sche.Order) {
            int currentTime = machineTime[0];
            for (int mac = 0; mac < machines; mac++) {
                int start = Math.Max(machineTime[mac], currentTime);
                currentTime = start + data[job][mac];
                machineTime[mac] = start + data[job][mac];
                bars.Add(new() {
                    Position = mac + 1,
                    ValueBase = start,
                    Value = currentTime,
                    FillColor = Color.FromHex(ColorHex[job % ColorNum])
                });
            }
        }
        return bars;
    }
}
