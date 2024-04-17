using System;

public class ConsoleClearExample {
    public static void Main() {
        for (int i = 0; i < 100; i++) {
            Console.WriteLine("generation: " + i);

            for (int j = 0; j < 50; j++) {
                Console.WriteLine("processing: " + j);
                ClearCurrentConsoleLine();
            }
        }
    }

    public static void ClearCurrentConsoleLine() {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }
}
