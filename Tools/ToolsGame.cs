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
        GCon.game.bioms.Add(ToolsGame.BiomType.meadow, new Biom("just.mp3", ToolsGame.BiomType.meadow, new LinearGenerator(3, 5, 0.5, false, 4, new BlockInsert(0.1)), GCon.game));


        GCon.game.CurBiom = GCon.game.bioms[ToolsGame.BiomType.meadow];
        GCon.game.CurLevel = GCon.game.CurBiom.levels[0];
        GCon.game.Player = (new Player(GCon.game.CurLevel.GetEmptyPosition(), 12.5f, 10, 5.5f, new BasicWeapon(20, 1, 0.01f, 500, GameObjects.blueShot), 1, 1, 1, 1, 100));

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
            GCon.game.Player.AddAction(new ItemAction("up", 1));
        }, () =>
        {
            GCon.game.Player.DeleteAction("up");
        }
        ));
        actions.Add(PlayerActionsEnum.moveDown, (() =>
        {
            GCon.game.Player.AddAction(new ItemAction("down", 1));
        }, () =>
        {
            GCon.game.Player.DeleteAction("down");
        }
        ));
        actions.Add(PlayerActionsEnum.moveLeft, (() =>
        {
            GCon.game.Player.AddAction(new ItemAction("left", 1));
        }, () =>
        {
            GCon.game.Player.DeleteAction("left");
        }
        ));
        actions.Add(PlayerActionsEnum.moveRight, (() =>
        {
            GCon.game.Player.AddAction(new ItemAction("right", 1));
        }, () =>
        {
            GCon.game.Player.DeleteAction("right");
        }
        ));
        actions.Add(PlayerActionsEnum.fire, (() =>
        {
            if (GCon.game.Player.Weapon.Reloaded)
            {
                if (GCon.game.Player.Weapon.AutoFire)
                {
                    GCon.game.Player.AddAction(new ItemAction("fire1", GCon.game.Player.Weapon.ReloadTime * GCon.game.Player.CharReloadTime, ItemAction.ExecutionType.EveryTime), "fire");
                }
                else
                {
                    GCon.game.Player.Weapon.Fire();
                    GCon.game.Player.Weapon.Reloaded = false;
                    GCon.game.Player.AddAction(new ItemAction("fire2", GCon.game.Player.Weapon.ReloadTime * GCon.game.Player.CharReloadTime, ItemAction.ExecutionType.OnlyFirstTime), "fire");
                }
            }
            else
            {
                GCon.game.Player.Weapon.Reloaded = true;
            }

        }, () =>
        {
            GCon.game.Player.Weapon.Reloaded = false;
        }
        ));
        /*//Movements
        actions.Add(PlayerActionsEnum.moveUp, new KeyCommand((id, gvars) =>
        {
            gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("up", 1, ItemAction.ExecutionType.EveryTime, true));
            //AddDelayedAction(gvars, id, "up", delay);
        },
        (id, gvars) =>
        {
            gvars.ItemsPlayers[id].DeleteAction("up");
        }));

        actions.Add(PlayerActionsEnum.moveDown, new KeyCommand((id, gvars) =>
        {
            gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("down", 1, ItemAction.ExecutionType.EveryTime, true));
            //AddDelayedAction(gvars, id, "down", delay);
        },
        (id, gvars) =>
        {
            gvars.ItemsPlayers[id].DeleteAction("down");
        }));

        actions.Add(PlayerActionsEnum.moveRight, new KeyCommand((id, gvars) =>
        {
            gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("right", 1, ItemAction.ExecutionType.EveryTime, true));
            //AddDelayedAction(gvars, id, "right", delay);
        },
        (id, gvars) =>
        {
            gvars.ItemsPlayers[id].DeleteAction("right");
        }));

        actions.Add(PlayerActionsEnum.moveLeft, new KeyCommand((id, gvars) =>
        {
            gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("left", 1, ItemAction.ExecutionType.EveryTime, true));
            //AddDelayedAction(gvars, id, "left", delay);
        },
        (id, gvars) =>
        {
            gvars.ItemsPlayers[id].DeleteAction("left");
        }));

        actions.Add(PlayerActionsEnum.cheat, new KeyCommand((id, gvars) =>
        {
            gvars.Cheating = !gvars.Cheating;
            gvars.ItemsPlayers[id].IncreaseScore(1000);
            gvars.ItemsPlayers[id].Immortal = !gvars.ItemsPlayers[id].Immortal;
            if (gvars.Cheating)
            {
                foreach (var item in gvars.Items.Values)
                {
                    if (item is Enemy)
                    {
                        item.Dispose();
                    }
                }
            }
            else
            {
                LambdaActions.ExecuteAction("createBoss", gvars, -1);
            }

        },
        (id, gvars) =>
        {

        }));

        actions.Add(PlayerActionsEnum.abilityQ, new KeyCommand((id, gvars) =>
        {
            gvars.ItemsPlayers[id].AbilityQ?.UseAbility(gvars, id);
        },
        (id, gvars) =>
        {

        }));
        actions.Add(PlayerActionsEnum.abilityE, new KeyCommand((id, gvars) =>
        {
            gvars.ItemsPlayers[id].AbilityE?.UseAbility(gvars, id);
        },
       (id, gvars) =>
       {

       }));*/
    }

    public static void SetupGame()
    {
        DefaultAssignOfKeys();
        SetupActions();
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
