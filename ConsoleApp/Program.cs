using Library;
using Library.Solver;
using System.Diagnostics;
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



/// Run a single instance II for a dataset
int[][] data = DataReader.LoadFile($"./Dataset/{datasets[0]}");
ScheduleSolverBase solver = new ScheduleSolverII(data);
solver.CheckData();
solver.RunMultiInstance(10000);
Figure figure = new("Gantt", "makespan", "machine");
figure.GanttChart(solver.GetGhattBars(), solver.MachineNum);
figure.SaveFigure("./Output/output.png");
Console.WriteLine(solver.Result.ToString());
