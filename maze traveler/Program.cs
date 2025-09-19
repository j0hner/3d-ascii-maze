using System;
using System.Collections.Generic;
using System.Linq;


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
        int length = sideLength * 2 + 1;
        bool[,] maze = new bool[length, length];

        // Fill everything with walls first
        for (int y = 0; y < length; y++)
            for (int x = 0; x < length; x++)
                maze[y, x] = true;

        // Frontier set
        HashSet<Tuple<int, int>> frontier = new HashSet<Tuple<int, int>>();

        // Helper: check bounds
        Func<Tuple<int, int>, bool> InBounds = c =>
            c.Item1 > 0 && c.Item1 < length - 1 &&
            c.Item2 > 0 && c.Item2 < length - 1;

        // Helper: get neighbors two steps away
        Func<Tuple<int, int>, List<Tuple<int, int>>> Neighbors = c =>
        {
            var result = new List<Tuple<int, int>>();
            int y = c.Item1, x = c.Item2;
            var dirs = new Tuple<int, int>[] {
                Tuple.Create(-2, 0),
                Tuple.Create(2, 0),
                Tuple.Create(0, -2),
                Tuple.Create(0, 2)
            };

            foreach (var d in dirs)
            {
                var n = Tuple.Create(y + d.Item1, x + d.Item2);
                if (InBounds(n))
                    result.Add(n);
            }
            return result;
        };

        // choose a random starting cell
        var start = Tuple.Create(
            (rand.Next(0, sideLength)) * 2 + 1,
            (rand.Next(0, sideLength)) * 2 + 1
        );

        maze[start.Item1, start.Item2] = false;

        // add its neighbors to the frontier
        foreach (var n in Neighbors(start))
            frontier.Add(n);

        while (frontier.Count > 0)
        {
            // pick a random frontier cell
            var frontierCell = frontier.ElementAt(rand.Next(frontier.Count));

            // find neighbors already in the maze
            var neighs = Neighbors(frontierCell)
                .Where(n => !maze[n.Item1, n.Item2])
                .ToList();

            if (neighs.Count > 0)
            {
                var connected = neighs[rand.Next(neighs.Count)];

                // carve passage between frontierCell and connected
                int wy = (frontierCell.Item1 + connected.Item1) / 2;
                int wx = (frontierCell.Item2 + connected.Item2) / 2;
                maze[frontierCell.Item1, frontierCell.Item2] = false;
                maze[wy, wx] = false;
            }

            // mark this cell as part of the maze
            maze[frontierCell.Item1, frontierCell.Item2] = false;
            frontier.Remove(frontierCell);

            // add untouched neighbors to frontier
            foreach (var n in Neighbors(frontierCell))
            {
                if (maze[n.Item1, n.Item2]) // still a wall
                    frontier.Add(n);
            }
        }

        // draw
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
                Console.Write(maze[y, x] ? "█" : " ");
            Console.WriteLine();
        }

        return maze;
    }
}