using Library.Solver;

namespace Library;
public static class EvolutionMethod {

    #region Mating Pool 
    public static List<JobSche> Ntournament(List<JobSche> groups, int take) {
        return groups.OrderBy(sche => sche.makespan).Take(take).ToList();
    }
    #endregion

    #region Parent Selection
    public static (JobSche, JobSche) RandomSelect(List<JobSche> pool) {
        int indexA = EvoRandom.Next(pool.Count);
        int indexB = EvoRandom.Next(pool.Count);
        while (indexB == indexA) {
            indexB = EvoRandom.Next(pool.Count);
        }
        return (pool[indexA], pool[indexB]);
    }
    #endregion

    #region Crossover
    public static (JobSche, JobSche) LinearOrderCrossOver(JobSche parent1, JobSche parent2, SolverBase solver) {
        int length = parent1.order.Length;
        int[] childOrder1 = new int[length];
        int[] childOrder2 = new int[length];

        // Choose two random crossover points as the slice
        int start = EvoRandom.Next(length);
        int end = EvoRandom.Next(start, length);
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
        JobSche child1 = new(childOrder1, solver.Evaluate(childOrder1));
        JobSche child2 = new(childOrder2, solver.Evaluate(childOrder2));
        return (child1, child2);

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
    #endregion

    #region Mutation
    public static void EasySwap(JobSche entity) {
        int jobs = entity.order.Length;
        int a = EvoRandom.Next(jobs);
        int b = EvoRandom.Next(jobs);
        (entity.order[a], entity.order[b]) = (entity.order[b], entity.order[a]);
    }
    #endregion

}
