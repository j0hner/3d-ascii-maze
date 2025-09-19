internal class Program
{
    private static void Main(string[] args)
    {
        Prims(10);
    }

    static bool[,] Prims(int sideLength)
    {
        int length = sideLength * 2 + 2;
        bool[,] maze = new bool[length, length];
        for (int y = 0; y < length - 1; y++)
        {
            for (int x = 0; x < length - 1; x++)
            {
                maze[y, x] = y % 2 != 0 && x % 2 != 0;
                Console.Write(maze[y, x] ? "1" : "0");
            }
            Console.WriteLine();
        }

        return new bool[,] { }; //shut up compiler
    }
}