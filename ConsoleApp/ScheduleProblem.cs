using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heuristic {
    public class ScheduleProblem {
        void Evo() {
            Stopwatch sw = new();
            sw.Start();
            // turn in to batch test
            string dataname = "tai20_20_1";
            int[][] data = Schedule.LoadFile($"./Dataset/{dataname}.txt");

            Schedule origin = new(dataname);
            origin.LoadData(data);
            origin.SetInitialOrder(Enumerable.Range(0, origin.Jobs - 1).ToArray());
            origin.Apply(VariantMethod.II);
            List<Schedule> schedules = [origin];

            for (int i = 0; i < 1000; i++) {
                Schedule sche = new(dataname);
                sche.LoadData(data);
                sche.Apply(VariantMethod.II);
                schedules.Add(sche);
            }
            //Parallel.For(0, problems.Count, i => {
            //    problems[i].Run();
            //});
            for (int i = 0; i < schedules.Count; i++) {
                schedules[i].Run();
            }
            Schedule best = schedules.First();
            foreach (var problem in schedules) {
                if (problem.Result.makespan < best.Result.makespan) {
                    best = problem;
                }
                //problem.PrintExpResult();
                //problem.WriteExpResult();
            }
            sw.Stop();
            double timecost = sw.Elapsed.TotalSeconds;
            Schedule.Plot($"{dataname}_II", data, best.Result.order);       // make figure
            Console.WriteLine($"Init Order : [{string.Join(',', best.InitialOrder)}]");
            Console.WriteLine($"Best Order : [{string.Join(',', best.Result.order)}]");
            Console.WriteLine($"Makespan : {best.Result.makespan} seconds");
            Console.WriteLine($"time cost{timecost}");

        }
    }
}
