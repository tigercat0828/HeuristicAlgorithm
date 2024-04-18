using Library;
using Library.Solver;
using System.Diagnostics;
using System.Text.Json;

string[] datasets = [
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
Stopwatch sw = new();

string filename = datasets[0];      // <------ assign dataset here 
int[][] data = DataReader.LoadFile($"./Dataset/{filename}");
Evolution evo = new Evolution.Builder().Configure(filename, 30, 100, 0.001)
                                       .WithData(data)
                                        //.SetInitSolutions()                                               // IIinit
                                       .SetMatingPoolMethod(EvolutionMethod.TruncationThreshold50)          // TruncationThreshold50|RouletteWheel|LinearRanking
                                       .SetCrossoverMethod(EvolutionMethod.LinearOrderCrossOver)            // LOX
                                       .SetMutationMethod(EvolutionMethod.EasySwap)                         // EasySwap
                                       .SetEnvironmentSelection(EvolutionMethod.GenerationModel)            // GenerationModel|Mechanism_2_4
                                       .SetSolver(new SolverII())   // II  
                                       .SetSolver(new SolverSA(100,0.001f,0.985f))   // SA   
                                       .Build();

evo.Run();
Console.WriteLine(evo.Result);
evo.SaveLog(1);

