using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class for controlling entire Phyward (GameControl)
/// </summary>
public static class GCon
{
    public static Dictionary<string, GameControl> games = new Dictionary<string, GameControl>();
    /// <summary>
    /// Game of the selected account
    /// </summary>
    public static GameControl game;
    public static bool freezeCamera = false;
    private static bool gameStarted = false;
    public static ActionHandler gameActionHandler;
    public static IInteractable lastInteractable;
    public static bool GameStarted
    {
        get
        {
            return gameStarted;
        }
        set
        {
            if (value == true)
            {
                ToolsGame.SetupGameAfterLoad();
            }
            gameStarted = value;
        }
    }
    private static bool paused = false;
    public static bool Paused
    {
        get
        {
            return paused;
        }
        set
        {
            if (value == true)
            {
                ToolsGame.PausePausables(true);
                ToolsSystem.ReleaseAllKeys();
                foreach (var item in ToolsUI.UIItems)
                {
                    item.OnLevelEnter();
                }
            }
            else
            {
                ToolsGame.PausePausables(false);
                foreach (var item in ToolsUI.UIItems)
                {
                    item.OnLevelLeave();
                }
            }
            paused = value;
        }
    }
    public static float percentageOfFrame = Time.fixedDeltaTime;

}
