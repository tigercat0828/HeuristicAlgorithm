
namespace HeuristicAlgorithm.Core.Solvers;
public class SimulatedAnnealing : SolverBase {

    public SimulatedAnnealing(float temperature = 100, float epsilon = 0.001f, float theta = 0.985f) {
        // temp = temperature
        Temperature = temperature;
        currentTemperature = Temperature;
        Epsilon = epsilon;
        Theta = theta;
    }

    public readonly float Epsilon;
    public readonly float Theta;
    public readonly float Temperature;
    private float currentTemperature;
    private readonly Random random = new();

    public override Schedule Run(Schedule? initSolution = null) {
        if (Data is null) throw new InvalidOperationException("Data not loaded.");

        Schedule solution = initSolution ?? InitialSolution();
        Schedule best = solution;
        // cooling tactic
        while (currentTemperature > Epsilon) {
            Schedule neighbor = RandomNeighbor(solution);
            if (neighbor.makespan <= solution.makespan) {
                solution = neighbor;
            }
            else {
                float delta = (solution.makespan - neighbor.makespan) / currentTemperature;
                if (MathF.Exp(delta / currentTemperature) > random.NextSingle()) {
                    solution = neighbor;
                }
            }
            SpanHistory.Add(solution.makespan);
            currentTemperature *= Theta;   // geometry cooldown
            if (solution.makespan <= best.makespan) {
                best = new(solution);
            }
        }
        Result = best;
        return Result;
    }

    private Schedule RandomNeighbor(Schedule sche) {
        int a = random.Next(0, JobNum);
        int b = random.Next(0, JobNum);
        int[] order = [.. sche.order];
        (order[a], order[b]) = (order[b], order[a]);
        int makespan = Evaluate(order);
        Schedule neighbor = new(order, makespan);
        return neighbor;
    }
}
