using Library.IO;
using Library.Widgets;
using System.Diagnostics;

namespace Library.Solvers;
public partial class Evolution {

    // Configuration
    public string Dataset { get; private set; }
    private int[][] m_Data;
    private int m_Generations;
    private int m_Population;
    private double m_MutationRate;
    private SolverBase m_Solver; // local search policy
    public JobSche Result { get; private set; }
    public LogFile LogFile { get; private set; }
    private MatingPoolDelegate MatingPoolMethod { get; set; }
    private CrossoverDelegate CrossoverMethod { get; set; }
    private MutationDelegate MutationMethod { get; set; }
    private EnvironmentSelectionDelegate EnvironmentSelectionMethod { get; set; }

    // Env Selection delegate
    private Evolution() { } // Make the constructor private

    public JobSche Run() {

        Console.Write($"{LogFile.GetExpName()}   ");
        Stopwatch sw = new();
        sw.Start();

        // Init Population
        List<JobSche> population = InitialPopulations();
        Result = population.MinBy(sche => sche.makespan)!;

        // Evolution Start
        using (var progress = new ProgressBar()) {
            for (int i = 0; i < m_Generations; i++) {
                K = i;
                // mating pool
                List<JobSche> matingPool = MatingPoolMethod(population);
                population.Clear();
                for (int t = 0; t < matingPool.Count; t += 2) {
                    var parent1 = matingPool[t];
                    var parent2 = matingPool[t + 1];
                    (JobSche child1, JobSche child2) = CrossoverMethod(parent1, parent2, m_Solver);

                    // mutation
                    if (EvoRandom.Prob() < m_MutationRate) MutationMethod(child1);
                    if (EvoRandom.Prob() < m_MutationRate) MutationMethod(child2);

                    // environment selection 
                    (JobSche sc1, JobSche sc2) = EnvironmentSelectionMethod(parent1, parent2, child1, child2);
                    population.Add(sc1);
                    population.Add(sc2);
                }
                // local-search    
                for (int sc = 0; sc < population.Count; sc++) {
                    JobSche? sche = population[sc];
                    sche = m_Solver.Run(sche); // can apply SA or TS
                }

                WriteLogFile(population);

                var localBest = population.MinBy(sche => sche.makespan)!;
                if (localBest.makespan < Result.makespan) {
                    Result = localBest;
                }
                progress.Report((double)i / m_Generations);
            }
        }

        Console.WriteLine($"   ✅");
        sw.Stop();
        LogFile.Result = Result;
        LogFile.TimeCost = Math.Round(sw.Elapsed.TotalSeconds, 2);
        return Result;
    }

    private List<JobSche> InitialPopulations() {
        // Init Population
        List<JobSche> population = new(m_Population);
        for (int i = 0; i < m_Population; i++) {
            var entity = m_Solver.Run();
            population.Add(entity);
        }
        return population;
    }
    int K = 0;
    private void WriteLogFile(List<JobSche> entities) {
        int[] spans = entities.Select(sc => sc.makespan).ToArray();
        double mean = spans.Average();
        double variance = spans.Sum(number => Math.Pow(number - mean, 2)) / spans.Length;
        double deviation = Math.Sqrt(variance);
        LogFile.meanList.Add(Math.Round(mean, 2));
        LogFile.DeviationList.Add(Math.Round(deviation, 2));
        //Console.WriteLine($"Gen {K + 1, 2} : μ = {mean:F2}, σ = {deviation:F2}"); // Debug
    }
    public void SaveLog(int epoch) {

        LogFile.SaveLog(epoch);

        string expname = LogFile.GetExpName();
        Figure figure = new("Gantt", "makespan", "machine #");
        figure.GanttChart(m_Data, Result);
        figure.SaveFigure($"./Output/figures/{expname}_{epoch}.png");
    }

}