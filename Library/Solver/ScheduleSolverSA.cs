using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Solver;
public class ScheduleSolverSA(int[][] data) : ScheduleSolverBase(data) {
    public override JobSche Run(int rounds = 1) {
        throw new NotImplementedException();
    }

    protected override JobSche Select(List<JobSche> neighbors, JobSche localBest) {
        throw new NotImplementedException();
    }
}
