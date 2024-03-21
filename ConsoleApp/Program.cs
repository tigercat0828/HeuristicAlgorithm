using Heuristic;
using System.ComponentModel;
using System.Diagnostics;


Stopwatch sw = new();
sw.Start();
// turn in to batch test
string dataname = "tai20_20_1";
int[][] data = Schedule.LoadFile($"./Dataset/{dataname}.txt");

Schedule origin = new(dataname);
origin.LoadData(data);
origin.SetInitialOrder(Enumerable.Range(0, origin.Jobs - 1).ToArray());
origin.Apply(VariantMethod.II);
List<Schedule> schedules = [origin];

for (int i = 0; i < 100000; i++) {
    Schedule sche = new(dataname);
    sche.LoadData(data);
    sche.Apply(VariantMethod.II);
    schedules.Add(sche);
}

int finish = 0;
Parallel.For(0, schedules.Count, i => {
    schedules[i].Run();
    Interlocked.Increment(ref finish);
    UpdateProgressBar(finish, schedules.Count); 
});
//for (int i = 0; i < schedules.Count; i++) {
//    finish++;
//    schedules[i].Run();
//    UpdateProgressBar(finish, schedules.Count); 
//}
Schedule best = schedules.First();
foreach (var problem in schedules) {
    if(problem.Result.makespan < best.Result.makespan) {
        best = problem;
    }
    //problem.PrintExpResult();
    //problem.WriteExpResult();
}
sw.Stop();
double timecost = sw.Elapsed.TotalSeconds;
Schedule.Plot($"{dataname}_II",data, best.Result.order);       // make figure
Console.WriteLine($"Init Order : [{string.Join(',', best.InitialOrder)}]");
Console.WriteLine($"Best Order : [{string.Join(',', best.Result.order)}]");
Console.WriteLine($"  Makespan : {best.Result.makespan} seconds");
Console.WriteLine($" time cost :{timecost}");

// 更新进度条的方法
void UpdateProgressBar(int current, int total) {
    float progress = (float)current / total * 100;
    //Console.CursorLeft = 0;
    Console.WriteLine($"Progress: {progress:F2}%   ");
}

//string dataname = "tai20_20_1";
//int[][] data = SchedulingProblem.LoadFile($"./Dataset/{dataname}.txt");
//SchedulingProblem problem = new($"{dataname}");
//problem.LoadData(data);
//problem.Apply(VariantMethod.II);
//problem.SetInitialOrder(Enumerable.Range(0, problem.Jobs - 1).ToArray());

//List<SchedulingProblem> problems = [problem];
//for (int i = 0; i < 100; i++) {
//    SchedulingProblem prob = new($"{dataname}");
//    prob.LoadData(data);
//    prob.IterativeImprove();    // iter 100, score 2402
//    prob.WriteExpResult();
//    problems.Add(prob);
//}

//// best
//JobOrder resultOrder = problem.Current;
//problem.PrintExpResult();
//problem.WriteExpResult();
//problem.MeasureAndPlot(resultOrder.order);


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
