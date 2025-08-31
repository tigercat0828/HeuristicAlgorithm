using CommunityToolkit.Mvvm.ComponentModel;
using HeuristicAlgorithm.Core.Evolution;
using HeuristicAlgorithm.WPF.Utilities;
using System.Collections.ObjectModel;
using static HeuristicAlgorithm.Core.Evolution.EvolutionAlgo;
namespace HeuristicAlgorithm.WPF.Models;
public class AlgoParam : ObservableObject {

}
public class ParamII : AlgoParam {

}
public partial class ParamSA : AlgoParam {
    [ObservableProperty] float _temperature = 100;
    [ObservableProperty] float _epsilon = 0.001f;
    [ObservableProperty] float _theta = 0.985f;
}
public partial class ParamTS : AlgoParam {
    [ObservableProperty] int _tenure = 10;
    [ObservableProperty] int _maxIter = 500;
}

public partial class ParamGA : AlgoParam {

    public ParamGA() {
        SelectedMatingPool = MatingPoolOptions.First();
        SelectedCrossover = CrossoverOptions.First();
        SelectedMutation = MutationOptions.First();
        SelectedSelection = EnvSelectionOptions.First();
    }

    [ObservableProperty] int _generations = 100;
    [ObservableProperty] int _population = 100;
    [ObservableProperty] float _mutationRate = 0.01f;

    [ObservableProperty] private ComboOption<MatingPoolDelegate>? _selectedMatingPool;
    [ObservableProperty] private ComboOption<CrossoverDelegate>? _selectedCrossover;
    [ObservableProperty] private ComboOption<MutationDelegate>? _selectedMutation;
    [ObservableProperty] private ComboOption<EnvironmentSelectionDelegate>? _selectedSelection;

    public ObservableCollection<ComboOption<MatingPoolDelegate>> MatingPoolOptions { get; } = [
        new("Truncation 50%", EvoMethod.TruncationThreshold50),
        new("Roulette Wheel", EvoMethod.RouletteWheel),
        new("Linear Ranking", EvoMethod.LinearRanking),
    ];
    public ObservableCollection<ComboOption<CrossoverDelegate>> CrossoverOptions { get; } = [
        new("LinearOrderCrossover", EvoMethod.LinearOrderCrossOver),
    ];
    public ObservableCollection<ComboOption<MutationDelegate>> MutationOptions { get; } = [
        new("EasySwap", EvoMethod.EasySwap),
    ];
    public ObservableCollection<ComboOption<EnvironmentSelectionDelegate>> EnvSelectionOptions { get; } = [
        new("Generation Model", EvoMethod.GenerationModel),
        new("Mechanism 2-4", EvoMethod.Mechanism_2_4),
    ];
}