using HeuristicAlgorithm.Core.Solvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeuristicAlgorithm.Core.Evolution; 
public static class EvoMethod {

    #region Mating Pool 
    public static List<Schedule> TruncationThreshold50(List<Schedule> group) {
        Schedule[] candidates = group.OrderBy(sche => sche.makespan).Take(group.Count / 2).ToArray();
        Schedule[] repeat = [.. candidates, .. candidates];
        ShuffleArray(repeat);
        return [.. repeat];

        static void ShuffleArray(Schedule[] array) {

            for (int i = array.Length - 1; i > 0; i--) {
                int j = EvoRandom.Next(0, i + 1);
                (array[j], array[i]) = (array[i], array[j]);
            }
        }
    }
    public static List<Schedule> RouletteWheel(List<Schedule> population) {
        int maxSpan = population.MaxBy(sc => sc.makespan)!.makespan;
        int[] spans = population.Select(sc => maxSpan - sc.makespan).ToArray();

        double total = spans.Sum();
        if (total == 0) {
            return new(population);
        }

        List<double> probTable = spans.Select(s => s / total).ToList();

        List<Schedule> matingPool = new(population.Count);
        for (int t = 0; t < population.Count; t++) {
            double prob = EvoRandom.Prob();
            double cumulativeProb = 0;
            for (int i = 0; i < probTable.Count; i++) {
                cumulativeProb += probTable[i];
                if (prob <= cumulativeProb) {
                    matingPool.Add(new Schedule(population[i]));
                    break;
                }
            }
        }

        return matingPool;
    }
    public static List<Schedule> LinearRanking(List<Schedule> population) {
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

        List<Schedule> matingPool = [];
        for (int t = 0; t < n; t++) {
            double randomProb = EvoRandom.Prob();
            double cumulativeProb = 0;
            for (int i = 0; i < n; i++) {
                cumulativeProb += probTable[i];
                if (randomProb <= cumulativeProb) {
                    matingPool.Add(new Schedule(sorted[i]));
                    break;
                }
            }
        }
        return matingPool;
    }

    #endregion

    #region Crossover
    public static (Schedule, Schedule) LinearOrderCrossOver(Schedule parent1, Schedule parent2, SolverBase solver) {
        int length = parent1.order.Length;
        int[] childOrder1 = [.. Enumerable.Repeat(-1, length)];
        int[] childOrder2 = [.. Enumerable.Repeat(-1, length)];

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
        Schedule child1 = new(childOrder1, solver.Evaluate(childOrder1));
        Schedule child2 = new(childOrder2, solver.Evaluate(childOrder2));
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

    public static void EasySwap(Schedule entity) {
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
    public static (Schedule, Schedule) GenerationModel(Schedule parent1, Schedule parent2, Schedule child1, Schedule child2) {

        return (child1, child2);
    }

    /// <summary>
    /// 2 parents, 2 children, pick best 2 entity
    /// </summary>
    public static (Schedule, Schedule) Mechanism_2_4(Schedule parent1, Schedule parent2, Schedule child1, Schedule child2) {

        // Compare the first two and second two numbers
        var (low1, high1) = parent1.makespan < parent2.makespan ? (parent1, parent2) : (parent2, parent1);
        var (low2, high2) = child1.makespan < child2.makespan ? (child1, child2) : (child2, child1);

        // Find the lowest of the four numbers
        var lowest = low1.makespan < low2.makespan ? low1 : low2;

        // Find the second lowest
        bool fromFirstPair = lowest.makespan == low1.makespan;
        Schedule secondLowest;

        if (fromFirstPair)
            secondLowest = high1.makespan < low2.makespan ? high1 : low2;
        else
            secondLowest = low1.makespan < high2.makespan ? low1 : high2;

        return (lowest, secondLowest);
    }
    #endregion
}
