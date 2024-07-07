using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ToolsGame;

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
    /// <summary>
    /// empty positions (X, Y)
    /// </summary>
    [JsonProperty]
    private List<(int, int)> emptyPos = new List<(int, int)>();
    [JsonProperty]
    private long leftNow;
    [JsonProperty]
    private List<string> onEnterActions = new List<string>();
    public Level(int id, ILevelGenerator generator, int width, int height, List<PreExit>[] exits, int lastAddetExitDirection, int firstAddedExitDirection) : base(false)
    {
        Id = id;
        this.ExitsAr = exits;
        Maze = generator.GenerateLevel(width, height, exits, out emptyPos, this);
        generator.SpawnItems(this);
        this.LastAddedExitDirection = lastAddetExitDirection;
        this.FirstAddedExitDirection = firstAddedExitDirection;
        //AddItem(GCon.game.Player);
    }

    public void ClearLevel()
    {
        for (int i = 0; i < Maze.GetLength(0); i++)
        {
            for (int j = 0; j < Maze.GetLength(1); j++)
            {
                if (i == 0 || i == Maze.GetLength(0) - 1 || j == 0 || j == Maze.GetLength(1) - 1)
                {
                    Maze[i, j] = MazeTile.block;
                }
                else
                {
                    Maze[i, j] = MazeTile.empty;
                    emptyPos.Add((i, j));
                }
            }
        }
        int x = 0;
        int y = 0;
        for (int i = 0; i < ExitsAr.Length; i++)
        {
            for (int j = 0; j < ExitsAr[i].Count; j++)
            {
                if (i == 2)
                {
                    y = 0;
                    x = ExitsAr[i][j].Position + 1;
                }
                if (i == 1)
                {
                    y = ExitsAr[i][j].Position + 1;
                    x = Maze.GetLength(1) - 1;
                }
                if (i == 0)
                {
                    y = Maze.GetLength(0) - 1;
                    x = ExitsAr[i][j].Position + 1;
                }
                if (i == 3)
                {
                    y = ExitsAr[i][j].Position + 1;
                    x = 0;
                }
                Maze[y, x] = MazeTile.empty;
            }
        }
        var temp = new Dictionary<int, Item>(Items);
        foreach (var item in temp.Values)
        {
            if (!(item is Exit))
            {
                Items.Remove(item.Id);
                item.Dispose();
            }
        }

    }

    public Item AddItem(Item item, Vector2 pos)
    {
        AddItem(item);
        item.InsertAtPosition(pos);
        return item;
    }
    public Item AddItem(Item item)
    {
        item.Prefab.SetActive(GCon.game.CurLevel == this && GCon.GameStarted);

        if (GCon.game.CurLevel == this)
        {
            item.IsInLevel = true;
            item.OnLevelEnter();
        }
        Items.Add(item.Id, item);
        return item;
    }
    public void DestroyAllItemsOfType<T>()
    {
        foreach (var item in Items.Values)
        {
            if (item is T)
            {
                item.Dispose();
            }
        }
    }

    public List<T> GetAllItemsOfType<T>()
    {
        List<T> list = new List<T>();
        foreach (var item in Items.Values)
        {
            if (item is T t)
            {
                list.Add(t);
            }
        }
        return list;
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
    public Vector2 GetEmptyPosition(bool blocking = false)
    {
        var index = Rng(0, emptyPos.Count);
        var position = emptyPos.Count > 0 ? emptyPos[index] : (-1, -1);
        if (blocking)
        {
            emptyPos.RemoveAt(index);
        }

        return new Vector2(position.Item2, position.Item1);
    }
    /// <summary>
    /// Gets empty position farthest from the exit
    /// </summary>
    /// <param name="blocking"></param>
    /// <returns></returns>
    public Vector2 GetEmptyPositionAtFarEnd(bool blocking = false)
    {
        double max = 0;
        PreExit exit = null;
        bool doubleBreak = false;
        for (int i = 0; i < ExitsAr.Length; i++)
        {
            for (int j = 0; j < ExitsAr[i].Count; j++)
            {
                exit = ExitsAr[i][j];
                /*doubleBreak = true;
                break;*/
            }
            /*if (doubleBreak)
                break;*/
        }
        double dist;
        int index = 0;
        for (int i = 0; i < emptyPos.Count; i++)
        {
            dist = Math.Sqrt(Math.Pow(emptyPos[i].Item1 - exit.Y, 2) + Math.Pow(emptyPos[i].Item2 - exit.X, 2));
            if (dist > max)
            {
                max = dist;
                index = i;
            }
        }
        if (blocking)
        {
            emptyPos.RemoveAt(index);
        }

        return new Vector2(emptyPos[index].Item2, emptyPos[index].Item1);
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
        foreach (var action in onEnterActions)
        {
            LambdaActions.ExecuteAction(action, null);
        }
    }
    public void OnLeave()
    {
        leftNow = GCon.game.Now;
    }
    public void AddOnEnterAction(string action)
    {
        onEnterActions.Add(action);
    }

}
