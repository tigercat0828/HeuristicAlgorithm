namespace Library.Solver;

/// <summary>
/// Iterative-Improvement
/// </summary>
public class ScheduleSolverII(int[][] data) : ScheduleSolverBase(data) {
    /// <summary>
    /// II process
    /// </summary>
    public override JobSche Run(JobSche init = null!) {
        int previousSpan = int.MaxValue;
        JobSche s = init ?? InitialSolution();
        while (s.makespan < previousSpan) {
            previousSpan = s.makespan;
            List<JobSche> neighbors = Neighbors(s);
            s = Select(neighbors, s);
        }
        return s;
    }
    /// <summary>
    /// Run multiple II instance parallely
    /// </summary>
    public override JobSche RunMultiInstance(int instance) {
        List<Func<JobSche, JobSche>> instances = new(instance);
        List<JobSche> locals = new(instance);

        for (int i = 0; i < instance; i++) {
            instances.Add(Run);
            locals.Add(null!);
        }

        Parallel.For(0, instance, i => {
            locals[i] = instances[i].Invoke(null!);
        });
        Result = locals.MaxBy(order => order.makespan)!;
        return Result;
    }
   
    /// <summary>
    /// acceptance criteria : Best-improving
    /// </summary>
    protected override JobSche Select(List<JobSche> neighbors, JobSche localBest) {

        foreach (var sche in neighbors) {
            if (sche.makespan < localBest.makespan) {
                localBest = sche;
            }
        }
        return localBest;
    }

}
