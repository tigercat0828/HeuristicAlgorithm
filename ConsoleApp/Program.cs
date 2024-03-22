using Heuristic;
using System.ComponentModel;
using System.Diagnostics;

ScheduleSolver solver = new("./Dataset/tai20_20_1.txt",AcceptanceMethod.II,1000);
solver.Run();

/*
Problem Definition
M machines, N jobs ...,solution for least time finish these jobs
 
Encoding : permutation-based 1,2,3,...,n
Neighborhood : swap two arbitrary jobs

Test Algorithm:
II, iterative improvement
SA, simulated annealing
TS, tabu search
RA, random search

Result:
Best/Average/Worst-case over 20 runs
computation environment and average computation time

Comparison in solution quality/efficiency/simplicity/robustness

More experiments
1. Run random search
2. large iterative time (may 1,000,000) & recorad local optima
3. different problem set size
4. intensification or diversification which is matter?

Correctness/Clarity/Carefulness/Completeness

 */
