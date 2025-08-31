using ScottPlot.TickGenerators;
using System.Windows;

namespace HeuristicAlgorithm.WPF.Views;
/// <summary>
/// Diagram.xaml 的互動邏輯
/// </summary>
public partial class Diagram : Window {

    public int Makespan { get; set; }
    public string OrderStr { get; set; }
    public Diagram(string title, int makespan, int[] order, int[] spanHistory) {
        InitializeComponent();

        Makespan = makespan;
        OrderStr = string.Join(", ", order);

        double[] xs = [.. Enumerable.Range(0, spanHistory.Length).Select(i => (double)i)];
        double[] ys = [.. spanHistory.Select(i => (double)i)];

        HistoryPlot.Plot.Clear();
        HistoryPlot.Plot.Add.Scatter(xs, ys);
        HistoryPlot.Plot.Title(title + " Span History");
        HistoryPlot.Plot.XLabel("Iteration");
        HistoryPlot.Plot.YLabel("MakeSpan");
        HistoryPlot.Plot.Legend.IsVisible = true;

        HistoryPlot.Plot.Axes.Bottom.TickGenerator = new NumericFixedInterval(1);

        HistoryPlot.Refresh();
        DataContext = this;
    }
    private void Copy_Click(object sender, RoutedEventArgs e) {
        Clipboard.SetText(OrderStr);
        MessageBox.Show("Copied to clipboard!");
    }
}
