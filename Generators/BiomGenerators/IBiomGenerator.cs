using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using static ToolsGame;

public abstract class IBiomGenerator
{
    protected ILevelGenerator[] levelGenerators;
    protected int levelCount;
    protected int cellSize;
    protected double probabilityOfLargerCell;
    protected int maxLevelSize;
    protected bool keepLinear;
    protected IBiomGenerator(int levelCount, int cellSize, double probabilityOfLargerCell, bool keepLinear, int maxLevelSize, ILevelGenerator levelGenerator, params ILevelGenerator[] levelGenerators)
    {
        var list = levelGenerators.ToList();
        list.Add(levelGenerator);
        this.levelGenerators = list.ToArray();
        this.levelCount = levelCount;
        this.cellSize = cellSize;
        this.maxLevelSize = maxLevelSize;
        this.probabilityOfLargerCell = probabilityOfLargerCell;
        this.keepLinear = keepLinear;
    }

    public class LevelStructure : ExitHandler
    {
        public int x;
        public int y;
        public int cellWidth;
        public int cellHeight;

        public LevelStructure(int x, int y, int cellWidth, int cellHeight, int structureId) : base(false)
        {
            this.x = x;
            this.y = y;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.Id = structureId;
        }
    }
    /// <summary>
    /// 2D dynamic array of empty/occupied cells
    /// </summary>
    protected List<List<bool>> Maze { get; set; } = new List<List<bool>> { new List<bool>() };
    /// <summary>
    /// Generates a new level of certain width and height
    /// </summary>
    /// <param name="biom">Reference to biom itself</param>
    /// <param name="maxSize">Maximal size of level (how many level cells the level can contain)</param>
    /// <returns></returns>
    public abstract void GenerateBiom(Biom biom);
    /// <summary>
    /// Creates a level for this biom
    /// </summary>
    /// <param name="generator">Type of level generator</param>
    /// <param name="algorithmStrength">Strength of the particular generator (mainly percentage of block count)</param>
    /// <param name="gameControlReference"></param>
    /// <param name="exits">How many exits are on each side (up, right, down, left)</param>
    /// <returns></returns>
    public Level CreateLevel(ILevelGenerator generator, Biom biom, int width, int height, List<PreExit>[] exits, int lastAddetExitDirection, int firstAddedExitDirection, int strId)
    {
        var lvl = new Level(GCon.game.IdLevels, generator, width, height, exits, lastAddetExitDirection, firstAddedExitDirection);
        lvl.Id = strId;
        biom.levels.Add(lvl);
        GCon.game.IdLevels++;
        return lvl;
    }
    public Level CreateLevel(ILevelGenerator generator, Biom biom, LevelStructure str, int cellSize = 0)
    {
        if (cellSize == 0)
        {
            cellSize = this.cellSize;
        }
        var lvl = new Level(GCon.game.IdLevels, generator, str.cellWidth * cellSize, str.cellHeight * cellSize, str.ExitsAr, str.LastAddedExitDirection, str.FirstAddedExitDirection);
        lvl.Id = str.Id;
        biom.levels.Add(lvl);
        GCon.game.IdLevels++;
        return lvl;
    }

