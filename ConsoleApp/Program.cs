using CsvHelper;
using Heuristic;
using System.Diagnostics;
string line = new ('=', 100);


Stopwatch sw = new ();
sw.Start();

string[] datasets = [
 "tai20_5_1.txt",
 "tai20_10_1.txt",
 "tai20_20_1.txt",
 "tai50_5_1.txt",
 "tai50_10_1.txt",
 "tai50_20_1.txt",
 "tai100_5_1.txt",
 "tai100_20_1.txt",
 "tai100_10_1.txt",
];
string[][] ExpResult = new string[datasets.Length + 1][];
for (int i = 0; i < ExpResult.Length; i++) ExpResult[i] = new string[4];
ExpResult[0] = ["", "II", "SA", "TS"];

List<ScheduleSolver> solvers = [];
for (int i = 0; i < datasets.Length; i++) {
    solvers.Add(new ScheduleSolver($"./Dataset/{datasets[i]}", AcceptanceMethod.II, 100));
    ExpResult[i][0] = datasets[i];
    //solvers.Add(new($"./Dataset/{dataset}", AcceptanceMethod.II, 10000));
    //solvers.Add(new($"./Dataset/{dataset}", AcceptanceMethod.II, 10000));
}

for (int i = 1; i < solvers.Count; i++) {
    ScheduleSolver solver = solvers[i];
    solver.Run();
    ExpResult[i][(int)solver.Method] = solver.ResultStr();
    Console.WriteLine($"{solver.ExperienceName} Done");
    Console.WriteLine(line);
}

using (var writer = new StreamWriter("output100.csv"))
using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture)) {
    foreach (var row in ExpResult) {
        foreach (var field in row) {
            csv.WriteField(field);
        }
        csv.NextRecord();
    }
}

sw.Stop();
Console.WriteLine($"{sw.Elapsed.TotalSeconds} seconds");

/*
Problem Definition
M machines, N jobs ...,solution for least time finish these jobs
 
Encoding : permutation-based 1,2,3,...,n
Neighborhood : swap two arbitrary jobs

Test Algorithm:
II, iterative improvement
SA, simulated annealing
TS, tabu search
RA, random search

Result:
Best/Average/Worst-case over 20 runs
computation environment and average computation time

Comparison in solution quality/efficiency/simplicity/robustness

More experiments
1. Run random search
2. large iterative time (may 1,000,000) & recorad local optima
3. different problem set size
4. intensification or diversification which is matter?
Correctness/Clarity/Carefulness/Completeness
 */
