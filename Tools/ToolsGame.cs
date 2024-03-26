//#define PREDICTED

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Tools for game management
/// </summary>
public static class ToolsGame
{
    private static System.Random rnd = new System.Random();
    public static Queue<double> pseudoRand = new Queue<double>();
    private const string PATH = "./rng.txt";

    public enum BiomType
    {
        meadow,
        dungeonEasy,
        dungeonHard,
        forest,
    }
    public enum MazeTile
    {
        counted, //only for maze setup, don't use
        potentialBlock, //only for maze setup, don't use
        empty,
        block
    }
    public static void CreateGame()
    {
        GlobalControl.game.Player = (new Player((0,0), 5.5f, new BasicWeapon(1,1,1), 1, 1, 1, 100));
        GlobalControl.game.bioms.Add(ToolsGame.BiomType.meadow, new Biom("just.mp3", ToolsGame.BiomType.meadow, new LinearGenerator(3, 2, 1, false, 5, new BlockInsert(0.01)), GlobalControl.game));


        GlobalControl.game.CurBiom = GlobalControl.game.bioms[ToolsGame.BiomType.meadow];
        GlobalControl.game.CurLevel = GlobalControl.game.CurBiom.levels[0];
    }

    public static void SetupGame()
    {
#if PREDICTED
        string[] txt = File.ReadAllLines(PATH);
        for (int i = 0; i < txt.Length; i++)
        {
            pseudoRand.Enqueue(System.Convert.ToDouble(txt[i]));
        }
#else
        File.WriteAllText(PATH, "");
#endif
    }
    public static int Rng(int lowerBound, int upperBound)
    {
#if PREDICTED
        if (pseudoRand.Count > 0)
        {
            return (int)pseudoRand.Dequeue();
        }
        else
        {
            int num = rnd.Next(lowerBound, upperBound);
            string destination = PATH;
            File.AppendAllText(destination, num.ToString() + "\n");
            return num;
        }
#else
        int num = rnd.Next(lowerBound, upperBound);
        string destination = PATH;
        File.AppendAllText(destination, num.ToString() + "\n");
        return num;
#endif

    }
    public static int Rng(int upperBound)
    {
        return Rng(0, upperBound);
    }
    public static double Rng()
    {
#if PREDICTED
        if (pseudoRand.Count > 0)
        {
            return pseudoRand.Dequeue();
        }
        else
        {
            double num = rnd.NextDouble();
            string destination = PATH;
            File.AppendAllText(destination, num.ToString() + "\n");
            return num;
        }
#else
        double num = rnd.NextDouble();
        string destination = PATH;
        File.AppendAllText(destination, num.ToString() + "\n");
        return num;
#endif

    }

}
