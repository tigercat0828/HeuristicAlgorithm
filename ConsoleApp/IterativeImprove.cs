﻿namespace Heuristic;
public class IterativeImprove : HeuristicAlgo {
    public IterativeImprove(int[][] data, int[] initOrder = null!) : base(data, initOrder) {
    }
    public override JobOrder Run() {
        SpanList.Clear();
        current = GenerateInitialOrder();
        SpanList.Add(current.makespan);

        int itertime = 0;
        int previousSpan;
        do {
            previousSpan = current.makespan;
            GetNeighbors(current.order);
            Select(neighbors);
            itertime++;
            SpanList.Add(current.makespan);
        } while (current.makespan < previousSpan);
        IterTime = itertime;
        return current;
    }
    protected override int Select(List<int[]> neighbors) {
        // Apply best-improve
        foreach (int[] nei in neighbors) {
            int score = Evaluate(nei);
            if (score < current.makespan) {
                current.makespan = score;
                current.order = nei;
            }
        }
        return current.makespan;
    }
}
