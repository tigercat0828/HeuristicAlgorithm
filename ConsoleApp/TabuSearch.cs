using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heuristic;

// https://www.geeksforgeeks.org/what-is-tabu-search/
public class TabuSearch : HeuristicAlgo {
    int TABU_SIZE = 3;
    int EPOCH = 50;

    public TabuSearch(int[][] data, int[] initOrder = null) : base(data, initOrder) {
    }

    public override JobOrder Run() {
        SpanList.Clear();
        current = GenerateInitialOrder();
        SpanList.Add(current.makespan);

        for (int i = 0; i < EPOCH; i++) {
            GetNeighbors(current.order);
            Select(neighbors);
            SpanList.Add(current.makespan);
        }
        IterTime = EPOCH;
        return current;
    }
    HashSet<int> TabuList = []; // for find in O(1)
    Queue<int> TabuQueue = [];
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
