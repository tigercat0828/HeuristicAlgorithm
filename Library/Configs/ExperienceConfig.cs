using static Library.Solvers.Evolution;
namespace Library.Configs;

public struct ExperimentConfig(
    string dataset,
    ParamConfig config,
    MatingPoolDelegate matingPoolMethod,
    EnvironmentSelectionDelegate envSelectionMethod) {

    public string Dataset = dataset;
    public ParamConfig Config = config;
    public MatingPoolDelegate MatingPoolMethod = matingPoolMethod;
    public EnvironmentSelectionDelegate EnvSelectionMethod = envSelectionMethod;
}
