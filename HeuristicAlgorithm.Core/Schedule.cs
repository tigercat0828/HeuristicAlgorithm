namespace HeuristicAlgorithm.Core;
public struct Schedule {
    public int[] order;     // the job order
    public int makespan;    // the time cost of this order

    public Schedule() {
        order = [];
        makespan = 0;
    }
    public Schedule(int[] order, int makespan) {
        this.order = [.. order];
        this.makespan = makespan;
    }
    public Schedule(Schedule other) {
        this.order = [.. other.order];
        this.makespan = other.makespan;
    }
    public override string ToString() {
        return $"{makespan} [{string.Join(", ", order)}]";
    }
}
