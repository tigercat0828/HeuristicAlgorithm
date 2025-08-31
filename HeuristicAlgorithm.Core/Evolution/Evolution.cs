using HeuristicAlgorithm.Core.Solvers;
using System.Diagnostics;

namespace HeuristicAlgorithm.Core.Evolution; 
public class EvolutionAlgo {
    // Configuration
    private int[][] _dataset;
    private int _generations;
    private int _population;
    private double _mutationRate;
    private SolverBase _solver; // local search policy
    public List<int> SpanHistory = [];
    public Schedule Result { get; private set; }
    private MatingPoolDelegate MatingPoolMethod { get; set; }
    private CrossoverDelegate CrossoverMethod { get; set; }
    private MutationDelegate MutationMethod { get; set; }
    private EnvironmentSelectionDelegate EnvironmentSelectionMethod { get; set; }
    private EvolutionAlgo() { } // Make the constructor private
    public Schedule Run() {

        Stopwatch sw = new();

        // Init Population
        List<Schedule> population = InitialPopulations();
        Result = population.MinBy(sche => sche.makespan)!;
        // Evolution Start

        for (int i = 0; i < _generations; i++) {
            // mating pool
            List<Schedule> matingPool = MatingPoolMethod(population);
            population.Clear();
            for (int t = 0; t < matingPool.Count; t += 2) {
                var parent1 = matingPool[t];
                var parent2 = matingPool[t + 1];
                (Schedule child1, Schedule child2) = CrossoverMethod(parent1, parent2, _solver);

                // mutation
                if (EvoRandom.Prob() < _mutationRate) MutationMethod(child1);
                if (EvoRandom.Prob() < _mutationRate) MutationMethod(child2);

                // environment selection 
                (Schedule sc1, Schedule sc2) = EnvironmentSelectionMethod(parent1, parent2, child1, child2);
                population.Add(sc1);
                population.Add(sc2);
            }
            // local-search    
            for (int sc = 0; sc < population.Count; sc++) {
                Schedule? sche = population[sc];
                sche = _solver.Run(sche); // can apply SA or TS
            }

            var localBest = population.MinBy(sche => sche.makespan)!;
            if (localBest.makespan < Result.makespan) {
                Result = localBest;
            }
            SpanHistory.Add(localBest.makespan);
        }

        return Result;
    }
    private List<Schedule> InitialPopulations() {
        // Init Population
        List<Schedule> population = new(_population);
        HashSet<int[]> orders = [];
        Schedule entity = new();
        for (int i = 0; i < _population; i++) {
            bool duplicate = false;
            do {
                entity = _solver.Run();
                duplicate = orders.Contains(entity.order);
                if (duplicate)
                    Console.WriteLine("dup");
                else
                    duplicate = false;

            } while (duplicate);

            orders.Add(entity.order);
            population.Add(new(entity));
        }
        return population;
    }

    // Builder 
    // ======================================================================

    // Builder-Pattern
    public delegate List<Schedule> MatingPoolDelegate(List<Schedule> groups);
    public delegate (Schedule, Schedule) CrossoverDelegate(Schedule parent1, Schedule parent2, SolverBase solver);
    public delegate (Schedule, Schedule) EnvironmentSelectionDelegate(Schedule parent1, Schedule parent2, Schedule childe2, Schedule child2);
    public delegate void MutationDelegate(Schedule sche);
    public class Builder {
        bool hasData = false;
        private EvolutionAlgo _instance = new();

        public Builder WithData(int[][] data) {
            _instance._dataset = data;
            hasData = true;
            return this;
        }
        public Builder Configure(int generations, int populations, double mutationRate) {
            _instance._population = populations;
            _instance._generations = generations;
            _instance._mutationRate = mutationRate;
            return this;
        }

        public Builder SetSolver(SolverBase solver) {
            if (!hasData) {
                throw new InvalidOperationException("should load data before set solver");
            }
            _instance._solver = solver;
            _instance._solver.LoadDataset(_instance._dataset);
            return this;
        }
        public Builder SetMatingPoolMethod(MatingPoolDelegate matingPool) {
            _instance.MatingPoolMethod = matingPool;
            return this;
        }
        public Builder SetCrossoverMethod(CrossoverDelegate crossover) {
            _instance.CrossoverMethod = crossover;
            return this;
        }
        public Builder SetMutationMethod(MutationDelegate mutation) {
            _instance.MutationMethod = mutation;
            return this;
        }
        public Builder SetEnvironmentSelection(EnvironmentSelectionDelegate environmentSelection) {
            _instance.EnvironmentSelectionMethod = environmentSelection;
            return this;
        }
        public EvolutionAlgo Build() {

            return _instance;
        }
    }
}
