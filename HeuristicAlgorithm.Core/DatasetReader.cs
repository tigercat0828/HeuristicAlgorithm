namespace HeuristicAlgorithm.Core;
public static class DatasetReader {
    public static int[][] LoadFile(string filename) {

        string[] lines = File.ReadAllLines(filename);
        string[] header = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        int jobs = int.Parse(header[0]);
        int machines = int.Parse(header[1]);

        int[][] data = new int[machines][];

        for (int i = 0; i < machines; i++) data[i] = new int[jobs];

        for (int m = 1; m < lines.Length; m++) {
            string[] tokens = lines[m].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < jobs; j++) {
                data[m - 1][j] = int.Parse(tokens[j]);
            }
        }
        return data;
    }
    /*
    Example data file format 
    first line: job_num machine_num dataset_name

    20 5 tai20_5_1
    54 83 15 71 77 36 53 38 27 87 76 91 14 29 12 77 32 87 68 94
    79  3 11 99 56 70 99 60  5 56  3 61 73 75 47 14 21 86  5 77
    16 89 49 15 89 45 60 23 57 64  7  1 63 41 63 47 26 75 77 40
    66 58 31 68 78 91 13 59 49 85 85  9 39 41 56 40 54 77 51 31
    58 56 20 85 53 35 53 41 69 13 86 72  8 49 47 87 58 18 68      
    */
}
