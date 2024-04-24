using Library.Configs;
using Library.IO;
using Library.Solvers;
using System.Diagnostics;
using static Library.Solvers.Evolution;
using static Library.Solvers.EvoMethod;
Console.OutputEncoding = System.Text.Encoding.UTF8;


// Experienment Parameters
// ===============================================
const int EPOCH = 20;
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
    // compare generation
    //new(150,100,0.001f),
    //new(200,100,0.001f),
    //new(250,100,0.001f),
    //new(300,100,0.001f),
    //new(350,100,0.001f),
    //new(350,100,0.001f),

    // compare population
    //new(150, 50,0.001f),
    //new(200,100,0.001f),
    //new(250,200,0.001f),
    //new(300,250,0.001f),
    //new(350,300,0.001f),
    //new(350,350,0.001f),
];
MatingPoolDelegate[] matingPoolMethods = [TruncationThreshold50, RouletteWheel, LinearRanking,];
EnvironmentSelectionDelegate[] envSelectionMethods = [GenerationModel, Mechanism_2_4];
List<ExperimentConfig> expConfigs = BuildAllExpConfigs();
CheckDiretory();
BestSolutions.Load();
// Main Program
// ===============================================================
Report report = new(datasets);
Console.WriteLine("Press 's' for single-thread mode.");
Console.WriteLine("Press 'm' for multi-thread mode. (not safe 🥹)");
char userInput = Console.ReadKey().KeyChar;
Console.WriteLine();
Stopwatch sw = new();
sw.Start();
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
sw.Stop();
Console.WriteLine($"All time cost : {sw.Elapsed.TotalSeconds}");
report.MakeCSV("./Output/report.csv");
BestSolutions.Save();
AskOpenOutputFolder();


// Function Area
// =============================================================
void RUN_ALL_EXPS_SINGLE_THREAD(List<ExperimentConfig> expConfigs) {
    foreach (var expConfig in expConfigs) {
        RunExperiment(expConfig, EPOCH);
    }
}
void RUN_ALL_EXPS_MULTI_THREAD(List<ExperimentConfig> expConfigs) {

    List<Task> tasks = [];
    foreach (var expConfig in expConfigs) {
        tasks.Add(Task.Run(() => RunExperiment(expConfig, EPOCH)));
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
    Evolution evo = new Builder()
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
        System.Diagnostics.Process.Start("explorer.exe", outputFolder);
    }

}
void RunExperiment(ExperimentConfig expConfig, int epochCount) {
    for (int epoch = 0; epoch < epochCount; epoch++) {
        Console.WriteLine($"Running {expConfig.Dataset}, {epoch}");
        int[][] data = DataReader.LoadFile($"./Dataset/{expConfig.Dataset}");

        // setup the pipeline
        Evolution evo = new Builder()
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
        evo.SaveLog(epoch);
        report.AddLog(expConfig.Dataset ,evo.LogFile);
        BestSolutions.CompareAndUpdate(expConfig.Dataset, evo.Result);
    }
}
void CheckDiretory() {
    if (!Directory.Exists("./Output")) {
        Directory.CreateDirectory("./Output");
    }
    if (!File.Exists("./Output/BestSolutions.json")) {
        File.Create("./Output/BestSolutions.json");
    }
    if (!Directory.Exists("./Output/logs")) {
        Directory.CreateDirectory("./Output/logs");
    }
    if (!Directory.Exists("./Output/figures")) {
        Directory.CreateDirectory("./Output/figures");
    }
}