    protected void GenerateLevelsFromStructures(LevelStructure[] arr, Biom biom)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            CreateLevel(ToolsMath.GetRandomElement<ILevelGenerator>(levelGenerators), biom, arr[arr.Length - 1 - i].cellWidth * cellSize, arr[arr.Length - 1 - i].cellHeight * cellSize, arr[arr.Length - 1 - i].ExitsAr, arr[arr.Length - 1 - i].LastAddedExitDirection, arr[arr.Length - 1 - i].FirstAddedExitDirection, arr[arr.Length - 1 - i].Id);
        }
    }
    /// <summary>
    /// Randomly chooses cell width and height of new level
    /// </summary>
    /// <param name="probability">Probability of larger cell</param>
    /// <param name="maxSize">Max cell count</param
    /// <param name="keepLinear">Whether to keep larger cell as one narrow long rectangle or to try to make it into square (or larger rectangle)</param>
    /// <returns>cell width and height of new level (levels are always rectangular)</returns>
    protected (int, int) GetCellSize(double probability, int maxSize, bool keepLinear, int x, int y, int widthIndex, int heightIndex, List<LevelStructure> allLevelStructures)
    {
        bool keepWidth = ToolsGame.Rng() < 0.5;
        int width = 1;
        int height = 1;
        int cellCount = 1;
        bool first = true;
        int[] empty;
        int maxWidth = Int32.MaxValue;
        int maxHeight = Int32.MaxValue;
        //while lucky and cell count is less than allowed maximum, add new cells
        while (Rng() <= probability && cellCount < maxSize)
        {
            //Okey, explenation here: Fundamental idea is 50/50 for +width or +height. But I may want to keep cells in row (keep linear = true).
            //Then if for the first Time the width is chosen, then width will be chosen every other Time too.
            //If keep linear is false the algorithm is allowed to add cell on each side (making more square-like rectangles).
            //But what if I need 7 cells and algo already formed square 2x2 - I have free space but I will never be able to form rectangle of exactly 7 cells.
            //So each Time algo checks if by enlarging particular dimension the rectangle is still possible to form.
            //And lastly the algo needs to check if by choosing particular dimension the number of cells won't be more than maximum.
            //And also if width or height is less than allowed maximum 
            //THIS TERM IS TOO CRAZY TO EVALUATE! Doing it brute force...
            //One more - when increasing size I need to check for empty blocks even in the new row/column
            empty = GetEmpty(x, y, allLevelStructures);
            if (maxWidth > empty[widthIndex])
                maxWidth = empty[widthIndex];
            if (maxHeight > empty[heightIndex])
                maxHeight = empty[heightIndex];
            bool widthOK = (maxSize - cellCount) % (height) == 0 && (cellCount + height) <= maxSize && width < maxWidth && (!keepLinear || keepWidth || first);
            bool heightOK = (maxSize - cellCount) % (width) == 0 && (cellCount + width) <= maxSize && height < maxHeight && (!keepLinear || !keepWidth || first);
            first = false;

            if (widthOK && heightOK)
            {
                if (ToolsGame.Rng() < 0.5)
                {
                    IncreaseDimension(ref height, ref cellCount, width);
                    if (widthIndex == 1)
                        x++;
                    else
                        x--;
                    keepWidth = false;
                }
                else
                {
                    IncreaseDimension(ref width, ref cellCount, height);
                    if (widthIndex == 0)
                        y++;
                    else
                        y--;
                    keepWidth = true;
                }
            }
            else if (!widthOK && heightOK)
            {
                IncreaseDimension(ref height, ref cellCount, width);
                if (widthIndex == 1)
                    x++;
                else
                    x--;
                keepWidth = false;
            }
            else if (widthOK && !heightOK)
            {
                IncreaseDimension(ref width, ref cellCount, height);
                if (widthIndex == 0)
                    y++;
                else
                    y--;
                keepWidth = true;
            }
            else
            {
                break;
            }

        }
        return (width, height);
    }
    private void IncreaseDimension(ref int increasingDim, ref int cellSize, int keptDim)
    {
        increasingDim++;
        cellSize += keptDim;
    }
    /// <param name="direction">0-up; 1-right; 2-down; 3-left</param>
    public void MoveInDirection(ref int x, ref int y, ref LevelStructure structure, int direction, int currentId, int width = 1, int height = 1)
    {
        MoveInDirection(ref x, ref y, ref structure, null, 0, 1, false, direction, currentId, width, height);
    }   
    /// <param name="direction">0-up; 1-right; 2-down; 3-left</param>
    public void MoveInDirection(ref int x, ref int y, ref LevelStructure structure, List<LevelStructure> allLevelStructures, double probability, int maxSize, bool keepLinear, int direction, int currentId, int width = 1, int height = 1)
    {
        int widthIndex = 0;
        int heightIndex = 0;
        if (structure != null)
        {
            x = structure.x;
            y = structure.y;
            switch (direction)
            {
                case 0:
                    y += structure.cellHeight;
                    x += structure.cellWidth - 1;
                    widthIndex = 1;
                    heightIndex = 0;
                    break;
                case 1:
                    y += structure.cellHeight - 1;
                    x += structure.cellWidth;
                    widthIndex = 1;
                    heightIndex = 0;
                    break;
                case 2:
                    y--;
                    widthIndex = 1;
                    heightIndex = 2;
                    break;
                case 3:
                    x--;
                    widthIndex = 3;
                    heightIndex = 0;
                    break;
                default:
                    break;
            }
        }
        if (allLevelStructures != null)
        {
            (width, height) = GetCellSize(probability, maxSize, keepLinear, x, y, widthIndex, heightIndex, allLevelStructures);
        }

        switch (direction)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                y -= height - 1;
                break;
            case 3:
                x -= width - 1;
                break;
            default:
                break;
        }
        var newStructure = new LevelStructure(x, y, width, height, currentId);
        if (structure == null)
        {
            structure = newStructure;
            return;
        }
        (int, int) exitPos;
        PreExit otherExit;
        if (direction % 2 == 1)
        {
            exitPos = GetExitsPos(y, structure.y, height, structure.cellHeight, cellSize);
            otherExit = new PreExit(exitPos.Item2, currentId);
        }
        else
        {
            exitPos = GetExitsPos(x, structure.x, width, structure.cellWidth, cellSize);
            otherExit = new PreExit(exitPos.Item2, currentId);
        }

        structure.AddExitPair(direction, otherExit, new PreExit(exitPos.Item1, structure.Id), newStructure);
        structure = newStructure;
        return;
    }

    
    /// <returns>Returns this exit pos and structure exit pos</returns>
    private (int, int) GetExitsPos(int pos, int structurePos, int length, int structureLength, int cellSize)
    {
        int boundaryLength;
        int myOffset = 0;
        int structureOffset = 0;

        if (pos < structurePos)
        {
            boundaryLength = pos - structurePos + length;
            structureOffset = 0;
            myOffset = structurePos - pos;
            if (boundaryLength > structureLength)
            {
                myOffset = pos + length - (structurePos + structureLength);
            }

        }
        else
        {
            boundaryLength = (structurePos - pos + structureLength);
            myOffset = 0;
            structureOffset = pos - structurePos;
            if (boundaryLength > length)
            {
                boundaryLength = length;
            }

        }
        int exitPos = ToolsGame.Rng(cellSize * boundaryLength);
        return (exitPos + myOffset * cellSize, exitPos + structureOffset * cellSize);
    }

    protected int[] GetEmpty(int x, int y, List<LevelStructure> levelStructures)
    {
        //up right down left
        int[] dims = new int[4] { Int32.MaxValue, Int32.MaxValue, Int32.MaxValue, Int32.MaxValue };
        for (int i = 0; i < levelStructures.Count; i++)
        {
            if (x >= levelStructures[i].x && x < levelStructures[i].x + levelStructures[i].cellWidth)
            {
                if (y - levelStructures[i].y < 0 && levelStructures[i].y - y < dims[0])
                {
                    dims[0] = levelStructures[i].y - y;
                }
                if (y - levelStructures[i].y > 0 && y - levelStructures[i].y - (levelStructures[i].cellHeight - 1) < dims[2])
                {
                    dims[2] = y - levelStructures[i].y - (levelStructures[i].cellHeight - 1);
                }
            }
            if (y >= levelStructures[i].y && y < levelStructures[i].y + levelStructures[i].cellHeight)
            {
                if (x - levelStructures[i].x > 0 && x - levelStructures[i].x - (levelStructures[i].cellWidth - 1) < dims[3])
                {
                    dims[3] = x - levelStructures[i].x - (levelStructures[i].cellWidth - 1);
                }
                if (x - levelStructures[i].x < 0 && levelStructures[i].x - x < dims[1])
                {
                    dims[1] = levelStructures[i].x - x;
                }
            }

        }
        return dims;
    }
}

