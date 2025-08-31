using HeuristicAlgorithm.WPF.ViewModels;
using System.Windows;


namespace HeuristicAlgorithm.WPF.Views;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    private readonly MainWindowViewModel _viewModel = new();

    public MainWindow() {
        InitializeComponent();
        DataContext = _viewModel = new MainWindowViewModel();
    }

    private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e) {
        e.Row.Header = $"M{e.Row.GetIndex() + 1}";
    }
}