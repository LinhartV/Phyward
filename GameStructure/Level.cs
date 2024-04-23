using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ToolsGame;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Instance of a level
/// </summary>
[Serializable]
public class Level : ExitHandler
{
    public Level()
    {
    }

    public MazeTile[,] Maze { get; set; }
    [JsonProperty]
    public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();
    [JsonProperty]
    private List<(int, int)> emptyPos = new List<(int, int)>();
    [JsonProperty]
    private long leftNow;
    public Level(int id, ILevelGenerator generator, int width, int height, List<PreExit>[] exits) : base(false)
    {
        Id = id;
        this.ExitsAr = exits;
        Maze = generator.GenerateLevel(width, height, exits, out emptyPos, this);
        generator.SpawnItems(this);
        //AddItem(GCon.game.Player);
    }

    public void AddItem(Item item)
    {
        item.Prefab.SetActive(GCon.game.CurLevel == this && GCon.gameStarted);
        Items.Add(item.Id, item);
    }


    /// <summary>
    /// Gets a random tile in maze
    /// </summary>
    /// <returns>A position of the maze</returns>
    public (int, int) GetAnyPosition()
    {
        return (Rng(0, Maze.GetLength(1)), Rng(0, Maze.GetLength(0)));
    }
    /// <summary>
    /// Gets an empty random tile in maze
    /// </summary>
    /// <param name="blocking">Whether to set the tile as blocked (thus removing it from empty positions) - should not be wall as it could block path.</param>
    /// <returns>Empty position of the maze</returns>
    public (int, int) GetEmptyPosition(bool blocking = false)
    {
        var index = Rng(0, emptyPos.Count);
        var position = emptyPos.Count > 0 ? emptyPos[index] : (-1, -1);
        if (blocking)
        {
            emptyPos.RemoveAt(index);
        }

        return position;
    }

    public void OnEnter()
    {
        if (leftNow != 0)
        {
            foreach (Item item in Items.Values)
            {
                item.ChangeNowDifference(GCon.game.Now - leftNow);
            }
        }
    }
    public void OnLeave()
    {
        leftNow = GCon.game.Now;
    }

}
