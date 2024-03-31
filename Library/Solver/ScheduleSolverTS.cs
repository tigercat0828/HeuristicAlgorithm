namespace Library.Solver;
/// <summary>
/// Tabu-Search
/// </summary>
public class ScheduleSolverTS(int[][] data) : ScheduleSolverBase(data) {
    public override JobSche Run(int rounds = 1) {
        throw new NotImplementedException();
    }

    protected override JobSche Select(List<JobSche> neighbors, JobSche localBest) {
        throw new NotImplementedException();
    }
}
