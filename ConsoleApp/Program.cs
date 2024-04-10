using Library;
using Library.Solver;
using System.Diagnostics;
using System.Net;
string line = new('=', 100);

Stopwatch sw = new();
sw.Start();

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
ScheduleSolverBase solver = new ScheduleSolverII(data);
solver.CheckData();
solver.Run();
Figure figure = new("II", "iteration", "makespan");
figure.ScatterChart(Enumerable.Range(1, solver.SpanList.Count).ToList(), solver.SpanList );
figure.SaveFigure("./Output/convergence.png");
/// Run a single instance II for a dataset
//int[][] data = DataReader.LoadFile($"./Dataset/{datasets[0]}");
//ScheduleSolverBase solver = new ScheduleSolverII(data);
//solver.CheckData();
//solver.RunMultiInstance(10000);
//Figure figure = new("Gantt", "makespan", "machine");
//figure.GanttChart(solver.GetGhattBars(), solver.MachineNum);
//figure.SaveFigure("./Output/output.png");
//Console.WriteLine(solver.Result.ToString());
