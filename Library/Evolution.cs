using Library.Solver;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library; 
public class Evolution {

    // Configuration
    public string Dataset { get; private set; }
    public string ExpName { get; private set; }
    private int[][] m_Data;
    private int m_Generations;
    private int m_Population;
    private double m_MutationRate;
    public JobSche Result { get; private set; }
    public ExpLog m_LogFile { get; private set; }

    private SolverBase m_Solver; // local search policy
    private MatingPoolDelegate MatingPool { get; set; }
    private CrossoverDelegate Crossover { get; set; }
    private MutationDelegate Mutation { get; set; }
    private EnvironmentSelectionDelegate EnvironmentSelection { get; set; }

    // Env Selection delegate
    private Evolution() { } // Make the constructor private
    public JobSche Run() {

        Stopwatch sw = new();
        sw.Start();
        List<JobSche> groups = new(m_Population);
        for (int i = 0; i < m_Population; i++) {
            JobSche job = m_Solver.InitialSolution();
            groups.Add(job);
        }
        Result = groups.MinBy(sche => sche.makespan)!;
        for (int i = 0; i < m_Generations; i++) {
            // mating pool
            List<JobSche> pool = MatingPool(groups);
            groups.Clear();
            for (int t = 0; t < pool.Count; t+=2) {
                var parent1 = pool[t];
                var parent2 = pool[t+1];
                (JobSche child1, JobSche child2) = Crossover(parent1, parent2, m_Solver);

                // mutation
                if (EvoRandom.Prob() < m_MutationRate) { Mutation(child1); /* Console.WriteLine("mutate");*/ }
                if (EvoRandom.Prob() < m_MutationRate) { Mutation(child2); /* Console.WriteLine("mutate");*/ }

                // environment selection 
                (JobSche sc1, JobSche sc2) = EnvironmentSelection(parent1, parent2, child1, child2);
                groups.Add(sc1);
                groups.Add(sc2);
            }

            // local-search    
            for (int sc = 0; sc < groups.Count; sc++) {
                //Console.WriteLine($"processing {sc}");
                JobSche? sche = groups[sc];
                sche = m_Solver.Run(sche); // can apply SA or TS
            }

            int[] spans = groups.Select(sc => sc.makespan).ToArray();
            double mean = spans.Average();
            double variance = spans.Sum(number => Math.Pow(number - mean, 2)) / spans.Length;
            double deviation = Math.Sqrt(variance);
            Console.WriteLine($"Gen {i+1,2} : μ = {mean:F2}, σ = {deviation:F2}");
            m_LogFile.meanList.Add(Math.Round(mean, 2));
            m_LogFile.DeviationList.Add(Math.Round(deviation, 2));

            var localBest = groups.MinBy(sche => sche.makespan)!;
            if(localBest.makespan < Result.makespan) {
                Result = localBest;
            }
        }
        sw.Stop();
        m_LogFile.Result = Result;
        m_LogFile.TimeCost = Math.Round(sw.Elapsed.TotalSeconds,2);
        return Result;
    }

    public void SaveLog() {
        m_LogFile.SaveLog();

        string expname = m_LogFile.GetExpName();
        Figure figure = new("Gantt", "makespan", "machine #");
        figure.GanttChart(m_Data, Result);
        figure.SaveFigure($"./Output/{expname}.png");
    }


    // ====================================================================================
    // Builder-Pattern
    public delegate List<JobSche> MatingPoolDelegate(List<JobSche> groups);
    public delegate (JobSche, JobSche) CrossoverDelegate(JobSche parent1, JobSche parent2, SolverBase solver);
    public delegate (JobSche, JobSche) EnvironmentSelectionDelegate(JobSche parent1, JobSche parent2, JobSche childe2, JobSche child2);
    public delegate void MutationDelegate(JobSche sche);
    public class Builder {
        bool hasData = false;
        private Evolution _instance = new();

        public Builder WithData(int[][] data) {
            _instance.m_Data = data;
            hasData = true;
            return this;
        }
        public Builder Configure(string filename, int generations, int population, double mutationRate) {
            _instance.Dataset = filename;
            _instance.m_Population = population;
            _instance.m_Generations = generations;
            _instance.m_MutationRate = mutationRate;
            _instance.m_LogFile = new(filename, generations, population, mutationRate);
            return this;
        }
        public Builder SetSolver(SolverBase solver) {
            if (!hasData) {
                throw new InvalidOperationException("should load data before set solver");
            }
            _instance.m_LogFile.LocalSearchMethod = solver.GetType().Name;
            _instance.m_Solver = solver;
            _instance.m_Solver.SetData(_instance.m_Data);
            return this;
        }
        public Builder SetMatingPoolMethod(MatingPoolDelegate matingPool) {
            _instance.m_LogFile.MatingPoolMethod = matingPool.Method.Name;
            _instance.MatingPool = matingPool;
            return this;
        }
        public Builder SetCrossoverMethod(CrossoverDelegate crossover) {
            _instance.m_LogFile.CrossOverMethod = crossover.Method.Name;
            _instance.Crossover = crossover;
            return this;
        }
        public Builder SetMutationMethod(MutationDelegate mutation) {
            _instance.m_LogFile.MutationMethod = mutation.Method.Name;
            _instance.Mutation = mutation;
            return this;
        }
        public Builder SetEnvironmentSelection(EnvironmentSelectionDelegate environmentSelection) {
            _instance.m_LogFile.EnvironmentMethod = environmentSelection.Method.Name;
            _instance.EnvironmentSelection = environmentSelection;
            return this;
        }
        public Evolution Build() {

            return _instance;
        }
    }
}