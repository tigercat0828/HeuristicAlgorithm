using Library.Solvers;
using System.Text.Json;

Console.WriteLine("");

JobSche tmep = new([1, 2, 3, 4], 100);
JsonSerializerOptions options = new() { WriteIndented = true };
string jsonString = JsonSerializer.Serialize(tmep, options);
File.WriteAllText("test.json", jsonString);
