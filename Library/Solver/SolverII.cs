namespace Library.Solver;

/// <summary>
/// Iterative-Improvement
/// </summary>
public class SolverII() : SolverBase() {
    /// <summary>
    /// II process
    /// </summary>
    public override JobSche Run(JobSche init = null) {
        EnsureDataLoaded();
        int previousSpan = int.MaxValue;
        JobSche solution = init ?? InitialSolution();
        while (solution.makespan < previousSpan) {

            previousSpan = solution.makespan;
            SpanList.Add(previousSpan);

            List<JobSche> neighbors = Neighbors(solution);
            solution = Select(neighbors, solution);
        }
        return solution;
    }
    private List<JobSche> Neighbors(JobSche sche) {
        // swap all (i,j) 
        List<JobSche> neighbors = [];
        int[] order = sche.order;

        for (int i = 0; i < order.Length; i++) {
            for (int j = i + 1; j < order.Length; j++) {
                int[] temp = [.. order];
                (temp[i], temp[j]) = (temp[j], temp[i]);

                JobSche newSche = new(temp, Evaluate(temp));
                neighbors.Add(newSche);
            }
        }
        return neighbors;
    }
    /// <summary>
    /// acceptance criteria : Best-improving
    /// </summary>
    private JobSche Select(List<JobSche> neighbors, JobSche localBest) {

        foreach (var sche in neighbors) {
            if (sche.makespan < localBest.makespan) {
                localBest = sche;
            }
        }
        return localBest;
    }
    /// <summary>
    /// Run multiple II instance parallely
    /// </summary>
    public JobSche RunMultiInstance(int instance) {
        List<Func<JobSche, JobSche>> instances = new(instance);
        List<JobSche> locals = new(instance);

        for (int i = 0; i < instance; i++) {
            instances.Add(Run);
            locals.Add(null!);
        }

        Parallel.For(0, instance, i => {
            locals[i] = instances[i].Invoke(null!);
        });
        Result = locals.MinBy(order => order.makespan)!;
        return Result;
    }
}
