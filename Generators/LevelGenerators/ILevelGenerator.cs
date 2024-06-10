using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ToolsGame;

public abstract class ILevelGenerator
{
    protected List<(int, int)> finalEmptyPos = new List<(int, int)>();
    protected MazeTile[,] maze;
    protected double blockCountPercentage;
    protected ISpawner[] spawners;
    protected ILevelGenerator(double algoStrength, params ISpawner[] spawners)
    {
        this.blockCountPercentage = algoStrength;
        this.spawners = spawners;
    }

    public void SpawnItems(Level level)
    {
        foreach (var spawner in spawners)
        {
            spawner.Spawn(level);
        }
    }

    /// <summary>
    /// Generates a new level of certain width and height
    /// </summary>
    /// <param name="width">Width of the level</param>
    /// <param name="height">Width of the level</param>
    ///<param name="exits">Positions of exits from level to another</param>
    /// <returns>Finished level layout;</returns>
    public virtual MazeTile[,] GenerateLevel(int width, int height, List<PreExit>[] exitsAr, out List<(int, int)> emptyPos, Level level)
    {
        //generates level border with exits and empty inside
        width += 2;
        height += 2;
        MazeTile[,] maze = new MazeTile[height, width];
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                if (i == 0 || i == maze.GetLength(0) - 1 || j == 0 || j == maze.GetLength(1) - 1)
                {
                    maze[i, j] = MazeTile.block;
                }
                else
                    maze[i, j] = MazeTile.empty;

            }
        }
        int x = 0;
        int y = 0;
        int xOffset = 0;
        int yOffset = 0;
        for (int i = 0; i < exitsAr.Length; i++)
        {
            for (int j = 0; j < exitsAr[i].Count; j++)
            {
                if (i == 2)
                {
                    y = 0;
                    x = exitsAr[i][j].Position + 1;
                    xOffset = 0;
                    yOffset = -1;
                }
                if (i == 1)
                {
                    y = exitsAr[i][j].Position + 1;
                    x = maze.GetLength(1) - 1;
                    xOffset = 1;
                    yOffset = 0;
                }
                if (i == 0)
                {
                    y = maze.GetLength(0) - 1;
                    x = exitsAr[i][j].Position + 1;
                    xOffset = 0;
                    yOffset = 1;
                }
                if (i == 3)
                {
                    y = exitsAr[i][j].Position + 1;
                    x = 0;
                    xOffset = -1;
                    yOffset = 0;
                }
                maze[y, x] = MazeTile.empty;
                exitsAr[i][j].X = x;
                exitsAr[i][j].Y = y;
                level.AddItem(new Exit(new Vector2(x + xOffset, y + yOffset), x, y, exitsAr[i][j].LevelId, exitsAr[i][j].ExitId));

            }
        }
        emptyPos = null;
        return maze;
    }
}

