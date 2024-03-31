namespace Heuristic;
public class SimulateAnnealing : HeuristicAlgo {

    public SimulateAnnealing(int[][] data, int[] initOrder = null) : base(data, initOrder) {
    }

    public override JobOrder Run() {
        throw new NotImplementedException();
    }

    protected override int Select(List<int[]> neighbors) {
        throw new NotImplementedException();
    }
}
