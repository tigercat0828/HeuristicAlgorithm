using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeuristicAlgorithm.Core;
using HeuristicAlgorithm.Core.Evolution;
using HeuristicAlgorithm.Core.Solvers;
using HeuristicAlgorithm.WPF.Models;
using HeuristicAlgorithm.WPF.Utilities;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;

namespace HeuristicAlgorithm.WPF.ViewModels;
public partial class MainWindowViewModel : ObservableObject {

    public MainWindowViewModel() {
        SelectedAlgorithm = AlgorithmOptions.First();
    }

    public int[][] Dataset = null!;
    public string DatasetName => Path.GetFileNameWithoutExtension(StatusString);
    public IReadOnlyList<int> SpanHistory;

    [ObservableProperty] private DataTable _dataTable;

    public ObservableCollection<ComboOption<AlgoParam>> AlgorithmOptions { get; } = [
        new ComboOption<AlgoParam>("II (Iterative Improvement)", new ParamII()),
        new ComboOption<AlgoParam>("SA (Simulated Annealing)", new ParamSA()),
        new ComboOption<AlgoParam>("TS (Tabu Search)", new ParamTS()),
        new ComboOption<AlgoParam>("GA (Genetic Algorithm)", new ParamGA()),
    ];

    [ObservableProperty] private AlgoParam? _selectedParam;
    [ObservableProperty] private ComboOption<AlgoParam>? _selectedAlgorithm;
    partial void OnSelectedAlgorithmChanged(ComboOption<AlgoParam>? value) {
        SelectedParam = value?.Value;
    }

    [ObservableProperty] private string _statusString = "load dataset file first ...";

    [RelayCommand(CanExecute = nameof(CanRunAlgo))] private void RunSelectedAlgorithm() {
        switch (SelectedParam) {
            case ParamII:
                IterativeImprovement();
                break;
            case ParamSA:
                SimulatedAnnealing();
                break;
            case ParamTS:
                TabuSearch();
                break;
            case ParamGA:
                GeneticAlgorithm();
                break;
            default:
                MessageBox.Show("請選擇一個演算法");
                break;
        }
    }
    private bool CanRunAlgo() => Dataset != null;

    [RelayCommand] private void OpenFile() {
        var dialog = new OpenFileDialog {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };
        bool result = dialog.ShowDialog() ?? false;
        if (result) {
            Dataset = DatasetReader.LoadFile(dialog.FileName);
            StatusString = dialog.FileName;
        }
        DataTable = ToDataGrid(Dataset);
        RunSelectedAlgorithmCommand.NotifyCanExecuteChanged();
    }
    [RelayCommand] private void Exit() {
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

    private void ShowResultDiagramWindow(string title, int makespan, int[] order, int[] history) {
        // show diagram.xaml and set owner to mainwindow
        var diagram = new Views.Diagram(title, makespan, order, history) {
            Owner = Application.Current.MainWindow
        };
        diagram.Show();
    }
    private void IterativeImprovement() {
        var solverII = new IterativeImprovement();
        solverII.LoadDataset(Dataset);
        var schedule = solverII.Run();
        ShowResultDiagramWindow(DatasetName, schedule.makespan, schedule.order, [.. solverII.SpanHistory]);
    }
    private void SimulatedAnnealing() {
        var param = SelectedParam as ParamSA;
        var solverSA = new SimulatedAnnealing(param.Temperature, param.Epsilon, param.Theta);
        solverSA.LoadDataset(Dataset);
        var schedule = solverSA.Run();
        ShowResultDiagramWindow(DatasetName, schedule.makespan, schedule.order, [.. solverSA.SpanHistory]);
    }
    private void TabuSearch() {
        var param = SelectedParam as ParamTS;
        var solverTS = new TabuSearch(param.Tenure, param.MaxIter);
        solverTS.LoadDataset(Dataset);
        var schedule = solverTS.Run();
        ShowResultDiagramWindow(DatasetName, schedule.makespan, schedule.order, [.. solverTS.SpanHistory]);
    }
    private void GeneticAlgorithm() {
        var param = SelectedParam as ParamGA;

        var builder = new EvolutionAlgo.Builder();
        builder
            .WithData(Dataset)
            .Configure(param.Generations, param.Population, param.MutationRate)
            .SetSolver(new IterativeImprovement())
            .SetMatingPoolMethod(param.SelectedMatingPool!.Value)
            .SetCrossoverMethod(param.SelectedCrossover!.Value)
            .SetMutationMethod(param.SelectedMutation!.Value)
            .SetEnvironmentSelection(param.SelectedSelection!.Value);
        var ga = builder.Build();
        var result = ga.Run();
        ShowResultDiagramWindow(DatasetName, result.makespan, result.order, [.. ga.SpanHistory]);
    }
}
