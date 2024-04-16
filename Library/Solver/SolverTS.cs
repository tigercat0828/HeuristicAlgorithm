
namespace Library.Solver;
/// <summary>
/// Tabu-Search
/// </summary>
public class SolverTS(int[][] data) : SolverBase(data) {
    public override JobSche Run(JobSche init = null) {
        throw new NotImplementedException();
    }

    public override JobSche RunMultiInstance(int instance) {
        throw new NotImplementedException();
    }

    protected override JobSche Select(List<JobSche> neighbors, JobSche localBest) {
        throw new NotImplementedException();
    }
}
