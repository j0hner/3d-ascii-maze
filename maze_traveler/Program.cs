using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


internal class Program
{
    static readonly Random rand = new Random(1);
    const bool debug = false;
    static int direction = 3; // 0=N, 1=E, 2=S, 3=W
    static int playerX = 1;
    static int playerY = 1;

    private static void Main(string[] args)
    {
        Console.Clear();
        Console.SetBufferSize(500, 500);
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;

        bool[,] maze = Prims(10);

        bool[,] view = GetView(maze, playerX, playerY, direction);
        Draw(view);

        while (true)
        {
            Console.CursorLeft = 0;
            Console.Write($"Enter action (W to move, QE to turn): ");
            ConsoleKeyInfo key;
            do
            {
                Console.Write(' ');
                Console.CursorLeft--;
                key = Console.ReadKey(false); //show the key pressed
                Thread.Sleep(100);
                Console.CursorLeft--;
            } while (
                key.Key != ConsoleKey.W &&
                key.Key != ConsoleKey.Q &&
                key.Key != ConsoleKey.E
            );

            while (Console.KeyAvailable) // consume ironeous key presses
                Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Q:
                    direction = (direction + 1) % 4;
                    break;
                case ConsoleKey.E:
                    direction = (direction + 3) % 4; // equivalent to -1 mod 4, avoids negatives
                    break;
                case ConsoleKey.W:
                    if (!Move(view)) continue;
                    break;
            }
            Console.Clear();
            view = GetView(maze, playerY, playerX, direction);
            Draw(view, maze);
        }
    }

    static bool Move(bool[,] view)
    {
        if (view[3, 1]) return false; // wall in front
        switch (direction)
        {
            case 0: // North
                playerY -= 1;
                break;
            case 1: // East
                playerX -= 1;
                break;
            case 2: // South
                playerY += 1;
                break;
            case 3: // West
                playerX += 1;
                break;
        }
        return true;
    }

    static void Draw(bool[,] view, bool[,] maze = null)
    {
        const int height = 41;
        const int width = 48;

        string[] playerSlices = new string[]{
            " ╱▔▔▔▔▔▔▔▔▔╲ ",
            "▕▔▔▔▔▔▔▔▔▔▔▔▏",
            "▕           ▏",
            "▕           ▏",
            "▕           ▏",
            "▕           ▏",
            "▕           ▏",
            "▕           ▏",
            "▕           ▏",
            "▕           ▏",
            " ▔▔▔▔▔▔▔▔▔▔▔ "
        };

        int[] cellWidths = new int[]{
            47, 35, 27, 21, 17
        };

        int[] cellHeights = new int[]{
            40, 28, 20, 14, 10
        };

        int[] cellDepths = new int[]{
            1, 6, 10, 13, 15
        };

        int[] cellCornerLineLengths = new int[]{
            6, 5, 4, 3
        };

        Console.SetCursorPosition(0, 0);
        Console.Write(" " + new string('_', cellWidths[0]));

        if (debug)
        {
            Console.WriteLine($"\rFacing: {(new string[] { "NORTH", "EAST", "SOUTH", "WEST" })[direction]} ({direction}), Pos: ({playerX},{playerY})");
            for (int y = 0; y < view.GetLength(0); y++)
            {
                Console.CursorLeft = width + 1;
                for (int x = 0; x < view.GetLength(1); x++)
                    Console.Write(view[y, x] ? "█" : (x == 1 && y == 4) ? "↑":" ");
                    
                Console.WriteLine();
            }

            if (maze != null)
            {
                Console.WriteLine();
                for (int y = 0; y < maze.GetLength(0); y++)
                {
                    Console.CursorLeft = width + 1;
                    for (int x = 0; x < maze.GetLength(1); x++)
                        if (y == playerY && x == playerX)
                            Console.Write(new string[] { "↑", "←", "↓", "→" }[direction]);
                        else
                            Console.Write(maze[y, x] ? "█" : " ");
                    Console.WriteLine();
                }
            }
            Console.SetCursorPosition(0, 0);
        }

        int depth = 0;
        int drawX = 1;
        int drawY = 1;

        char PickChar(bool hasLine, bool isTransition, char lineChar, char transitionChar)
        {
            if (hasLine) return lineChar;
            if (isTransition) return transitionChar;
            return ' ';
        }

        //draw in diagonals and horizontals
        for (int i = 1; i <= 15; i++)
        {
            bool hasWestLine = view[4 - depth, 0];
            bool hasEastLine = view[4 - depth, 2];
            bool isCellTransition = i == cellDepths[depth + 1];

            Console.SetCursorPosition(drawX, height - drawY);
            Console.Write(PickChar(hasWestLine, isCellTransition, '╱', '▔'));

            Console.SetCursorPosition(width - drawX, height - drawY);
            Console.Write(PickChar(hasEastLine, isCellTransition, '╲', '▔'));

            Console.SetCursorPosition(width - drawX, drawY);
            Console.Write(PickChar(hasEastLine, isCellTransition, '╱', '_'));

            Console.SetCursorPosition(drawX, drawY);
            Console.Write(PickChar(hasWestLine, isCellTransition, '╲', '_'));

            if (isCellTransition)
            {
                string bottomLine = new string('_', cellCornerLineLengths[depth] - 1);
                string topLine = new string('▔', cellCornerLineLengths[depth] - 1);

                if (!hasWestLine)
                {
                    Console.CursorLeft = cellDepths[depth];
                    Console.Write(bottomLine);
                }
                if (!hasEastLine)
                {
                    Console.CursorLeft = width + 1 - cellDepths[depth + 1];
                    Console.Write(bottomLine);
                }
                Console.CursorTop += cellHeights[depth + 1] + 1;
                if (!hasWestLine)
                {
                    Console.CursorLeft = cellDepths[depth];
                    Console.Write(topLine);
                }
                if (!hasEastLine)
                {
                    Console.CursorLeft = width + 1 - cellDepths[depth + 1];
                    Console.Write(topLine);
                }


                depth++;
                bool drawWall = view[4 - depth, 1];

                Console.SetCursorPosition(drawX + 1, drawY);
                Console.Write(new string(drawWall ? '_' : ' ', cellWidths[depth]));

                Console.SetCursorPosition(drawX + 1, height - drawY);
                Console.Write(new string(drawWall ? '▔' : ' ', cellWidths[depth]));

                if (drawWall) break;
            }

            drawX++;
            drawY++;
        }

        bool vertStopFlag = false;
        // draw in verticals
        for (int i = 0; i < 5; i++)
        {
            depth = 4 - i;

            int cellDepth = cellDepths[i] + ((i == 0) ? 0 : 1);

            Console.SetCursorPosition(cellDepth - 1, cellDepth);

            for (int j = 0; j < cellHeights[i]; j++)
            {
                Console.Write("▕");
                Console.CursorLeft--;
                Console.CursorTop++;
            }

            Console.SetCursorPosition(width - cellDepth + 1, cellDepth);

            for (int j = 0; j < cellHeights[i]; j++)
            {
                Console.Write("▏");
                Console.CursorLeft--;
                Console.CursorTop++;
            }
            if (vertStopFlag)
                break;
            if (depth - 1 >= 0 && view[depth - 1, 1])
                vertStopFlag = true;
        }

        Console.SetCursorPosition(18, 29);
        foreach (string slice in playerSlices)
        {
            Console.Write(slice);
            Console.CursorLeft = 18;
            Console.CursorTop++;
        }


        Console.SetCursorPosition(0, height);
        Console.WriteLine(" " + new string('▔', cellWidths[0]));
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

        return maze;
    }
}
