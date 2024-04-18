using Library.Solver;
using System.Net;
using System.Transactions;

namespace Library;
public static class EvolutionMethod {

    #region Mating Pool 
    public static List<JobSche> TruncationThreshold50(List<JobSche> group) {
        var selected =  group.OrderBy(sche => sche.makespan).Take(group.Count/2).ToArray();
        var repeat = RepeatElementsTwice(selected);
        ShuffleArray(repeat);
        return [.. repeat];

        static JobSche[] RepeatElementsTwice(JobSche[] origin) {
            JobSche[] repeatedArray = new JobSche[origin.Length * 2];
            int index = 0;

            foreach (var item in origin) {
                repeatedArray[index++] = item;
                repeatedArray[index++] = item;
            }

            return repeatedArray;
        }

        static void ShuffleArray(JobSche[] array) {

            for (int i = array.Length - 1; i > 0; i--) {
                int j = EvoRandom.Next(0, i + 1);
                (array[j], array[i])=(array[i], array[j]);
            }
        }

    }
    public static List<JobSche> RouletteWheel(List<JobSche> group) {
        int maxSpan = group.MaxBy(sc=>sc.makespan)!.makespan;
        int[] spans = group.Select(sc => maxSpan - sc.makespan).ToArray();
        double total = spans.Sum();
        List<double> probabilities = spans.Select(s => s / total).ToList();

        List<JobSche> matingPool = new(group.Count);
        for (int t = 0; t < group.Count; t++) {
            double prob = EvoRandom.Prob();
            double cumulativeProb = 0;
            for (int i = 0; i < probabilities.Count; i++) {
                cumulativeProb += probabilities[i];
                if (prob <= cumulativeProb) {
                    matingPool.Add(new JobSche(group[i]));
                    break;
                }
            }
        }
        return matingPool;
    }
    public static List<JobSche> LinearRanking(List<JobSche> groups) {
        double minP = 0.0;
        double maxP = 2.0;
        var sorted =  groups.OrderByDescending(sc => sc.makespan).ToList();
        int n = sorted.Count;
        List<double> probTable = [];
        for (int i = 0; i < n; i++) {
            int rank = i+1;
            double prob = minP/n+(maxP-minP)*(rank-1)/n/(n-1);        
            probTable.Add(prob); 
        }
        Console.WriteLine(probTable.Sum());
        // check sum =1;
        List<JobSche> matingPool = [];
        for (int t = 0; t < n; t++) {
            double randomProb = EvoRandom.Prob();
            double cumulativeProb = 0;
            for (int i = 0; i < n; i++) {
                cumulativeProb += probTable[i];
                if (randomProb <= cumulativeProb) {
                    matingPool.Add(new JobSche(groups[i]));
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

        // Find the second lowest by comparing the higher of the lowest pair and the lower of the higher pair
        JobSche secondLowest;
        if (lowest == low1) {
            secondLowest = low2.makespan < high1.makespan ? low2 : high1;
        }
        else {
            secondLowest = low1.makespan < high2.makespan ? low1 : high2;
        }

        return (lowest, secondLowest);
    }
    #endregion
}
