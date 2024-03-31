namespace Library;
public static class IO {
    public static int[][] LoadFile(string filename) {
        string[] lines = File.ReadAllLines(filename);
        string[] tokens = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int jobs = int.Parse(tokens[0]);
        int machines = int.Parse(tokens[1]);
        int[][] data = new int[jobs][];
        for (int i = 0; i < jobs; i++) data[i] = new int[machines];
        for (int i = 1; i < lines.Length; i++) {
            string[] elements = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < elements.Length; t++) {
                data[t][i - 1] = int.Parse(elements[t]);
            }
        }
        return data;
    }
}
