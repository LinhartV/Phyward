using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToolsGame;

public class CustomLevelGenerator : ILevelGenerator
{
    private MazeTile[,] customMaze;
    public CustomLevelGenerator(MazeTile[,] customMaze, double algoStrength, params ISpawner[] spawners) : base(algoStrength, spawners)
    {
        this.customMaze = customMaze;
    }

    public override ToolsGame.MazeTile[,] GenerateLevel(int width, int height, List<PreExit>[] exitsAr, out List<(int, int)> emptyPos, Level level)
    {
        maze = base.GenerateLevel(width, height, exitsAr, out emptyPos, level);
        for (int i = 0; i < customMaze.GetLength(0); i++)
        {
            for (int j = 0; j < customMaze.GetLength(1); j++)
            {
                maze[i + 1, j + 1] = customMaze[i, j];
            }
        }
        return maze;
    }
}

