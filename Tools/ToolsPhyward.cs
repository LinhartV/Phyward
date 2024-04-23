using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
            int numberOfDrops = ToolsGame.Rng(drop.minCount, drop.maxCount + 1);
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
}

