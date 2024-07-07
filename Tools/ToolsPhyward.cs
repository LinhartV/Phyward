using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ToolsGame;
using UnityEngine.Tilemaps;
/// <summary>
/// Tools and classes asosiated with Phyward itself (whatever that means)
/// </summary>
public static class ToolsPhyward
{
    [Serializable]
    public class Drop
    {
        public int minCount;
        public int maxCount;
        public float probability;
        public Func<Collectable> collectable;

        public Drop(int minCount, int maxCount, float probability, Func<Collectable> collectable)
        {
            this.minCount = minCount;
            this.maxCount = maxCount;
            this.probability = probability;
            this.collectable = collectable;
        }
    }
    public static void EnterLevel(Level lvl)
    {

        GCon.freezeCamera = true;
        GCon.game.gameActionHandler.AddAction(new ItemAction("unfreeze", 1, ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning));
        ToolsPhyward.BuildLevel(lvl, GCon.game.CurLevel, new Vector2(-1,-1));
        GCon.game.CurLevel.OnLeave();
        GCon.game.CurLevel = lvl;
        lvl.OnEnter();
    }
    public static void EnterLevel(Level lvl, Vector2 pos)
    {

        GCon.freezeCamera = true;
        GCon.game.gameActionHandler.AddAction(new ItemAction("unfreeze", 1, ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning));
        ToolsPhyward.BuildLevel(lvl, GCon.game.CurLevel, pos);
        GCon.game.CurLevel.OnLeave();
        GCon.game.CurLevel = lvl;
        lvl.OnEnter();
    }

    public static void DropDrops(List<Drop> drops, (float, float) pos)
    {
        float offset = 0.6f;
        List<Collectable> list = new List<Collectable>();
        foreach (var drop in drops)
        {
            if (ToolsGame.Rng() > drop.probability)
            {
                continue;
            }
            int numberOfDrops = ToolsGame.Rng(drop.minCount, drop.maxCount);
            for (int i = 0; i < numberOfDrops; i++)
            {
                Collectable c = drop.collectable();
                GCon.game.CurLevel.AddItem(c);
                list.Add(c);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list.Count == 1)
            {
                list[i].InsertAtPosition(pos, true);
            }
            else
            {
                list[i].InsertAtPosition((pos.Item1 + offset * (float)Math.Sin(i * 2 * Math.PI / list.Count), pos.Item2 + offset * (float)Math.Cos(i * 2 * Math.PI / list.Count)), true);
            }
        }
    }
    public static void DropDrops(List<Drop> drops, Vector2 pos)
    {
        DropDrops(drops, (pos.y, pos.x));
    }

    [Serializable]
    public class EnemyInfo
    {
        public EnemyInfo() { }

        public EnemyInfo(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
    public static Dictionary<Type, EnemyInfo> enemyInfos = new Dictionary<Type, EnemyInfo>();
    public static void InstantiateEnemyInfos()
    {
        enemyInfos.Add(typeof(TimeEnemy), new EnemyInfo("Clocker", "Bytost, jejíž život je závislý na tikání hodin"));
    }



    public static void BuildLevel(Level level, Level prevLevel, Vector2 playerPos)
    {
        if (level == null)
        {
            return;
        }
        Tilemap solidMap = GameObjects.solidLayer;
        solidMap.ClearAllTiles();
        MazeTile[,] maze = level.Maze;

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                switch (maze[i, j])
                {
                    case MazeTile.empty:

                        break;
                    case MazeTile.block:
                        InsertTile(GameObjects.wallTile, solidMap, j, i);
                        break;
                    default:
                        break;
                }
            }
        }
        if (prevLevel != null)
        {
            var temp = new List<Item>(prevLevel.Items.Values);
            foreach (var obj in temp)
            {
                obj.OnLevelLeave();
                if (!obj.IsInLevel)
                {
                    GCon.game.ItemsToBeSetInactive.Add(obj);
                }
            }
        }
        foreach (var obj in level.Items.Values)
        {
            obj.Prefab.SetActive(true);
            obj.OnLevelEnter();
        }
        if (playerPos != new Vector2(-1,-1))
        {
            GCon.game.Player.Prefab.transform.position = new Vector3(playerPos.x, playerPos.y, GCon.game.Player.Prefab.transform.position.z);
        }
    }
    public static void InsertTile(TileBase tile, Tilemap map, int x, int y)
    {
        var tilePosition = map.WorldToCell(new Vector3(x, y));
        map.SetTile(tilePosition, tile);
    }



}

