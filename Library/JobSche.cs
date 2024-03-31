namespace Library;
public class JobSche {
    public int[] order;
    public int makespan;
    public JobSche() {

    }
    public JobSche(int[] order, int makespan) {
        this.order = [.. order];
        this.makespan = makespan;
    }

    public override string ToString() {
        return $"makespan={makespan}, [{string.Join(", ", order)}]";
    }
    public string GetAnswerString() => string.Join(", ", order);


}
