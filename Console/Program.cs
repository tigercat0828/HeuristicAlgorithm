

Console.WriteLine("Hello");
SchedulingProblem problem = new("./Dataset/tai20_20_1.txt");
problem.Print();

int len = 20; 
int[] orders = new int[len];
for (int i = 0; i < len; i++) {
    orders[i] = i;  
}
int time = problem.MeasureAndPlot(orders);
Console.WriteLine(time);

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