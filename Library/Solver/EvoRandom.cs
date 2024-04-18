namespace Library.Solver;
public static class EvoRandom {
    private readonly static Random random = new();
    public static int Next(int range) {
        return random.Next(range);
    }
    public static int Next(int min, int max) {
        return random.Next(min, max);
    }
    public static double Prob() {
        return random.NextDouble();
    }

}
