using Library;
using Library.Solver;


public class Evolution {

    // Configuration
    public string ExpName { get; set; }
    private int[][] m_Data;
    private int m_Generations;
    private int m_Population;
    private int m_PoolSize;
    private double m_MutationRate;

    private SolverBase m_Solver; // local search policy
    private MatingPoolDelegate MatingPool { get; set; }
    private CrossoverDelegate Crossover { get; set; }
    private MutationDelegate Mutation { get; set; }
    private EnvironmentSelectionDelegate EnvironmentSelection { get; set; }

    // Env Selection delegate
    private Evolution() { } // Make the constructor private
    public JobSche Run() {
        List<JobSche> groups = new(m_Population);
        for (int i = 0; i < m_Population; i++) {
            JobSche job = m_Solver.InitialSolution();
            groups.Add(job);
        }
        
        for (int i = 0; i < m_Generations; i++) {
            int[] spans = groups.Select(sc => sc.makespan).ToArray();
            double mean = spans.Average();
            double variance = spans.Sum(number => Math.Pow(number - mean, 2)) / spans.Length;
            double stdDeviation = Math.Sqrt(variance);

            Console.WriteLine($"Gen {i} : μ = {mean:F2}, σ = {stdDeviation}");           
            // mating pool
            List<JobSche> pool = MatingPool(groups);

            for (int t = 0; t < pool.Count; t++) {
                (JobSche child1, JobSche child2) = Crossover(parent1, parent2, m_Solver);

                // mutation
                if (EvoRandom.Prob() < m_MutationRate) Mutation(child1);
                if (EvoRandom.Prob() < m_MutationRate) Mutation(child2);

                // environment selection 
                (JobSche sc1, JobSche sc2) = EnvironmentSelection(parent1, parent2, child1, child2);
                groups.Add(sc1);
                groups.Add(sc2);
            }


            groups.Clear();
            for (int t = 0; t < m_Population; t += 2) {
                
                // selectparent
                (JobSche parent1, JobSche parent2) = SelectParent(pool);
                // cross-over
                (JobSche child1, JobSche child2) = Crossover(parent1, parent2, m_Solver);

                // mutation
                if (EvoRandom.Prob() < m_MutationRate) Mutation(child1);
                if (EvoRandom.Prob() < m_MutationRate) Mutation(child2);

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
        }
        return groups.MinBy(sche => sche.makespan)!;
    }

    static (JobSche, JobSche) SelectParent(List<JobSche> pool) {
        int indexA = EvoRandom.Next(pool.Count);
        int indexB = EvoRandom.Next(pool.Count);
        while (indexB == indexA) {
            indexB = EvoRandom.Next(pool.Count);
        }
        return (pool[indexA], pool[indexB]);
    }
    // ====================================================================================
    // Builder-Pattern
    public delegate List<JobSche> MatingPoolDelegate(List<JobSche> groups);
    public delegate (JobSche, JobSche) CrossoverDelegate(JobSche parent1, JobSche parent2, SolverBase solver);
    public delegate (JobSche, JobSche) EnvironmentSelectionDelegate(JobSche parent1, JobSche parent2, JobSche childe2,JobSche child2);
    public delegate void MutationDelegate(JobSche sche);
    public class Builder {
        bool hasData = false;
        private Evolution _instance = new();

        public Builder WithData(int[][] data) {
            _instance.m_Data = data;
            hasData = true;
            return this;
        }
        public Builder Configure(string expName, int generations, int population, int poolSize, double mutationRate) {
            _instance.ExpName = expName;
            _instance.m_Generations = generations;
            _instance.m_Population = population;
            _instance.m_PoolSize = poolSize;
            _instance.m_MutationRate = mutationRate;
            return this;
        }
        public Builder SetSolver(SolverBase solver) {
            if (!hasData) {
                throw new InvalidOperationException("should load data before set solver");
            }
            _instance.m_Solver = solver;
            _instance.m_Solver.SetData(_instance.m_Data);
            return this;
        }
        public Builder SetMatingPoolMethod(MatingPoolDelegate matingPool) {
            _instance.MatingPool = matingPool;
            return this;
        }
        public Builder SetCrossoverMethod(CrossoverDelegate crossover) {
            _instance.Crossover = crossover;
            return this;
        }
        public Builder SetMutationMethod(MutationDelegate mutation) {
            _instance.Mutation = mutation;
            return this;
        }
        public Builder SetEnvironmentSelection(EnvironmentSelectionDelegate environmentSelection) {
            _instance.EnvironmentSelection = environmentSelection;
            return this;
        }
        public Evolution Build() {
            return _instance;
        }
    }
}
