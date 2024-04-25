namespace Library.Solvers;

/// <summary>
/// Solution Entity
/// </summary>
public class JobSche {
    public JobSche(int[] order, int makespan) {
        this.order = [.. order];
        this.makespan = makespan;
    }

    public int makespan { get; set; }
    public int[] order { get; set; }
    public string orderjsonstr => string.Join(" ", order);
    public JobSche() : this([], 0) { }
    public JobSche(int[] order) : this(order, 0) { }
    public JobSche(JobSche other) : this(other.order, other.makespan) { }

    public override string ToString() {
        return $"makespan={makespan}, [{string.Join(", ", order)}]";
    }
    public string GetAnswerString() => string.Join(" ", order);
}
