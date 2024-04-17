using Library;
using Library.Solver;

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

int[][] data = DataReader.LoadFile($"./Dataset/{datasets[0]}");

Evolution evo = new Evolution.Builder().Configure("tai100_20_1", 30, 12, 4, 0.001)
                                       .WithData(data)
                                        //.SetInitSolutions()                                               // IIinit
                                       .SetMatingPoolMethod(EvolutionMethod.Truncation)                     // Truncation|RouletteWheel|LinearRanking|ExponentialRanking
                                       .SetCrossoverMethod(EvolutionMethod.LinearOrderCrossOver)            // LOX
                                       .SetMutationMethod(EvolutionMethod.EasySwap)                         // EasySwap
                                       .SetEnvironmentSelection(EvolutionMethod.GenerationModel)            // generational model|2/4mechanism
                                       .SetSolver(new SolverSA(100,0.001f,0.985f))   // SA   
                                       //.SetSolver(new SolverII())   // II  
                                       .Build();
var result = evo.Run();
Console.WriteLine(result);
Figure figure = new("Gantt", "makespan", "machine #");

figure.GanttChart(data, result);
figure.SaveFigure("./Output/Gantt.png");

