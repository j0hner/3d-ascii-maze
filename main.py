from mazelib import Maze
from mazelib.generate.Prims import Prims
Prims
maze = Maze()
maze.generator = Prims(15,15)
maze.generate()

grid = maze.grid

# print to console
for row in grid:
    print("".join("█" if cell == 1 else " " for cell in row))