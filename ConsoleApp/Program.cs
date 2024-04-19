using Library.Configs;
using Library.IO;
using Library.Solvers;
using System.Diagnostics;
using static Library.Solvers.Evolution;
using static Library.Solvers.EvoMethod;
Console.OutputEncoding = System.Text.Encoding.UTF8;
//EntireLog entirelog = new(datasets);


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
    new(100,100,0.001f)     // generation, population, mutationRate
];
MatingPoolDelegate[] matingPoolMethods = [TruncationThreshold50, RouletteWheel, LinearRanking,];
EnvironmentSelectionDelegate[] envSelectionMethods = [GenerationModel, Mechanism_2_4];

// Main Program
// ===============================================================

var expConfigs = BuildAllExpConfigs();

Stopwatch sw = new();
sw.Start();

RUN_DEBUG_EXP(8);
//RUN_ALL_EXPS_SINGLE_THREAD(expConfigs);
//RUN_ALL_EXPS_MULTI_THREAD(expConfigs);   // Console output will be a mess LOL. may use GUI to improve :)

sw.Stop();
Console.WriteLine($"All time cost : {sw.Elapsed.TotalSeconds}");
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
        .Configure(filename, new(10000,1000,0.001))
        .WithData(data)
        .SetMatingPoolMethod(LinearRanking)
        //.SetMatingPoolMethod(RouletteWheel)
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
        Console.WriteLine($"Running {expConfig.Dataset}, Epoch: {epoch}");
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
    }
}
