namespace Library.Solver;

/// <summary>
/// Solution Entity
/// </summary>
public class JobSche(int[] order, int makespan) {
    public int[] order = [.. order];
    public int makespan = makespan;
    public JobSche() : this([], 0) { }
    public JobSche(int[] order) : this(order, 0) { }
    public JobSche(JobSche other) : this(other.order, other.makespan) { }

    public override string ToString() {
        return $"makespan={makespan}, [{string.Join(", ", order)}]";
    }
    public string GetAnswerString() => string.Join(" ", order);
}
