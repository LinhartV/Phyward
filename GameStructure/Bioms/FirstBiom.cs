using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static IBiomGenerator;
using static Tutorial;

public static class FirstBiom
{
    private static IBiomGenerator generator;
    private static Biom biom;
    public static Biom GetFirstBiom()
    {
        Queue<LevelStructure> levelStructures = new Queue<LevelStructure>();
        biom = new Biom("",1);
        int currentId = 0;
        generator = new CustomGenerator(5);
        int x = 0;
        int y = 0;
        int crossroads;
        IBiomGenerator.LevelStructure str = null;
        //Secret
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 1, 1);
        levelStructures.Enqueue(str);
        //Intro
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 1, 1);
        levelStructures.Enqueue(str);
        //First enemy
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 2, 2);
        levelStructures.Enqueue(str);
        //Blowgun
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 1, 1);
        levelStructures.Enqueue(str);
        //First fight
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 2, 2);
        levelStructures.Enqueue(str);
        //Base
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 1, 1);
        levelStructures.Enqueue(str);
        //Crossroads
        crossroads = currentId;
        generator.MoveInDirection(ref x, ref y, ref str, 2, currentId++, 3, 2);
        levelStructures.Enqueue(str);
        //Path down
        generator.MoveInDirection(ref x, ref y, ref str, 2, currentId++, 4, 4);
        levelStructures.Enqueue(str);
        //Down2
        generator.MoveInDirection(ref x, ref y, ref str, 2, currentId++, 2, 4);
        levelStructures.Enqueue(str);
        //Path left
        str = levelStructures.ElementAt(crossroads);
        generator.MoveInDirection(ref x, ref y, ref str, 3, currentId++, 3, 3);
        levelStructures.Enqueue(str);
        //Path right
        str = levelStructures.ElementAt(crossroads);
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 3, 5);
        levelStructures.Enqueue(str);
        //Preboss
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 1, 1);
        levelStructures.Enqueue(str);
        //Boss
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 4, 4);
        levelStructures.Enqueue(str);

        PreLevel(levelStructures.Dequeue());
        biom.FirstBiomLevel = Level1(levelStructures.Dequeue());
        Level2(levelStructures.Dequeue());
        Level3(levelStructures.Dequeue());
        Level4(levelStructures.Dequeue());
        Level5(levelStructures.Dequeue());
        Crossroads(levelStructures.Dequeue());
        PathDown(levelStructures.Dequeue());
        PathDown2(levelStructures.Dequeue());
        PathLeft(levelStructures.Dequeue());
        PathRight(levelStructures.Dequeue());
        PreBossLevel(levelStructures.Dequeue());
        BossLevel(levelStructures.Dequeue());

        GCon.game.TutorialPhase = 0;

        return biom;
    }
    private static Level PreLevel(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0f), biom, str);
        l.AddItem(new TutorialTrigger(new TutorialBlock("Hmmm, tady má někdo rád secrety...", "", ""), true));
        l.AddItem(new Unit(l.GetEmptyPosition(true), Units.Mass()));
        l.AddItem(new Unit(l.GetEmptyPosition(true), Units.Length()));
        l.AddItem(new Unit(l.GetEmptyPosition(true), Units.Length()));
        l.AddItem(new Unit(l.GetEmptyPosition(true), Units.Mass()));
        return l;
    }
    private static Level Level1(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.4f), biom, str);
        l.AddItem(new DestroyableBlock(20,l.GetOutExitPosition(true)));
        l.AddItem(new TutorialTrigger(new TutorialBlock("Vítej ve hře Phyward", "Probudil ses na podlaze v neznámém sklepení, aniž by sis cokoliv pamatoval. Ve snaze najít odpovědi se vydáš kupředu.", "Postavu ovládáš WSAD nebo šipkami."), true));
        GCon.game.Player.InsertAtPosition(l.GetEmptyPositionAtFarEnd(),true);
        return l;
    }
    private static Level Level2(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(1f), biom, str);
        l.AddItem(new MassEnemy(0.6f), l.GetEmptyPositionAtFarEnd(false));
        l.AddItem(new TutorialTrigger(new TutorialBlock("Nepřítel před tebou!", "Chodba je příliš úzká, musíš proběhnout, zbraň zatím nemáš...", ""), true));
        return l;
    }

    private static Level Level3(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.4f), biom, str);
        l.AddItem(new Collectable(CraftedWeapons.Blowgun()), l.GetEmptyPositionAtFarEnd());
        l.AddItem(new TutorialTrigger(new TutorialBlock("Nález zbraně", "Na zemi leží foukačka", "Seber zbraň a stiskni I pro inventář."), true));
        l.AddItem(new DestroyableBlock(15, l.GetOutExitPosition()));
        return l;
    }
    private static Level Level4(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.01f), biom, str);
        l.AddItem(new TimeEnemy(0.6f), l.GetEmptyPositionAtFarEnd());
        l.AddItem(new TimeEnemy(0.6f), l.GetEmptyPositionAtFarEnd());
        l.AddItem(new DestroyableBlock(30, l.GetOutExitPosition()));

        //dropList.AddItem(new TutorialTrigger(new TutorialBlock("První souboj!", "Pokus se neumřít...", "Časoví nepřátelé... Netrvá dlouho a zemřou sami."), true));
        return l;
    }
    private static Level Level5(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.5f), biom, str);
        l.AddItem(new Base(l.GetEmptyPosition()));
        l.AddItem(new TutorialTrigger(new TutorialBlock("Základna!", "Našel jsi opuštěnou základnu - pro teď se stane tvým domovem", "Pro interakci se základnou na ni stoupni a stiskni F."), true));
        l.AddItem(new InvisibleBlock(l.GetOutExitPosition()));
        return l;
    }
    private static Level Crossroads(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(1f), biom, str);
        l.AddItem(new TutorialTrigger(new TutorialBlock("Dobrodružství před tebou!", "Posilněný skvělým tutoriálem vyrážíš vstříc dalším nebezpečím.", "Úroveň vyhraješ, až nalezneš všechny tajné svitky... v této úrovni je jen jeden."), true));
        return l;
    }
    private static Level PathDown(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.2f), biom, str);

        l.AddItem(new MassEnemy(0.6f), l.GetEmptyPosition());
        l.AddItem(new MassEnemy(0.6f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new Unit(l.GetEmptyPositionAtFarEnd(), Units.Mass()));
        l.AddItem(new Unit(l.GetEmptyPosition(), Units.Length()));
        return l;
    }
    private static Level PathDown2(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.1f), biom, str);

        l.AddItem(new ShootingEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new MassEnemy(0.6f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        return l;
    }
    private static Level PathLeft(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.05f), biom, str);
        l.AddItem(new ShootingEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new ShootingEnemy(0.7f), l.GetEmptyPosition());
        return l;
    }
    private static Level PathRight(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.2f), biom, str);
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.7f), l.GetEmptyPosition());
        return l;
    }
    private static Level PreBossLevel(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.5f), biom, str);
        l.AddItem(new Base(l.GetEmptyPosition()));
        l.AddItem(new TutorialTrigger(new TutorialBlock("Dobře se připrav", "Intuice ti říká, že v další místnosti se utkáš s Bossem", "Obsaď si základnu (klávesa F), vyrob si vše potřebné a hodně štěstí!"), true));


        return l;
    }
    private static Level BossLevel(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0), biom, str);
        l.AddItem(new TimeEnemy(0.4f), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(0.4f), l.GetEmptyPosition());
        l.AddItem(new Boss1(0.5f), new Vector2(l.Maze.GetLength(1) / 2, l.Maze.GetLength(0) - 5));

        return l;
    }
}

