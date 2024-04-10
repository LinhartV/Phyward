using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Generates biom levels as a tree graph 
/// Can create levels of larger cells that will fit the lattice (no two levels will overlap)
/// </summary>
public class TreeGenerator : IBiomGenerator
{

    private ILevelGenerator[] levelGenerators;
    private int levelCount;
    private int cellSize;
    private double probabilityOfLargerCell;
    private int maxLevelSize;
    private bool keepLinear;
    private double algoStrength;
    public TreeGenerator(double algoStrength, int levelCount, int cellSize, double probabilityOfLargerCell, bool keepLinear, int maxLevelSize, ILevelGenerator levelGenerator, params ILevelGenerator[] levelGenerators)
    {
        var list = levelGenerators.ToList();
        list.Add(levelGenerator);
        this.levelGenerators = list.ToArray();
        this.levelCount = levelCount;
        this.cellSize = cellSize;
        this.maxLevelSize = maxLevelSize;
        this.probabilityOfLargerCell = probabilityOfLargerCell;
        this.keepLinear = keepLinear;
        this.algoStrength = algoStrength;
    }

    override public Dictionary<int, Level> GenerateBiom(Biom biom, GameControl gameControlReference)
    {
        int currentId = gameControlReference.IdLevels;
        Dictionary<int, Level> levels = new Dictionary<int, Level>();
        //just for checking the lattice (if dead end happens, algo will pop levelStructures untill it can continue.
        List<LevelStructure> levelStructures = new List<LevelStructure>();
        int x = 0;
        int y = 0;
        int[] empty;
        int direction;
        double rand = ToolsGame.Rng();
        LevelStructure structure = null;
        while (levelStructures.Count < levelCount)
        {
            List<int> unblocked = new List<int>();
            empty = GetEmpty(x, y, levelStructures);
            for (int j = 0; j < empty.Length; j++)
            {
                if (empty[j] != 1)
                {
                    unblocked.Add(j);
                }
            }
            if (unblocked.Count == 0)
            {
                //do sth
            }
            direction = unblocked[ToolsGame.Rng(unblocked.Count)];
            MoveInDirection(ref x, ref y, ref structure, levelStructures, probabilityOfLargerCell, maxLevelSize, keepLinear, direction, currentId, cellSize);


            levelStructures.Add(structure);
            currentId++;
        }
        var arr = levelStructures.ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            levels.Add(gameControlReference.IdLevels, CreateLevel(ToolsMath.GetRandomElement<ILevelGenerator>(levelGenerators), gameControlReference, biom, arr[arr.Length - 1 - i].cellWidth * cellSize, arr[arr.Length - 1 - i].cellHeight * cellSize, arr[arr.Length - 1 - i].ExitsAr));
        }
        return levels;
    }





}

