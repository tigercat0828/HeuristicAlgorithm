namespace Library.Solver;

/// <summary>
/// Iterative-Improvement
/// </summary>
/// <param name="data"></param>
public class ScheduleSolverII(int[][] data) : ScheduleSolverBase(data) {
    /// <summary>
    /// Run multiple II instance parallely
    /// </summary>
    public override JobSche Run(int instance) {
        List<Func<JobSche>> instances = new(instance);
        List<JobSche> locals = new(instance);

        for (int i = 0; i < instance; i++) {
            instances.Add(II);
            locals.Add(null!);
        }
        // single thread for debug, we can set instance to 1
        //for (int i = 0; i < instance; i++) locals[i] = instances[i].Invoke();
    
        Parallel.For(0, instance, i => {
            locals[i] = instances[i].Invoke();
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
    protected JobSche II() {
        int previousSpan = int.MaxValue;
        JobSche s = InitialSolution();
        while (s.makespan < previousSpan  ) {
            previousSpan = s.makespan;
            List<JobSche> neighbors = Neighbors(s);
            s = Select(neighbors, s);
        }
        return s;
    }
}
