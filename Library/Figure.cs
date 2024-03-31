using ScottPlot;
using ScottPlot.TickGenerators;

namespace Library;
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
    /// <summary>
    /// Plot a group of (x,y) corrdinate
    /// </summary>
    public void  ScatterChart<T>(T[] xs, T[] ys) {
        Plot.Add.Scatter(xs, ys);
    }
    /// <summary>
    /// Plot a GanttChart
    /// </summary>
    /// <param name="bars">span of each single job</param>
    /// <param name="instanceNum">machine num</param>
    /// <returns></returns>
    public void  GanttChart(IEnumerable<Bar> bars, int instanceNum) {
        
        var barPlot = Plot.Add.Bars(bars);
        barPlot.Horizontal = true;

        NumericManual ticks = new();
        for (int i = 0; i < instanceNum; i++) ticks.AddMajor(i + 1, (i + 1).ToString());
        Plot.Axes.Margins(left: 0);
        Plot.Axes.Left.TickGenerator = ticks;
        Plot.Axes.SetLimitsY(bottom: instanceNum + 1, top: 0); // reverse the axis
    }
    public void SaveFigure(string filename, int width = 1600, int height = 900) {
        Plot.SavePng(filename, width, height);
    }
}
