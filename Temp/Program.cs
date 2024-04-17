public class GeneticAlgorithm {
    private static Random random = new Random();

    // Function to perform Linear Order Crossover and produce two children
    public static void LinearOrderCrossover(int[] parent1, int[] parent2, out int[] child1, out int[] child2) {
        int length = parent1.Length;
        child1 = new int[length];
        child2 = new int[length];

        // Choose two random crossover points as the slice
        int start = random.Next(length);
        int end = random.Next(start, length);
        // Initialize children with -1 to indicate unfilled positions
        for (int i = 0; i < length; i++) {
            child1[i] = -1;
            child2[i] = -1;
        }

        // Copy a slice from each parent based on the crossover points
        for (int i = start; i <= end; i++) {
            child1[i] = parent1[i];
            child2[i] = parent2[i];
        }

        FillChildWithRemainingElements(parent2, child1);
        FillChildWithRemainingElements(parent1, child2);

        void FillChildWithRemainingElements(int[] parent, int[] child) {
            int length = parent.Length;
            int curPos = 0;
            for (int i = 0; i < length; i++) {
                if (!child.Contains(parent[i])) {       // we can speed up search with new a set
                                                        // Find the next unfilled position in the child
                    while (child[curPos] != -1) curPos++;

                    child[curPos] = parent[i];
                }
            }
        }
    }


    static void Main(string[] args) {
        // Example parents (permutations of numbers 1 through 6)
        int[] parent1 = { 1, 2, 3, 4, 5, 6 };
        int[] parent2 = { 4, 5, 6, 1, 2, 3 };

        // Generate offspring
        LinearOrderCrossover(parent1, parent2, out int[] child1, out int[] child2);

        // Print the offspring to demonstrate the result
        Console.WriteLine("Child 1: " + string.Join(", ", child1));
        Console.WriteLine("Child 2: " + string.Join(", ", child2));
    }
}
