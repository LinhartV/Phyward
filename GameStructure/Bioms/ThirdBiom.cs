using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static IBiomGenerator;
using static Tutorial;

public static class ThirdBiom
{
    static CustomGenerator customGenerator;
    static Biom biom;
    public static Biom GetThirdBiom()
    {
        customGenerator = new CustomGenerator(5);

        var generator1 = new BlockInsert(0.125f, new RandomSpawner((() => { return new TimeEnemy(1); }, 3), (() => { return new MassEnemy(1); }, 2)));
        var generator2 = new BlockInsert(0.3, new RandomSpawner((() => { return new ShootingEnemy(1); }, 2), (() => { return new SwarmEnemy(1); }, 1)));
        var generator3 = new BlockInsert(0, new RandomSpawner((() => { return new SwarmEnemy(1); }, 3)));
        var generator4 = new BlockInsert(0.5, new RandomSpawner((() => { return new MassEnemy(1); }, 2), (() => { return new TimeEnemy(1); }, 4)));
        var generator5 = new BlockInsert(0.1, new RandomSpawner((() => { return new MassEnemy(1); }, 1), (() => { return new TimeEnemy(1); }, 1), (() => { return new SwarmEnemy(1); }, 1), (() => { return new ShootingEnemy(1); }, 1)));
        var generator6 = new BlockInsert(0.05, new RandomSpawner((() => { return new TimeEnemy(1); }, 2), (() => { return new SwarmEnemy(1); }, 2)));
        var generator7 = new BlockInsert(0.1, new RandomSpawner((() => { return new ShootingEnemy(1); }, 3), (() => { return new MassEnemy(1); }, 2)));


        biom = new Biom("just.mp3", 2, new LinearGenerator(40, 10, 0.6, false, 6, generator1, generator2, generator3, generator4, generator5, generator6, generator7));

        biom.levels[0].ClearLevel();
        for (int i = 0; i < biom.levels.Count; i++)
        {
            if (i % 5 == 0)
            {
                biom.levels[i].AddItem(new Base(biom.levels[i].GetEmptyPosition()));
            }
        }

        biom.levels[2].ClearLevel();
        biom.levels[2].AddItem(new Boss2(1.2f, new List<ToolsPhyward.Drop>() {
                new ToolsPhyward.Drop(1, 1, 1, () => { return new Collectable(new Scroll(Units.force)); })
            }), biom.levels[2].GetEmptyPosition());
        biom.levels[2].AddItem(new SwarmEnemy(1.5f), biom.levels[2].GetEmptyPosition());
        biom.levels[2].AddItem(new SwarmEnemy(1.5f), biom.levels[2].GetEmptyPosition());

        biom.levels[0].AddItem(new TutorialTrigger(new TutorialBlock("Závěrečná výzva", "Tato úroveň je... dá se říci nekonečná.", "Vyhraješ ji tím, že vyrobíš portál ze Síly (kterou musíš nejprve najít a vyrobit), použiješ ho... a přežiješ.")), biom.levels[0].GetOutExitPosition());



        //EndBoss
        Queue<LevelStructure> levelStructures = new Queue<LevelStructure>();
        int x = 0;
        int y = 0;
        IBiomGenerator.LevelStructure str = null;
        //Intro
        customGenerator.MoveInDirection(ref x, ref y, ref str, 1, 40, 1, 1);
        levelStructures.Enqueue(str);
        //Small crossroads
        customGenerator.MoveInDirection(ref x, ref y, ref str, 0, 41, 4, 4);
        levelStructures.Enqueue(str);

        biom.FirstBiomLevel = PreBoss(levelStructures.Dequeue());
        FinalBoss(levelStructures.Dequeue());

        biom.FirstBiomLevel = biom.levels[0];
        GCon.game.Player.InsertAtPosition(biom.levels[0].GetEmptyPositionAtFarEnd(), true);


        return biom;
    }
    private static Level PreBoss(LevelStructure str)
    {
        Level l = customGenerator.CreateLevel(new BlockInsert(0f), biom, str);
        l.AddItem(new InvisibleBlock(l.GetOutExitPosition()));
        l.AddItem(new Base(new Vector2(3, 3)));
        l.AddItem(new TutorialTrigger(new TutorialBlock("VHEGR - poslední nepřítel", "Závěrečný nepřítel jménem VHEGR se s tebou rozhodl utkat.", "Úkol zní jasně - poraž ho!")), new Vector2(3, 3));

        return l;
    }
    private static Level FinalBoss(LevelStructure str)
    {
        Level l = customGenerator.CreateLevel(new BlockInsert(0f), biom, str);

        l.AddItem(new FinalBoss(1), new Vector2(l.Maze.GetLength(1) / 2, l.Maze.GetLength(0) - 3));
        l.AddItem(new Block(l.GetOutExitPosition(true), GameObjects.block));
        l.AddOnEnterAction("moveOneTileUp");

        return l;
    }
}

