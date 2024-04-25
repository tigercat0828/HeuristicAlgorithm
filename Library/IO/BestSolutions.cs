using Library.Solvers;
using System.Text.Json;

namespace Library.IO {
    public static class BestSolutions {
        static string filename = "./Output/BestSolutions.json";
        static List<string> datasets = [
            "tai20_5_1.txt",
            "tai20_10_1.txt",
            "tai20_20_1.txt",
            "tai50_5_1.txt",
            "tai50_10_1.txt",
            "tai50_20_1.txt",
            "tai100_5_1.txt",
            "tai100_10_1.txt",
            "tai100_20_1.txt",
        ];

        public static Dictionary<string, JobSche> Solutions { get; private set; }

        public static void CompareAndUpdate(string dataset, JobSche sche) {
            if (Solutions[dataset] is null) {
                Solutions[dataset] = sche;
                return;
            }
            if (sche.makespan <  Solutions[dataset].makespan) {
                Solutions[dataset] = sche;
            }
        }
        public static void Save() {
            JsonSerializerOptions options = new() { WriteIndented = true, IncludeFields = true };
            string jsonString = JsonSerializer.Serialize(Solutions, options);
            File.WriteAllText(filename, jsonString);

            using (StreamWriter writer = new StreamWriter("./Output/BestSolutions.csv")) {
                // Write the header to the file
                writer.WriteLine("Dataset, Makespan, Order");

                foreach (var solution in Solutions) {
                    string dataset = solution.Key;
                    var sche = solution.Value;

                    writer.WriteLine($"{dataset},{sche.makespan}, {sche.orderjsonstr}");
                }
            }

        }

        public static void Load() {
            string jsonString = File.ReadAllText(filename);
            if (string.IsNullOrEmpty(jsonString)) {
                Solutions = [];
                foreach (var dataset in datasets) {
                    Solutions.Add(dataset, null!);
                }
            }
            else {
                Solutions = JsonSerializer.Deserialize<Dictionary<string, JobSche>>(jsonString);
            }

        }
    }
}
