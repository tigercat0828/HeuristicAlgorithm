using Library.Solver;
namespace Library;
public class ScheduleEvolution {
    private static readonly Random random = new();
    public ScheduleEvolution(int[][] data, int generations, int population, int poolSize, ScheduleSolverBase solver) {
        Generations = generations;
        Population = population;
        PoolSize = poolSize;
        Data = data;
        this.solver = solver;
    }
    public readonly ScheduleSolverBase solver;    // iterative improve
    public readonly int[][] Data;
    public readonly int Generations;
    public readonly int Population;
    public readonly int PoolSize;

    public JobSche Evolution() {
        List<JobSche> groups = new(Population);
        for (int i = 0; i < Population; i++) {
            JobSche job = solver.InitialSolution();
            groups.Add(job);
        }
        for (int i = 0; i < Generations; i++) {

            // mating pool
            List<JobSche> pool = MatingPool(groups);
            groups.Clear();
            for (int t = 0; t < Population; t += 2) {
                // selectparent
                (JobSche A, JobSche B) = PickParents(pool);
                // cross-over
                (JobSche C, JobSche D) = CrossOver(A, B);
                groups.Add(C);
                groups.Add(D);
            }
            // local-search    
            for (int sc = 0; sc < groups.Count; sc++) {
                JobSche? sche = groups[sc];
                sche = solver.Run(sche); // can apply SA or TS
            }
        }
        return groups.MaxBy(sche => sche.makespan)!;
    }
    protected List<JobSche> MatingPool(List<JobSche> groups) {
        return groups.OrderBy(sche => sche.makespan).Take(PoolSize).ToList();
    }
    protected (JobSche, JobSche) PickParents(List<JobSche> pool) {
        
        int indexA = random.Next(pool.Count);
        int indexB = random.Next(pool.Count);
        while (indexB == indexA) {
            indexB = random.Next(pool.Count);
        }
        return (pool[indexA], pool[indexB]);
    }
    protected (JobSche, JobSche) CrossOver(JobSche A, JobSche B) {
        return (B, A);
    }
}
