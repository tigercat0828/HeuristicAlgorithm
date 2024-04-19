namespace Library.Solvers;
/// <summary>
/// Simulated-Annealing
/// </summary>
public class SolverSA : SolverBase {

    public readonly float Epsilon;
    public readonly float Theta;
    public readonly float Temperature;
    private float currentTemperature;

    public SolverSA(float temperature, float epsilon, float theta) {
        // temp = temperature
        Temperature = temperature;
        currentTemperature = Temperature;
        Epsilon = epsilon;
        Theta = theta;
    }

    public override JobSche Run(JobSche init = null!) {
        EnsureDataLoaded();
        JobSche solution = init ?? InitialSolution();
        JobSche best = solution;
        // cooling tactic
        while (currentTemperature > Epsilon) {
            JobSche neighbor = Neighbor(solution);
            if (neighbor.makespan <= solution.makespan) {
                solution = neighbor;
            }
            else {
                float delta = (solution.makespan - neighbor.makespan) / currentTemperature;
                if (MathF.Exp(delta / currentTemperature) > random.NextSingle()) {
                    solution = neighbor;
                }
            }
            SpanList.Add(solution.makespan);
            currentTemperature *= Theta;   // geometry cooldown
            if (solution.makespan <= best.makespan) {
                best = new(solution);
            }
        }
        Result = best;
        return Result;
    }
    /// <summary>
    /// random generate a single neightbor of sche
    /// </summary>
    private JobSche Neighbor(JobSche sche) {
        int a = random.Next(0, JobNum);
        int b = random.Next(0, JobNum);
        int[] order = [.. sche.order];
        (order[a], order[b]) = (order[b], order[a]);
        int makespan = Evaluate(order);
        JobSche neighbor = new(order, makespan);
        return neighbor;
    }
}

