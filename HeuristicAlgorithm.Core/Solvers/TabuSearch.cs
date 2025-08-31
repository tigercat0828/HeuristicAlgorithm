namespace HeuristicAlgorithm.Core.Solvers;
public class TabuSearch : SolverBase {
    private readonly int tabuTenure;
    private readonly int maxIter;

    private readonly Queue<(int, int)> tabuQueue = new();
    private readonly HashSet<(int, int)> tabuSet = new();

    public TabuSearch(int tabuTenure = 10, int maxIter = 500) {
        this.tabuTenure = tabuTenure;
        this.maxIter = maxIter;
    }

    public override Schedule Run(Schedule? init = null) {
        if (Data == null) throw new InvalidOperationException("Dataset not loaded.");

        Schedule current = init ?? InitialSolution();
        Schedule best = current;

        for (int iter = 0; iter < maxIter; iter++) {
            List<(Schedule sche, (int, int) move)> neighbors = Neighbors(current);

            Schedule candidate = default;   // 預設值
            (int, int) chosenMove = (0, 0);
            bool found = false;

            foreach (var (sche, move) in neighbors) {
                bool inTabu = tabuSet.Contains(move);

                // Aspiration criterion
                if (!inTabu || sche.makespan < best.makespan) {
                    if (!found || sche.makespan < candidate.makespan) {
                        candidate = sche;
                        chosenMove = move;
                        found = true;
                    }
                }
            }

            if (!found) break; // 這一輪沒有合法鄰居 → 結束

            current = candidate;
            if (current.makespan < best.makespan) {
                best = current;
            }

            // 更新 tabu list
            tabuQueue.Enqueue(chosenMove);
            tabuSet.Add(chosenMove);

            if (tabuQueue.Count > tabuTenure) {
                var oldest = tabuQueue.Dequeue();
                tabuSet.Remove(oldest);
            }

            SpanHistory.Add(best.makespan);
        }

        return best;
    }

    private List<(Schedule, (int, int))> Neighbors(Schedule sche) {
        List<(Schedule, (int, int))> neighbors = new();
        int[] order = sche.order;

        for (int i = 0; i < order.Length; i++) {
            for (int j = i + 1; j < order.Length; j++) {
                int[] temp = [.. order];
                (temp[i], temp[j]) = (temp[j], temp[i]);
                Schedule nei = new(temp, Evaluate(temp));
                neighbors.Add((nei, (i, j)));
            }
        }
        return neighbors;
    }
}
