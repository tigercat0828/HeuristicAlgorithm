
namespace Library.Solver;
/// <summary>
/// Simulated-Annealing
/// </summary>
/// <param name="data"></param>
public class ScheduleSolverSA(int[][] data) : ScheduleSolverBase(data) {
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
