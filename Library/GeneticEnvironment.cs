using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Library.Solver;
using Library.Solver;
namespace Library;
public class ScheduleGeneticEvolution {
    private static readonly Random random = new();
    public ScheduleGeneticEvolution(int[][] data, int generations, int population, int poolSize) {
        Generations = generations;
        Population = population;
        PoolSize = poolSize;
        Data = data;
        solver = null!;
    }
    protected ScheduleSolverBase solver;
    public readonly int[][] Data;
    public readonly int Generations;
    public readonly int Population;
    public readonly int PoolSize;
    public void SetSolver(ScheduleSolverBase solver) {
        this.solver = solver;
    }
    public JobSche Evolution() {
        List<JobSche> groups = new(Population);
        for (int i = 0; i < Population; i++) {
            JobSche job = solver.InitialSolution();
            groups.Add(job);
        }
        for (int i = 0; i < Generations; i++) {

            // mating pool
            List<JobSche> pool = MatingPool(groups);
            // selectparent
            (JobSche A, JobSche B) =  PickParents(pool);
            // cross-over
            // local-search    

        }
        return groups.MaxBy(sche => sche.makespan)!;
    }
    protected List<JobSche> MatingPool(List<JobSche> groups) {
        return groups.OrderBy(sche => sche.makespan).Take(PoolSize).ToList();
    }
    protected (JobSche, JobSche) PickParents(List<JobSche> pool) {
        int indexA = random.Next(pool.Count);
        
        int indexB= random.Next(pool.Count);
        
        return (pool[indexA], pool[indexB]);
    }
    protected void LocalSearch() {
        
    }


    
}
