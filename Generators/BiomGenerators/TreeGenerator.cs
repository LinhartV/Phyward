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
    public TreeGenerator(int levelCount, int cellSize, double probabilityOfLargerCell, bool keepLinear, int maxLevelSize, ILevelGenerator levelGenerator, params ILevelGenerator[] levelGenerators) : base(levelCount, cellSize, probabilityOfLargerCell, keepLinear, maxLevelSize, levelGenerator, levelGenerators)
    {
    }

    override public void GenerateBiom(Biom biom)
    {
        int currentId = GCon.game.IdLevels;
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

        GenerateLevelsFromStructures(arr, biom);
    }





}

