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
            if (!TabuList.Contains(score)) {
                if (score < current.makespan) {
                    current.makespan = score;
                    current.order = nei;
                }
            }
            
            
        }
        return current.makespan;
    }
}


/*
  foreach (List<int> neighbor in neighbors)
        {
            if (!tabu_list.Contains(neighbor))
            {
                int neighbor_fitness = ObjectiveFunction(neighbor);
                if (neighbor_fitness < best_neighbor_fitness)
                {
                    best_neighbor = neighbor;
                    best_neighbor_fitness = neighbor_fitness;
                }
            }
        }
        if (best_neighbor.Count == 0)
        {
              // No non-tabu neighbors found,
            // terminate the search
            break;
        }
        current_solution = best_neighbor;
        tabu_list.Add(best_neighbor);
        if (tabu_list.Count > tabu_list_size)
        {
              // Remove the oldest entry from the
            // tabu list if it exceeds the size
            tabu_list.RemoveAt(0);
        }
        if (ObjectiveFunction(best_neighbor) < ObjectiveFunction(best_solution))
        {
              // Update the best solution if the
            // current neighbor is better
            best_solution = best_neighbor;
        }
 */