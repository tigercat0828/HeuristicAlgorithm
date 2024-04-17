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
    private ParentSelectionDelegate ParentSelection { get; set; }
    private CrossoverDelegate Crossover { get; set; }
    private MutationDelegate Mutation { get; set; }

    // Env Selection delegate
    private Evolution() { } // Make the constructor private
    public JobSche Run() {
        List<JobSche> groups = new(m_Population);
        for (int i = 0; i < m_Population; i++) {
            JobSche job = m_Solver.InitialSolution();
            groups.Add(job);
        }
        for (int i = 0; i < m_Generations; i++) {
            Console.WriteLine($"Gen {i}");
            // mating pool
            List<JobSche> pool = MatingPool(groups, m_PoolSize);

            groups.Clear();
            for (int t = 0; t < m_Population; t += 2) {
                // selectparent
                (JobSche parent1, JobSche parent2) = ParentSelection(pool);
                // cross-over
                (JobSche children1, JobSche children2) = Crossover(parent1, parent2, m_Solver);

                // replace all parents
                groups.Add(children1);
                groups.Add(children2);
            }
            // mutation the current population
            for (int t = 0; t < groups.Count; t++) {
                if (EvoRandom.Prob() < m_MutationRate) {
                    Mutation(groups[i]);
                    Console.WriteLine("Mutate!");
                }
            }

            // local-search    
            for (int sc = 0; sc < groups.Count; sc++) {
                JobSche? sche = groups[sc];
                sche = m_Solver.Run(sche); // can apply SA or TS
            }
        }
        return groups.MinBy(sche => sche.makespan)!;
    }
    // ====================================================================================
    // Builder-Pattern
    public delegate List<JobSche> MatingPoolDelegate(List<JobSche> groups, int take);
    public delegate (JobSche, JobSche) ParentSelectionDelegate(List<JobSche> pool);
    public delegate (JobSche, JobSche) CrossoverDelegate(JobSche parent1, JobSche parent2, SolverBase solver);
    //public delegate (JobSche, JobSche) ChooseSubstitutionDelegate(JobSche A, JobSche B, JobSche C,JobSche D);
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
        public Builder SetParentSelectionMethod(ParentSelectionDelegate parentSelection) {
            _instance.ParentSelection = parentSelection;
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

        public Evolution Build() {
            return _instance;
        }
    }
}
