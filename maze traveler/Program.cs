using System;
using System.Collections.Generic;

internal class Program
{
    static Random rand = new Random();

    private static void Main(string[] args)
    {
        Console.Clear();
        Prims(10);
        Console.SetCursorPosition(0, 22);
    }

    static bool[,] Prims(int sideLength)
    {
        int length = sideLength * 2 + 2;
        bool[,] maze = new bool[length, length];

        Dictionary<int[], List<int[]>> cellWalls = new Dictionary<int[], List<int[]>>(); // dict cell corrds : walls [U, R, L, D]
        List<int[]> frontier = new List<int[]>(); // all walls on the maze perimeter

        for (int y = 0; y < length - 1; y++)
        {
            for (int x = 0; x < length - 1; x++)
            {
                maze[y, x] = !(y % 2 != 0 && x % 2 != 0);
                if (!maze[y, x])
                {
                    List<int[]> currentCellWalls = new List<int[]>();

                    // do not add side walls (saves a check)
                    if (y - 1 > 0) currentCellWalls.Add(new int[] { y - 1, x });
                    if (x - 1 > 0) currentCellWalls.Add(new int[] { y, x - 1 });
                    if (y + 1 < length) currentCellWalls.Add(new int[] { y + 1, x });
                    if (x + 1 < length) currentCellWalls.Add(new int[] { y, x + 1 });

                    cellWalls.Add(new int[]{y, x}, currentCellWalls);
                }
                Console.Write(maze[y, x] ? "█" : " ");
            }
            Console.WriteLine();
        }

        // choose random start point
        int[] startCoords = new int[] {(rand.Next(0, sideLength - 1) + 1) * 2, (rand.Next(0, sideLength - 1) + 1) * 2};

        Console.SetCursorPosition(startCoords[0] - 1, startCoords[1] - 1);
        Console.Write("*");

        foreach (int[] wall in cellWalls[startCoords]) frontier.Add(wall);

        return new bool[,] { }; //shut up compiler
    }
}