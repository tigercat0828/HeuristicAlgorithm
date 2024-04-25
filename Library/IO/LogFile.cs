using Library.Solvers;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Library.IO;
public class LogFile {
    public LogFile(string datasetName, int generation, int population, double mutationRate) {
        DatasetName = datasetName;
        Generation = generation;
        Population = population;
        MutationRate = mutationRate;
    }

    public string DatasetName = string.Empty;
    public int Generation;
    public int Population;
    public double MutationRate;
    public string MatingPoolMethod = string.Empty;
    public string CrossOverMethod = string.Empty;
    public string MutationMethod = string.Empty;
    public string EnvironmentMethod = string.Empty;
    public string LocalSearchMethod = string.Empty;
    [JsonIgnore]
    public List<double> meanList = [];
    [JsonIgnore]
    public List<double> DeviationList = [];
    public string Means => $"[{string.Join(',', meanList)}]";
    public string Deviations => $"[{string.Join(',', DeviationList)}]";
    public JobSche Result = null!;
    public double TimeCost;

    public string GetExpName() {
        return $"[{DatasetName}][{Generation},{Population},{MutationRate:F3}][{MatingPoolMethod}][{EnvironmentMethod}]";
    }
    private static readonly JsonSerializerOptions options = new() {
        WriteIndented = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    public void SaveLog(int epoch) {
        // will be the ouput filename like [tai_20_5][...].json
        string filename = GetExpName();
        string jsonString = JsonSerializer.Serialize(this, options);
        File.WriteAllText($"./Output/Logs/{filename}_{epoch}.json", jsonString);
        return;
    }

}
