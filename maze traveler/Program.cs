using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


internal class Program
{
    static Random rand = new Random(1);
    static int direction = 3;

    private static void Main(string[] args)
    {
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8;
        bool[,] maze = Prims(10);
        bool[,] view = GetView(maze, 1, 1, direction);

        System.Console.WriteLine();

        int h = view.GetLength(0);
        int w = view.GetLength(1);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Console.Write(view[y, x] ? "█" : " ");
            }
            Console.WriteLine();
        }
    }

    static bool[,] GetView(bool[,] source, int pivotY, int pivotX, int direction)
    {
        int srcH = source.GetLength(0);
        int srcW = source.GetLength(1);

        int viewH = 5;
        int viewW = 3;

        bool[,] result = new bool[viewH, viewW];

        // Offsets for NORTH orientation, relative to pivot at bottom-center
        // dy: -4..0 (up 4 to pivot), dx: -1..1
        int[,] offsets = new int[,]
        {
            {-4, -1}, {-4, 0}, {-4, 1},
            {-3, -1}, {-3, 0}, {-3, 1},
            {-2, -1}, {-2, 0}, {-2, 1},
            {-1, -1}, {-1, 0}, {-1, 1},
            { 0, -1}, { 0, 0}, { 0, 1},
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int dy = offsets[i, 0];
            int dx = offsets[i, 1];

            // Rotate offsets
            int rdy = dy, rdx = dx;
            switch (direction % 4)
            {
                case 0: // North
                    rdy = dy; rdx = dx;
                    break;
                case 1: // East
                    rdy = -dx; rdx = dy;
                    break;
                case 2: // South
                    rdy = -dy; rdx = -dx;
                    break;
                case 3: // West
                    rdy = dx; rdx = -dy;
                    break;
            }

            int sy = pivotY + rdy;
            int sx = pivotX + rdx;

            // Map offsets into result coordinates (0..4, 0..2)
            int ry = dy + 4; // shift -4..0 to 0..4
            int rx = dx + 1; // shift -1..1 to 0..2

            if (sy >= 0 && sy < srcH && sx >= 0 && sx < srcW)
                result[ry, rx] = source[sy, sx];
            else
                result[ry, rx] = false; // OOB is empty
        }

        return result;
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