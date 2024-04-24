namespace Library.IO;
public class Report {
    public Dictionary<string, List<LogFile>> dataset_logs;

    public Report(List<string> datasets) {
        dataset_logs = [];
        foreach (var dataset in datasets) {
            dataset_logs.Add(dataset, []);
        }
    }
    public void AddLog(string dataset, LogFile log) {
        dataset_logs[dataset].Add(log);
    }
    public void MakeCSV() {
        // Specify the file path where you want to write the content
        string filePath = "./Output/Report.csv";

        // Write content to the file using StreamWriter
        using (StreamWriter writer = new StreamWriter(filePath)) {
            // Write the header to the file
            writer.WriteLine("Dataset, makespan,Generation,Population, MutationRate,MatingPool,EnvSelect,LocalSearch,order");

            // Loop through the dataset_logs and write each entry to the file
            foreach (var dataset_log in dataset_logs) {
                string dataset = dataset_log.Key;
                var logs = dataset_log.Value;

                foreach (var log in logs) {
                    writer.WriteLine($"{dataset},{log.Result.makespan},{log.Generation},{log.Generation},{log.MutationRate},{log.MatingPoolMethod},{log.EnvironmentMethod},{log.LocalSearchMethod},{log.Result.orderjsonstr}");
                }
            }
        }

        Console.WriteLine("Content has been written to the file.");

    }
}
