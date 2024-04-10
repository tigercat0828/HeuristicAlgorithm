
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace Library.Solver;
/// <summary>
/// Simulated-Annealing
/// </summary>
/// <param name="data"></param>
public class ScheduleSolverSA : ScheduleSolverBase {
    public float InitTemp { get; }
    public float MinTemp { get; }
    public float Theta { get; }
    private float currentTemp;

    public ScheduleSolverSA(int[][] data, float initTem, float minTemp, float theta) : base(data) {
        // temp = temperature
        InitTemp = initTem;
        MinTemp = minTemp;
        Theta = theta;
    }

    public override JobSche Run(JobSche init = null) {
        JobSche s = init ?? InitialSolution();
        int score = int.MaxValue;

        while(currentTemp > MinTemp) {

            // https://moodle3.ntnu.edu.tw/pluginfile.php/1258102/mod_resource/content/0/02_Trajectory-based%20Search%20I%20%282024%29.pdf
            // p32 

            currentTemp = currentTemp * Theta;
        }
        return new JobSche();
    }
    private JobSche Neighbor(JobSche sche) {
        int a  = random.Next(0, JobNum);
        int b = random.Next(0, JobNum);
        int[] order = sche.order.ToArray();
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

/*
    while T > min_T:
        T = T * theta
        new_arrange = generat(ori_arrange, n)
        new_t = evaluate(array, new_arrange, m, n)
        if new_t < t:
            t = new_t
            ori_arrange = new_arrange
            continue
        span_list.append(t)
        T_list.append(T)
        p = math.exp((t - new_t) / T)
        if random.random() <= p:
            t = new_t
            ori_arrange = new_arrange
            continue
    return t, ori_arrange



 public static double Anneal(double startTemperature, double endTemperature, double coolingRate)
    {
        double temp = startTemperature;
        double currentSolution = random.NextDouble();
        double bestSolution = currentSolution;

        while (temp > endTemperature)
        {
            double newSolution = GetNeighbor(currentSolution);
            double currentEnergy = ObjectiveFunction(currentSolution);
            double newEnergy = ObjectiveFunction(newSolution);

            if (AcceptanceProbability(currentEnergy, newEnergy, temp) > random.NextDouble())
            {
                currentSolution = newSolution;
            }

            if (ObjectiveFunction(currentSolution) < ObjectiveFunction(bestSolution))
            {
                bestSolution = currentSolution;
            }

            // 降低溫度
            temp *= 1 - coolingRate;
        }

        return bestSolution;
    }



 
*/