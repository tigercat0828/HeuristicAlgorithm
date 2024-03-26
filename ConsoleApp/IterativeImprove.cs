namespace Heuristic;
public class IterativeImprove : HeuriAlgo {
    public IterativeImprove(int[][] data, int[] initOrder = null!) : base(data, initOrder) {
    }
    public override JobOrder Run() {
        Spans.Clear();
        current = GenerateInitialOrder();
        Spans.Add(current.makespan);

        int itertime = 0;
        int previousSpan;
        do {
            previousSpan = current.makespan;
            GetNeighbors(current.order);
            Select(neighbors);
            itertime++;
            Spans.Add(current.makespan);
        } while (current.makespan != previousSpan);
        IterTime = itertime;
        return current;
    }
}
