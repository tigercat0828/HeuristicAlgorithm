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
//SolverBase solver = new SolverSA(data, 1000, 0.001f, 0.99f);
//solver.CheckData();
//solver.Run();
//Console.WriteLine(solver.Result.ToString());
//Figure figure = new("II", "iterations", "makespan");
//figure.ScatterChart(Enumerable.Range(1, solver.SpanList.Count).ToList(), solver.SpanList);
//figure.SaveFigure("./Output/convergence.png");

ScheduleEvolutionTemp temp = new(
    data,
    100,
    1000,
    100,
    new SolverSA(data, 1000, 0.001f, 0.99f));


ScheduleEvolution evo = new ScheduleEvolution.Builder().WithData(data)
                                                       .SetSolver(new SolverII())
                                                       .SetCrossoverMethod(EvoAlgo.LinearCrossOver)
                                                       .SetMatingPoolMethod(EvoAlgo.TopRatio)
                                                       .SetParentSelectionMethod(EvoAlgo.RandomSelect)
                                                       .Build();
