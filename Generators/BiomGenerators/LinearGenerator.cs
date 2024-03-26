using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Generates biom levels as a path graph 
/// Can create levels of larger cells that will fit the lattice (no two levels will overlap)
/// </summary>
public class LinearGenerator : IBiomGenerator
{
   
    private ILevelGenerator[] levelGenerators;
    private int levelCount;
    private int cellSize;
    private double probabilityOfLargerCell;
    private int maxLevelSize;
    private bool keepLinear;
    public LinearGenerator(int levelCount, int cellSize, double probabilityOfLargerCell, bool keepLinear, int maxLevelSize, ILevelGenerator levelGenerator, params ILevelGenerator[] levelGenerators)
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

    override public Dictionary<int, Level> GenerateBiom(Biom biom, GameControl gameControlReference)
    {
        int currentId = gameControlReference.Id;
        Dictionary<int, Level> levels = new Dictionary<int, Level>();
        //real level structure that will be converted to level
        Stack<LevelStructure> levelStructures = new Stack<LevelStructure>();
        //just for checking the lattice (if dead end happens, algo will pop levelStructures untill it can continue.
        List<LevelStructure> allLevelStructures = new List<LevelStructure>();
        int x = 0;
        int y = 0;
        int[] empty;
        int direction;
        LevelStructure structure = null;
        while (levelStructures.Count < levelCount)
        {
            List<int> unblocked = new List<int>();

            if (levelStructures.Count == 0)
            {
                unblocked.Add(0);
                unblocked.Add(1);
                unblocked.Add(2);
                unblocked.Add(3);
            }
            else
            {
                empty = GetEmpty(x, y, allLevelStructures);
                if (empty[0] != allLevelStructures[allLevelStructures.Count()-1].cellHeight)
                    unblocked.Add(0);
                if (empty[1] != allLevelStructures[allLevelStructures.Count() - 1].cellWidth)
                    unblocked.Add(1);
                if (empty[2] != 1)
                    unblocked.Add(2);
                if (empty[3] != 1)
                    unblocked.Add(3);
            }
            
            
            if (unblocked.Count == 0)
            {
                currentId--;
                levelStructures.Pop();
                levelStructures.Peek().PopExit();
                structure = levelStructures.Peek();
                x = structure.x;
                y = structure.y;
                continue;
            }
            allLevelStructures = new List<LevelStructure>(levelStructures);
            direction = unblocked[ToolsGame.Rng(unblocked.Count)];

            
            MoveInDirection(ref x, ref y, ref structure, allLevelStructures, probabilityOfLargerCell, maxLevelSize, keepLinear, direction, currentId, cellSize);

            levelStructures.Push(structure);
            allLevelStructures.Add(structure);
            currentId++;
        }
        var arr = levelStructures.ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            levels.Add(gameControlReference.Id, CreateLevel(ToolsMath.GetRandomElement<ILevelGenerator>(levelGenerators), gameControlReference, biom, arr[arr.Length - 1 - i].cellWidth * cellSize, arr[arr.Length - 1 - i].cellHeight * cellSize, arr[arr.Length - 1 - i].ExitsAr));
        }
        return levels;
    }
    




}

