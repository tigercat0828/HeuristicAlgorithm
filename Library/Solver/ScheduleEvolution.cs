namespace Library.Solver;
/// <summary>
/// Solve schedule problem with genetic evolution
/// </summary>
public class ScheduleEvolution {
    private static readonly Random random = new();
    /// <summary>
    /// select the local search algorithm with passing solver class. II/SA/TS/RND
    /// </summary>
    /// <param name="solver"></param>
    public ScheduleEvolution(int[][] data, int generations, int population, int poolSize, SolverBase solver) {
        Generations = generations;
        Population = population;
        PoolSize = poolSize;
        Data = data;
        this.solver = solver;
    }
    public readonly SolverBase solver;    // local search policy
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
                (JobSche parent1, JobSche parent2) = SelectParents(pool);
                // cross-over
                (JobSche children1, JobSche children2) = CrossOver(parent1, parent2);

                // POLICY : replace all parents : p
                groups.Add(children1);
                groups.Add(children2);
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
    /// <summary>
    /// POLICY : randomly pick two entity from the pool
    /// </summary>
    protected (JobSche, JobSche) SelectParents(List<JobSche> pool) {

        int indexA = random.Next(pool.Count);
        int indexB = random.Next(pool.Count);
        while (indexB == indexA) {
            indexB = random.Next(pool.Count);
        }
        return (pool[indexA], pool[indexB]);
    }
    /// <summary>
    /// POLICY : Perform LOX (Linear Order Crossover)
    /// </summary>
    /// <returns>Two newborn children</returns>
    protected (JobSche, JobSche) CrossOver(JobSche parent1, JobSche parent2) {
        int length = parent1.order.Length;
        int[] childOrder1 = new int[length];
        int[] childOrder2 = new int[length];

        // Choose two random crossover points as the slice
        int start = random.Next(length);
        int end = random.Next(start, length);
        // Initialize children with -1 to indicate unfilled positions
        for (int i = 0; i < length; i++) {
            childOrder1[i] = -1;
            childOrder2[i] = -1;
        }

        // Copy a slice from each parent based on the crossover points
        for (int i = start; i <= end; i++) {
            childOrder1[i] = parent1.order[i];
            childOrder2[i] = parent2.order[i];
        }

        FillChildWithRemainingElements(parent2.order, childOrder1);
        FillChildWithRemainingElements(parent1.order, childOrder2);
        JobSche child1 = new (childOrder1, solver.Evaluate(childOrder1));
        JobSche child2 = new (childOrder2, solver.Evaluate(childOrder2));
        return ( child1, child2);

        static void FillChildWithRemainingElements(int[] parent, int[] child) {
            int length = parent.Length;
            int curPos = 0;
            for (int i = 0; i < length; i++) {
                if (!child.Contains(parent[i])) {       // we can speed up search with new a set
                                                        // Find the next unfilled position in the child
                    while (child[curPos] != -1) curPos++;

                    child[curPos] = parent[i];
                }
            }
        }
    }
}
