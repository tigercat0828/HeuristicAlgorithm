namespace Library.Solver;
/// <summary>
/// Tabu-Search
/// </summary>
public class SolverTS() : SolverBase {
    public override JobSche Run(JobSche init = null) {
        EnsureDataLoaded();
        JobSche solution = init ?? InitialSolution();
        throw new NotImplementedException();
    }

}
