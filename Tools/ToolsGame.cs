//#define PREDICTED

using System;
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
    /// <summary>
    /// For each element of PlayerActionEnum assign lambda function for keyPress and keyRelease by name.
    /// </summary>
    public static Dictionary<PlayerActionsEnum, (Action, Action)> actions = new();

    public enum PlayerActionsEnum { none = 0, moveUp = 1, moveDown = 2, moveLeft = 3, moveRight = 4, fire = 5, abilityQ = 6, abilityE = 7, other = 8, cheat = 9 }

    public enum MazeTile
    {
        counted, //only for maze setup, don't use
        potentialBlock, //only for maze setup, don't use
        empty,
        block
    }


    public static void CreateGame()
    {
        GCon.game.bioms.Add(new MeadowBiom("just.mp3", new LinearGenerator(5, 20, 0, false, 1, new BlockInsert(0.125f, new RandomSpawner((() => { return new TimeEnemy(1); }, 3), (() => { return new MassEnemy(1); }, 1))))));


        GCon.game.CurBiom = GCon.game.bioms[0];
        GCon.game.CurLevel = GCon.game.CurBiom.levels[0];
        GCon.game.Player = (new Player(GCon.game.CurLevel.GetEmptyPosition(), 4f, 50, 25f, new BasicWeapon(ToolsMath.SecondsToFrames(1), 10, 5, ToolsMath.SecondsToFrames(1), GameObjects.redSmallShot), 1, 1, 1, 1, 100));

    }

    public static void DefaultAssignOfKeys()
    {
        KeyController.AddKey(KeyCode.W, new RegisteredKey(PlayerActionsEnum.moveUp), KeyCode.UpArrow);
        KeyController.AddKey(KeyCode.S, new RegisteredKey(PlayerActionsEnum.moveDown), KeyCode.DownArrow);
        KeyController.AddKey(KeyCode.D, new RegisteredKey(PlayerActionsEnum.moveRight), KeyCode.RightArrow);
        KeyController.AddKey(KeyCode.A, new RegisteredKey(PlayerActionsEnum.moveLeft), KeyCode.LeftArrow);
        KeyController.AddKey(KeyCode.C, new RegisteredKey(PlayerActionsEnum.cheat));
        KeyController.AddKey(KeyCode.E, new RegisteredKey(PlayerActionsEnum.abilityE));
        KeyController.AddKey(KeyCode.Q, new RegisteredKey(PlayerActionsEnum.abilityQ));
        KeyController.AddKey(KeyCode.Space, new RegisteredKey(PlayerActionsEnum.fire), KeyCode.Mouse0);
    }

    public static void SetupActions()
    {
        actions.Add(PlayerActionsEnum.moveUp, (() =>
        {
            GCon.game.Player.AddAction(new ItemAction("up", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        }, () =>
        {
            GCon.game.Player.DeleteAction("up");
        }
        ));
        actions.Add(PlayerActionsEnum.moveDown, (() =>
        {
            GCon.game.Player.AddAction(new ItemAction("down", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        }, () =>
        {
            GCon.game.Player.DeleteAction("down");
        }
        ));
        actions.Add(PlayerActionsEnum.moveLeft, (() =>
        {
            GCon.game.Player.AddAction(new ItemAction("left", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        }, () =>
        {
            GCon.game.Player.DeleteAction("left");
        }
        ));
        actions.Add(PlayerActionsEnum.moveRight, (() =>
        {
            GCon.game.Player.AddAction(new ItemAction("right", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        }, () =>
        {
            GCon.game.Player.DeleteAction("right");
        }
        ));
        actions.Add(PlayerActionsEnum.fire, (() =>
        {
            if (GCon.game.Player.Weapon != null)
            {

                if (GCon.game.Player.Weapon.Reloaded)
                {
                    if (GCon.game.Player.Weapon.AutoFire)
                    {
                        GCon.game.Player.AddAction(new ItemAction("fire1", GCon.game.Player.Weapon.ReloadTime * GCon.game.Player.CharReloadTime, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning), "fire");
                    }
                    else
                    {
                        GCon.game.Player.Weapon.Fire();
                        GCon.game.Player.Weapon.Reloaded = false;
                        GCon.game.Player.AddAction(new ItemAction("fire2", GCon.game.Player.Weapon.ReloadTime * GCon.game.Player.CharReloadTime, ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning), "fire");
                    }
                }
                else
                {
                    GCon.game.Player.Weapon.Reloaded = true;
                }
            }

        }, () =>
        {
            if (GCon.game.Player.Weapon != null)
            {
                GCon.game.Player.Weapon.Reloaded = false;
            }
        }
        ));
    }

    public static void SetupGame()
    {
        DefaultAssignOfKeys();
        SetupActions();
        ToolsPhyward.InstantiateEnemyInfos();
        LambdaActions.SetupLambdaActions();

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
    public static float Rng()
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
        return (float)num;
#endif

    }

}
