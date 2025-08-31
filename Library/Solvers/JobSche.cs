namespace Library.Solvers;

/// <summary>
/// Solution Entity
/// </summary>
public class JobSche {
    public JobSche() {
        this.Order = [];
        this.Makespan = 0;
    }
    public JobSche(int[] order, int makespan) {
        this.Order = [.. order];
        this.Makespan = makespan;
    }
    public JobSche(JobSche other) {
        this.Order = [.. other.Order];
        this.Makespan = other.Makespan;
    }
    public int Makespan;
    public int[] Order;
    public string orderjsonstr => string.Join(" ", Order);

    public override string ToString() {
        return $"makespan={Makespan}, [{string.Join(", ", Order)}]";
    }
}
