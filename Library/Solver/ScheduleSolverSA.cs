
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace Library.Solver;
/// <summary>
/// Simulated-Annealing
/// </summary>
/// <param name="data"></param>
public class ScheduleSolverSA : ScheduleSolverBase {

    public readonly float Epsilon;
    public readonly float Theta;
    public readonly float Temperature;
    private float currentTemperature;


    public ScheduleSolverSA(int[][] data, float temperature, float epsilon, float theta) : base(data) {
        // temp = temperature
        Temperature = temperature;
        currentTemperature = Temperature;
        Epsilon = epsilon;
        Theta = theta;
    }

    public override JobSche Run(JobSche init = null) {
        JobSche solution = init ?? InitialSolution();

        // cooling tactic
        while(currentTemperature > Epsilon) {
            JobSche neighbor = Neighbor(solution);
            if(neighbor.makespan <= solution.makespan) {
                solution = neighbor;
            }
            else {
                float delta = (solution.makespan - neighbor.makespan)/ currentTemperature;
                if(MathF.Exp(delta/ currentTemperature) > random.NextSingle()) {
                    solution = neighbor;
                }
            }
            SpanList.Add(solution.makespan);
            currentTemperature *= Theta;   // geometry cooldown
        }
        return solution;
    }
    /// <summary>
    /// random generate a single neightbor of sche
    /// </summary>
    private JobSche Neighbor(JobSche sche) {
        int a  = random.Next(0, JobNum);
        int b = random.Next(0, JobNum);
        int[] order = [.. sche.order];
        (order[a], order[b]) = (order[b], order[a]);
        int makespan = Evaluate(order);
        JobSche neighbor = new (order, makespan);
        return neighbor;
    }
    public override JobSche RunMultiInstance(int instance) {
        throw new NotImplementedException();
    }

    protected override JobSche Select(List<JobSche> neighbors, JobSche localBest) {
        throw new NotImplementedException();
    }
}

