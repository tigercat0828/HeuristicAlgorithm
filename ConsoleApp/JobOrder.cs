using static System.Formats.Asn1.AsnWriter;

namespace Heuristic;
public class JobOrder(int[] orders, int score) {
    public int[] order = orders;
    public int makespan = score;
    public JobOrder(JobOrder other) : this([.. other.order], other.makespan) { }

}



