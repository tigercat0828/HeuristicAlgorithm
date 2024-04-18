namespace Library.IO;
public class EntireLog {
    public Dictionary<string, List<LogFile>> dataset_logs;

    public EntireLog(List<string> datasets) {
        dataset_logs = [];
        foreach (var dataset in datasets) {
            dataset_logs.Add(dataset, []);
        }
    }
    public void AddLog(string dataset, LogFile log) {
        dataset_logs[dataset].Add(log);
    }
    public void MakeCSV() {
        using (var writer = new StringWriter()) {
            foreach (var dataset_log in dataset_logs) {
                string dataset = dataset_log.Key;
                var log = dataset_log.Value;
            }
        }

    }
}
