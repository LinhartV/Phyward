using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToolsGame;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine.UIElements;

/// <summary>
/// How does this work: Random empty tile is selected and checked, whether putting wall there would split the maze to two unconnected parts. If so, another position is tried.
/// </summary>
public class BlockInsert : ILevelGenerator
{
    private List<(int, int)> finalEmptyPos = new List<(int, int)>();
    private MazeTile[,] maze;
    private double blockCountPercentage;

    public BlockInsert(double algoStrength)
    {
        this.blockCountPercentage = algoStrength;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="blockCountPercentage"></param>
    /// <param name="exits">Position of exits from level to another level</param>
    /// <returns></returns>
    public override MazeTile[,] GenerateLevel(int width, int height, List<PreExit>[] exits, out List<(int, int)> emptyPos, Level l)
    {
        int blockCount = (int)(width * height * blockCountPercentage);
        if (blockCount + 1 > width * height)
            blockCount = width * height - 1;
        maze = base.GenerateLevel(width, height, exits, out emptyPos, l);
        //for creating level border
        width += 2;
        height += 2;
        //add all empty positions to emptyPos (except for exits)
        emptyPos = new List<(int, int)>();
        int exitsCount = 0;
        for (int i = 0; i < exits.Length; i++)
        {
            exitsCount += exits[i].Count;
        }
        for (int i = 1; i < height - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                emptyPos.Add((i, j));
            }
        }
        //add blocks one by one
        for (int i = 0; i < blockCount; i++)
        {
            if (!emptyPos.Remove(AddBlock(emptyPos, i + 1, exitsCount)))
                break;
        }
        finalEmptyPos = new List<(int, int)>(emptyPos);
        return maze;
    }
    
    /// <summary>
    /// Recursivelly checks if adding this block will split the maze. 
    /// </summary>
    /// <param name="start">If it's the initial or recursive call</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="num">number of blocks checked for being connected together</param>
    /// <param name="blockCount">number of already set blocks</param>
    /// <returns>If block splits the maze, number of traversed tiles</returns>
    private (bool, int) IsBlocked(bool start, int x, int y, int num, int exitsCount = 0, int blockCount = 0)
    {
        //when checking empty space, count it and set it as checked
        if (maze[y, x] != MazeTile.potentialBlock)
        {
            maze[y, x] = MazeTile.counted;
            num++;
        }
        bool doubleBreak = false;
        //recursivelly check the direct surrounding - traversing over an empty space will make it counted and not traversable anymore.
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j += 2)
            {
                if (i != 0)
                {
                    j = 0;
                }
                if (y + i >= 0 && y + i < maze.GetLength(0) && x + j >= 0 && x + j < maze.GetLength(1) && maze[y + i, x + j] != MazeTile.counted && maze[y + i, x + j] != MazeTile.block && maze[y + i, x + j] != MazeTile.potentialBlock)
                {
                    num = IsBlocked(false, x + j, y + i, num).Item2;
                    if (start)
                    {
                        doubleBreak = true;
                        break;
                    }
                }
            }
            if (doubleBreak)
                break;
        }
        //When all possible space is traversed, it will be checked if number of all traversed tiles is equal to all empty tiles
        //(it would mean that all empty tiles are traversable and thus new block won't split the maze)
        if (start)
        {
            if (num == (maze.GetLength(0) - 2) * (maze.GetLength(1) - 2) - blockCount + exitsCount)
            {
                for (int i = 0; i < maze.GetLength(0); i++)
                {
                    for (int j = 0; j < maze.GetLength(1); j++)
                    {
                        if (maze[i, j] == MazeTile.potentialBlock)
                        {
                            maze[i, j] = MazeTile.block;
                        }
                        else if (maze[i, j] == MazeTile.counted)
                            maze[i, j] = MazeTile.empty;
                    }
                }
                return (false, 0);
            }
            else
            {
                for (int i = 0; i < maze.GetLength(0); i++)
                {
                    for (int j = 0; j < maze.GetLength(1); j++)
                    {
                        if (maze[i, j] == MazeTile.potentialBlock || maze[i, j] == MazeTile.counted)
                            maze[i, j] = MazeTile.empty;
                    }
                }
                return (true, 0);
            }
            
        }
        else
            return (true, num);
    }
    /// <summary>
    /// Adds one block to maze in a way that the block will not split the maze
    /// </summary>
    /// <param name="emptyPos"></param>
    /// <param name="blockCount"></param>
    /// <param name="exitsCount">Number of exits tiles</param>
    /// <returns>index of an empty position to remove</returns>
    private (int, int) AddBlock(List<(int, int)> emptyPos, int blockCount, int exitsCount)
    {
        List<(int, int)> emptyPosTemp = new List<(int, int)>(emptyPos);
        (int, int) position;
        int index;
        do
        {
            if (emptyPosTemp.Count == 0)
            {
                return (-1, -1);
            }
            index = Rng(0, emptyPosTemp.Count);
            //index = 0;
            position = emptyPosTemp[index];
            maze[position.Item1, position.Item2] = MazeTile.potentialBlock;
            emptyPosTemp.RemoveAt(index);
        } while (IsBlocked(true, position.Item2, position.Item1, 0, exitsCount, blockCount).Item1);
        return position;
    }

}
