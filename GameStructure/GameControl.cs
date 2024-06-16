using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using static ToolsGame;

/// <summary>
/// Class for controlling game of particular player account
/// </summary>
[Serializable]
public class GameControl
{
    public GameControl() { }
    public string PlayerName { get; set; }
    //public Dictionary<int, Level> Levels { get;private set; } = new Dictionary<int, Level>();
    /// <summary>
    /// Increasing id to be assigned for levels during creation
    /// </summary>
    public int IdLevels { get; set; } = 0;
    /// <summary>
    /// Increasing id to be assigned for levels during creation
    /// </summary>
    public int IdItems { get; set; } = 0;
    public long Now { get; set; } = 0;
    public List<Biom> bioms = new List<Biom>();

    /// <summary>
    /// List of items to be destroyed at the end of this frame
    /// </summary>
    public List<ActionHandler> ItemsToBeDestroyed { get; set; } = new List<ActionHandler>();
    /// <summary>
    /// /// <summary>
    /// List of items to be set inactive at the end of this frame
    /// </summary>
    public List<Item> ItemsToBeSetInactive { get; set; } = new List<Item>();
    /// <summary>
    /// List of all items in the game
    /// </summary>
    public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();
    /// <summary>
    /// All items where ExecuteActions will be triggered
    /// </summary>
    private Dictionary<ToolsSystem.PauseType, GameSystem> itemStep = new();

    public Biom CurBiom { get; set; }

    public ActionHandler gameActionHandler;
    public Level CurLevel { get; set; }
    public Player Player { get; set; }
    public GameControl(string playerName)
    {
        PlayerName = playerName;
        itemStep.Add(ToolsSystem.PauseType.InGame, new GameSystem());
        itemStep.Add(ToolsSystem.PauseType.Inventory, new GameSystem(() =>
        {
            foreach (var item in ToolsUI.UIItems)
            {
                item.OnLevelEnter();
            }
        }, () =>
        {
            foreach (var item in ToolsUI.UIItems)
            {
                item.OnLevelLeave();
            }
        }));
        itemStep.Add(ToolsSystem.PauseType.Animation, new GameSystem());
    }

    public void ActivateThisGame()
    {
        GCon.gameSystems.AddRange(GCon.game.itemStep);
    }

}

