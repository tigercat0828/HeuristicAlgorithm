namespace Library.Configs;
public struct ParamConfig(int generations, int populations, double mutationRate) {
    public int generations = generations;
    public int populations = populations;
    public double mutationRate = mutationRate;
}