using Library.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Solvers; 
public partial class Evolution {
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
            _instance.LogFile = new(filename, generations, population, mutationRate);
            return this;
        }
        public Builder Configure(string filename, ParamConfig config) {
            _instance.Dataset = filename;
            _instance.m_Population = config.populations;
            _instance.m_Generations = config.generations;
            _instance.m_MutationRate = config.mutationRate;
            _instance.LogFile = new(filename, config.generations, config.populations, config.mutationRate);
            return this;
        }

        public Builder SetSolver(SolverBase solver) {
            if (!hasData) {
                throw new InvalidOperationException("should load data before set solver");
            }
            _instance.LogFile.LocalSearchMethod = solver.GetType().Name;
            _instance.m_Solver = solver;
            _instance.m_Solver.SetData(_instance.m_Data);
            return this;
        }
        public Builder SetMatingPoolMethod(MatingPoolDelegate matingPool) {
            _instance.LogFile.MatingPoolMethod = matingPool.Method.Name;
            _instance.MatingPoolMethod = matingPool;
            return this;
        }
        public Builder SetCrossoverMethod(CrossoverDelegate crossover) {
            _instance.LogFile.CrossOverMethod = crossover.Method.Name;
            _instance.CrossoverMethod = crossover;
            return this;
        }
        public Builder SetMutationMethod(MutationDelegate mutation) {
            _instance.LogFile.MutationMethod = mutation.Method.Name;
            _instance.MutationMethod = mutation;
            return this;
        }
        public Builder SetEnvironmentSelection(EnvironmentSelectionDelegate environmentSelection) {
            _instance.LogFile.EnvironmentMethod = environmentSelection.Method.Name;
            _instance.EnvironmentSelectionMethod = environmentSelection;
            return this;
        }
        public Evolution Build() {

            return _instance;
        }
    }
}
