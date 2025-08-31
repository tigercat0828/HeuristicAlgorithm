using CommunityToolkit.Mvvm.ComponentModel;

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
