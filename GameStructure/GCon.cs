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
    public static IInteractable lastInteractable;
    public static ActionHandler menuActionHandler = new ActionHandler(true, ToolsSystem.PauseType.Menu);

    public static Dictionary<ToolsSystem.PauseType, GameSystem> gameSystems = new() { { ToolsSystem.PauseType.Menu, new GameSystem(() => {}) } };

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
    private static Stack<ToolsSystem.PauseType> paused = new Stack<ToolsSystem.PauseType>();

    public static void AddPausedType(ToolsSystem.PauseType type)
    {
        if (paused.Count != 0)
        {
            gameSystems[paused.Peek()]?.OnDeactivation();
        }
        paused.Push(type);
        gameSystems[paused.Peek()].OnActivation();
        ToolsGame.PausePausables(paused.Peek());
        ToolsSystem.ReleaseAllKeys();
    }
    public static ToolsSystem.PauseType GetPausedType()
    {
        return paused.Count == 0 ? ToolsSystem.PauseType.Menu : paused.Peek();
    }
    public static void PopPausedType()
    {
        gameSystems[paused.Peek()]?.OnDeactivation();
        paused.Pop();
        gameSystems[paused.Peek()]?.OnActivation();
        ToolsGame.PausePausables(paused.Peek());
        ToolsSystem.ReleaseAllKeys();
    }

    public static float frameTime = Time.fixedDeltaTime;

}
