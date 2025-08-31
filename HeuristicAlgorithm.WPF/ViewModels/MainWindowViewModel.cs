using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeuristicAlgorithm.Core;
using HeuristicAlgorithm.Core.Solvers;
using HeuristicAlgorithm.WPF.Models;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Windows;

namespace HeuristicAlgorithm.WPF.ViewModels;
public partial class MainWindowViewModel : ObservableObject {

    public MainWindowViewModel() {
        SelectedAlgorithm = AlgorithmType.IterativeImprovement;
        SelectedParam = new ParamII();
    }

    public int[][] Dataset = null!;
    public string DatasetName => Path.GetFileNameWithoutExtension(StatusString);
    public IReadOnlyList<int> SpanHistory;

    [ObservableProperty] private DataTable _dataTable;
    [ObservableProperty] private AlgoParam? _selectedParam = null;
    [ObservableProperty] private AlgorithmType _selectedAlgorithm = AlgorithmType.IterativeImprovement;
    partial void OnSelectedAlgorithmChanged(AlgorithmType value) {

        SelectedParam = value switch {
            AlgorithmType.IterativeImprovement => new ParamII(),
            AlgorithmType.SimulatedAnnealing => new ParamSA(),
            AlgorithmType.TabuSearch => new ParamTS(),
            _ => null
        };
    }

    [ObservableProperty] private string _statusString = "load dataset file first ...";


    [RelayCommand(CanExecute = nameof(CanRunAlgo))]
    private void IterativeImprovement() {
        var solverII = new IterativeImprovement();
        solverII.LoadDataset(Dataset);
        var schedule = solverII.Run();
        ShowResultDiagramWindow(DatasetName, schedule.makespan, schedule.order, [.. solverII.SpanHistory]);
    }
    [RelayCommand(CanExecute = nameof(CanRunAlgo))]
    private void SimulatedAnnealing() {
        var param = SelectedParam as ParamSA;
        var solverSA = new SimulatedAnnealing(param.Temperature, param.Epsilon, param.Theta);
        solverSA.LoadDataset(Dataset);
        var schedule = solverSA.Run();
        ShowResultDiagramWindow(DatasetName, schedule.makespan, schedule.order, [.. solverSA.SpanHistory]);
    }
    [RelayCommand(CanExecute = nameof(CanRunAlgo))]
    private void TabuSearch() {
        var param = SelectedParam as ParamTS;
        var solverTS = new TabuSearch(param.Tenure, param.MaxIter);
        solverTS.LoadDataset(Dataset);
        var schedule = solverTS.Run();
        ShowResultDiagramWindow(DatasetName, schedule.makespan, schedule.order, [.. solverTS.SpanHistory]);
    }

    private bool CanRunAlgo() => Dataset != null;


    [RelayCommand]
    private void OpenFile() {
        var dialog = new OpenFileDialog {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };
        bool result = dialog.ShowDialog() ?? false;
        if (result) {
            Dataset = DatasetReader.LoadFile(dialog.FileName);
            StatusString = dialog.FileName;
        }
        DataTable = ToDataGrid(Dataset);

        IterativeImprovementCommand.NotifyCanExecuteChanged();
        SimulatedAnnealingCommand.NotifyCanExecuteChanged();
        TabuSearchCommand.NotifyCanExecuteChanged();
    }
    [RelayCommand]
    private void Exit() {
        Application.Current.Shutdown();
    }


    //============================================

    private static DataTable ToDataGrid(int[][] data) {

        var table = new DataTable();
        int rows = data.Length;
        int cols = data[0].Length;
        for (int c = 0; c < cols; c++) {
            table.Columns.Add($"J{c + 1}", typeof(int));
        }

        for (int r = 0; r < rows; r++) {
            var row = table.NewRow();
            for (int c = 0; c < cols; c++) {
                row[c] = data[r][c];
            }
            table.Rows.Add(row);
        }
        return table;
    }

    public enum AlgorithmType {
        IterativeImprovement,
        SimulatedAnnealing,
        TabuSearch,
        GeneticAlgorithm
    }
    public Array AlgorithmTypes => Enum.GetValues<AlgorithmType>();

    private void ShowResultDiagramWindow(string title, int makespan, int[] order, int[] history) {
        // show diagram.xaml and set owner to mainwindow
        var diagram = new Views.Diagram(title, makespan, order, history) {
            Owner = Application.Current.MainWindow
        };
        diagram.Show();
    }
}
