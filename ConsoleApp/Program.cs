using Library.Configs;
using Library.IO;
using Library.Solvers;
using System.Diagnostics;
using static Library.Solvers.EvolutionAlgo;
using static Library.Solvers.EvoMethod;
Console.OutputEncoding = System.Text.Encoding.UTF8;


// Experienment Parameters
// ===============================================
const int ROUNDS = 10;
List<string> datasets = [
    "tai20_5_1.txt",
    "tai20_10_1.txt",
    "tai20_20_1.txt",
    "tai50_5_1.txt",
    "tai50_10_1.txt",
    "tai50_20_1.txt",
    "tai100_5_1.txt",
    "tai100_10_1.txt",
    "tai100_20_1.txt",
];

ParamConfig[] paramConfigs = [
   // generation, population, mutationRate
   new(100,100,0.001f),
   //new(300,50,0.001f),
   //new(300,70,0.001f),
   //new(300,100,0.001f),
   //new(300,300,0.001f),
   //new(300,500,0.001f),
   //new(300,700,0.001f),
   //new(300,1000,0.001f),
];
MatingPoolDelegate[] matingPoolMethods = [TruncationThreshold50, RouletteWheel, LinearRanking,];
EnvironmentSelectionDelegate[] envSelectionMethods = [GenerationModel, Mechanism_2_4];
List<ExperimentConfig> expConfigs = BuildAllExpConfigs();
CheckMakeDiretory();
BestSolutions.Load();

List<List<int>> figureData = [];
// Main Program
// ===============================================================
Report report = new(datasets);
Console.WriteLine("Press 's' for single-thread mode.");
Console.WriteLine("Press 'm' for multi-thread mode. (not safe 🥹)");
char userInput = Console.ReadKey().KeyChar;
Console.WriteLine();
Stopwatch sw = new();
sw.Start();
for (int ep = 0; ep < ROUNDS; ep++) {
    figureData.Clear();

    switch (userInput) {
        case 's':
            RUN_ALL_EXPS_SINGLE_THREAD(expConfigs);
            break;
        case 'm':
            RUN_ALL_EXPS_MULTI_THREAD(expConfigs);
            break;
        default:
            RUN_ALL_EXPS_SINGLE_THREAD(expConfigs);
            break;
    }

    Console.WriteLine($"All time cost : {sw.Elapsed.TotalSeconds}");
    report.MakeCSV("./Output/report.csv");
    BestSolutions.Save();
    // Population Overlay Exp Figure
    //var gen = Enumerable.Range(1, 300).ToList();
    //LinePattern[] linePatterns = [LinePattern.Solid, LinePattern.DenselyDashed, LinePattern.Dashed, LinePattern.Dotted, LinePattern.Solid];
    //MarkerShape[] markerStyles = [MarkerShape.OpenCircle, MarkerShape.OpenSquare, MarkerShape.Asterisk, MarkerShape.Cross];
    //string[] labelStrings = ["pop 10", "pop 50", "pop 70", "pop 100", "pop 300"];
    //Figure overlay = new("Pop:10/50/70/100/300", "generation", "makespan (local)");
    //for (int i = 0; i < 5; i++) {
    //    var sca = overlay.Plot.Add.Scatter(gen, figureData[i]);
    //    sca.Label = labelStrings[i];
    //    sca.LinePattern = linePatterns[i];
    //    sca.MarkerStyle = MarkerStyle.None;
    //    //sca.MarkerStyle.Shape = markerStyles[i];
    //    sca.MarkerSize = 5;
    //}
    //overlay.Plot.ShowLegend(Alignment.UpperRight);
    //overlay.SaveFigure($"./Output/PopOverlay_{ep}.png");
}
sw.Stop();
AskOpenOutputFolder();

// Function Area
// =============================================================
void RUN_ALL_EXPS_SINGLE_THREAD(List<ExperimentConfig> expConfigs) {
    foreach (var expConfig in expConfigs) {
        RunExperiment(expConfig, ROUNDS);
    }
}
void RUN_ALL_EXPS_MULTI_THREAD(List<ExperimentConfig> expConfigs) {

    List<Task> tasks = [];
    foreach (var expConfig in expConfigs) {
        tasks.Add(Task.Run(() => RunExperiment(expConfig, ROUNDS)));
    }
    // Wait for all tasks to complete
    Task.WhenAll(tasks).Wait();
}
List<ExperimentConfig> BuildAllExpConfigs() {
    var experimentConfigs = new List<ExperimentConfig>();
    foreach (var dataset in datasets) {
        foreach (var config in paramConfigs) {
            foreach (var matingPoolMethod in matingPoolMethods) {
                foreach (var envSelectionMethod in envSelectionMethods) {
                    experimentConfigs.Add(new ExperimentConfig(dataset, config, matingPoolMethod, envSelectionMethod));
                }
            }
        }
    }
    return experimentConfigs;
}
void RUN_DEBUG_EXP(int datasetNum) {
    string filename = datasets[datasetNum];
    int[][] data = DataReader.LoadFile($"./Dataset/{filename}");
    EvolutionAlgo evo = new Builder()
        .Configure(filename, new(1000, 1000, 0.001))
        .WithData(data)
        .SetMatingPoolMethod(LinearRanking)
        .SetCrossoverMethod(LinearOrderCrossOver)
        .SetMutationMethod(EasySwap)
        .SetEnvironmentSelection(GenerationModel)
        .SetSolver(new SolverSA(22136, 1f, 0.999f))
        .Build();

    evo.Run();
    Console.WriteLine(evo.Result);
    evo.SaveLog(1);
}
void AskOpenOutputFolder() {
    Console.WriteLine("Press space to open the Output folder or exit ...");
    if (Console.ReadKey().Key == ConsoleKey.Spacebar) {
        string outputFolder = Path.Combine(Environment.CurrentDirectory, "Output");
        Process.Start("explorer.exe", outputFolder);
    }
}

void RunExperiment(ExperimentConfig expConfig, int rounds) {
    for (int round = 0; round < rounds; round++) {
        Console.WriteLine($"Running {expConfig.Dataset}, {round}");

        int[][] data = DataReader.LoadFile($"./Dataset/{expConfig.Dataset}");

        // setup the pipeline
        EvolutionAlgo evo = new Builder()
            .Configure(expConfig.Dataset, expConfig.Config)
            .WithData(data)
            .SetMatingPoolMethod(expConfig.MatingPoolMethod)
            .SetCrossoverMethod(LinearOrderCrossOver)   // fixed
            .SetMutationMethod(EasySwap)                // fixed
            .SetEnvironmentSelection(expConfig.EnvSelectionMethod)
            .SetSolver(new SolverSA(22136, 1f, 0.999f))
            .Build();
        evo.Run();
        Console.WriteLine(evo.Result + "\n");
        evo.SaveLog(1 /*epoch*/);
        report.AddLog(expConfig.Dataset, evo.LogFile);
        BestSolutions.CompareAndUpdate(expConfig.Dataset, evo.Result);
        figureData.Add(evo.m_MakespanList);
    }
}
void CheckMakeDiretory() {
    string[] directories = [
        "./Output",
        "./Output/Logs",
        "./Output/Figures/MakespanGantt",
        "./Output/Figures/Convergence"
    ];
    foreach (string directory in directories) {
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
    }
    string filePath = "./Output/BestSolutions.json";
    if (!File.Exists(filePath)) {
        File.Create(filePath).Close();
    }
}
