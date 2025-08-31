namespace HeuristicAlgorithm.Core.Solvers;
public class IterativeImprovement : SolverBase {
    public override Schedule Run(Schedule? initSolution = null) {
        if (Data == null) throw new InvalidOperationException("Dataset not loaded.");

        int previousSpan = int.MaxValue;

        Schedule solution = initSolution ?? InitialSolution();
        while (solution.makespan < previousSpan) {

            previousSpan = solution.makespan;
            SpanHistory.Add(previousSpan);

            List<Schedule> neighbors = Neighbors(solution);
            solution = Select(neighbors, solution);
        }
        return solution;

    }
    public List<Schedule> Neighbors(Schedule sche) {
        // swap all (i,j) 
        List<Schedule> neighbors = [];
        int[] order = sche.order;
        for (int i = 0; i < order.Length; i++) {
            for (int j = i + 1; j < order.Length; j++) {
                int[] temp = [.. order];
                (temp[i], temp[j]) = (temp[j], temp[i]);
                Schedule nei = new(temp, Evaluate(temp));
                neighbors.Add(nei);
            }
        }
        return neighbors;
    }
    private Schedule Select(List<Schedule> neighbors, Schedule localBest) {
        foreach (var sche in neighbors) {
            if (sche.makespan < localBest.makespan) {
                localBest = sche;
            }
        }
        return localBest;
    }
}
