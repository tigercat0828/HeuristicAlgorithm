namespace Library.Solvers;
public static class EvoMethod {

    #region Mating Pool 
    public static List<JobSche> TruncationThreshold50(List<JobSche> group) {
        JobSche[] candidates = group.OrderBy(sche => sche.makespan).Take(group.Count / 2).ToArray();
        JobSche[] repeat = [.. candidates, .. candidates];
        ShuffleArray(repeat);
        return [.. repeat];

        static void ShuffleArray(JobSche[] array) {

            for (int i = array.Length - 1; i > 0; i--) {
                int j = EvoRandom.Next(0, i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
        }
    }

    public static List<JobSche> RouletteWheel(List<JobSche> population) {
        int maxSpan = population.MaxBy(sc => sc.makespan)!.makespan;
        int[] spans = population.Select(sc => maxSpan - sc.makespan).ToArray();

        double total = spans.Sum();
        if (total == 0) {
            return new(population);
        }

        List<double> probTable = spans.Select(s => s / total).ToList();

        List<JobSche> matingPool = new(population.Count);
        for (int t = 0; t < population.Count; t++) {
            double prob = EvoRandom.Prob();
            double cumulativeProb = 0;
            for (int i = 0; i < probTable.Count; i++) {
                cumulativeProb += probTable[i];
                if (prob <= cumulativeProb) {
                    matingPool.Add(new JobSche(population[i]));
                    break;
                }
            }
        }

        return matingPool;
    }
    public static List<JobSche> LinearRanking(List<JobSche> population) {
        double minP = 0.0;
        double maxP = 2.0;
        var sorted = population.OrderByDescending(sc => sc.makespan).ToList();
        int n = sorted.Count;
        double[] probTable = new double[n];
        for (int i = 0; i < n; i++) {
            int rank = i + 1;
            double prob = minP / n + (maxP - minP) * (rank - 1) / (n * n - n);
            probTable[i] = prob;
        }
        // Debug : check sum =1 and same different and prob diff is same;
        // Console.WriteLine(probTable.Sum()); for (int i = 0; i < probTable.Count-1; i++) Console.WriteLine(probTable[i+1]-probTable[i]);

        List<JobSche> matingPool = [];
        for (int t = 0; t < n; t++) {
            double randomProb = EvoRandom.Prob();
            double cumulativeProb = 0;
            for (int i = 0; i < n; i++) {
                cumulativeProb += probTable[i];
                if (randomProb <= cumulativeProb) {
                    matingPool.Add(new JobSche(sorted[i]));
                    break;
                }
            }
        }
        return matingPool;
    }

    #endregion

    #region Crossover
    public static (JobSche, JobSche) LinearOrderCrossOver(JobSche parent1, JobSche parent2, SolverBase solver) {
        int length = parent1.order.Length;
        int[] childOrder1 = Enumerable.Repeat(-1, length).ToArray();
        int[] childOrder2 = Enumerable.Repeat(-1, length).ToArray();

        // Choose two random crossover points as the slice
        int start = EvoRandom.Next(length);
        int end = EvoRandom.Next(start, length);

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
        int i = EvoRandom.Next(jobs);
        int j = EvoRandom.Next(jobs);
        (entity.order[i], entity.order[j]) = (entity.order[j], entity.order[i]);
    }
    #endregion

    #region Environment Selection
    /// <summary>
    /// just return children
    /// </summary>
    public static (JobSche, JobSche) GenerationModel(JobSche parent1, JobSche parent2, JobSche child1, JobSche child2) {

        return (child1, child2);
    }

    /// <summary>
    /// 2 parents, 2 children, pick best 2 entity
    /// </summary>
    public static (JobSche, JobSche) Mechanism_2_4(JobSche parent1, JobSche parent2, JobSche child1, JobSche child2) {

        // Compare the first two and second two numbers
        var (low1, high1) = parent1.makespan < parent2.makespan ? (parent1, parent2) : (parent2, parent1);
        var (low2, high2) = child1.makespan < child2.makespan ? (child1, child2) : (child2, child1);

        // Find the lowest of the four numbers
        var lowest = low1.makespan < low2.makespan ? low1 : low2;

        // Find the second lowest
        JobSche secondLowest;

        if (lowest == low1) {
            // If the lowest is from the first pair, compare high1 (the higher of the first pair) and low2 (the lower of the second pair)
            secondLowest = high1.makespan < low2.makespan ? high1 : low2;
        }
        else {
            // If the lowest is from the second pair, compare low1 (the lower of the first pair) and high2 (the higher of the second pair)
            secondLowest = low1.makespan < high2.makespan ? low1 : high2;
        }
        return (lowest, secondLowest);
    }
    #endregion
}
