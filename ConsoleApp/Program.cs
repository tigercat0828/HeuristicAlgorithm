using CsvHelper;
using Heuristic;

string[][] values =
       [
            ["empty", "II", "SA", "TS"],
            ["data1", "v11", "v12", "v13"],
            ["data2", "v21", "v22", "v23"],
            // Add more data rows as needed
            ["datan", "vn1", "vn2", "vn3"]
       ];

using (var writer = new StreamWriter("output.csv"))
using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
{
    foreach (var row in values)
    {
        foreach (var field in row)
        {
            csv.WriteField(field);
        }
        csv.NextRecord();
    }
}

//Console.WriteLine("CSV file created successfully using CsvHelper.");

//ScheduleSolver singleTest = new("./Dataset/tai20_20_1.txt", AcceptanceMethod.II, 30000);
//singleTest.Run();

//string[] datasets = [
// "tai100_10_1.txt",
// "tai100_20_1.txt",
// "tai100_5_1.txt",
// "tai20_10_1.txt",
// "tai20_20_1.txt",
// "tai20_5_1.txt",
// "tai50_10_1.txt",
// "tai50_20_1.txt",
// "tai50_5_1.txt"
//];
//List<ScheduleSolver> solvers = [];
//foreach (var dataset in datasets)
//{
//    ScheduleSolver II = new($"./Dataset/{dataset}", AcceptanceMethod.II, 10000);
//    ScheduleSolver SA = new($"./Dataset/{dataset}", AcceptanceMethod.SA, 10000);
//    ScheduleSolver TS = new($"./Dataset/{dataset}", AcceptanceMethod.TS, 10000);
//    solvers.Add(II);
//    solvers.Add(SA);
//    solvers.Add(TS);
//}

//string[][] ExpsResult = new string[datasets.Length +1][];
//ExpsResult[0] = ["", "II","SA", "TS"];

//for (int i = 0; i < solvers.Count; i++)
//{
//    ScheduleSolver solver = solvers[i];
//    solver.Run();
//    ExpsResult[i][(int)solver.Method] = solver.ResultStr();
//}
//using (var writer = new StreamWriter("output.csv"))
//using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
//{
//    foreach (var row in ExpsResult)
//    {
//        csv.WriteRecord(row);
//    }
//}
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
