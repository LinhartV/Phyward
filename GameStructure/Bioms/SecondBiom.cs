using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static IBiomGenerator;
using static Tutorial;

public static class SecondBiom
{
    private static IBiomGenerator generator;
    private static Biom biom;
    private static float biomCoef = 0.8f;
    public static Biom GetSecondBiom()
    {
        Queue<LevelStructure> levelStructures = new Queue<LevelStructure>();
        biom = new Biom("", 4);
        int currentId = 0;
        generator = new CustomGenerator(5);
        int x = 0;
        int y = 0;
        IBiomGenerator.LevelStructure str = null;
        int firstCrossroad;
        int rightCrossroad;
        //Intro
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 1, 1);
        levelStructures.Enqueue(str);
        //Small crossroads
        firstCrossroad = currentId;
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 2, 2);
        levelStructures.Enqueue(str);
        //right1
        rightCrossroad = currentId;
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 3, 4);
        levelStructures.Enqueue(str);
        //right2
        generator.MoveInDirection(ref x, ref y, ref str, 1, currentId++, 3, 3);
        levelStructures.Enqueue(str);

        //right up1 preboss
        str = levelStructures.ElementAt(rightCrossroad);
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 1, 1);
        levelStructures.Enqueue(str);
        //Boss1
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 4, 4);
        levelStructures.Enqueue(str);
        //up1
        str = levelStructures.ElementAt(firstCrossroad);
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 2, 3);
        levelStructures.Enqueue(str);
        //up2
        int upperCrossroads = currentId;
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 2, 2);
        levelStructures.Enqueue(str);

        //up3
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 2, 2);
        levelStructures.Enqueue(str);
        //up2 left
        str = levelStructures.ElementAt(upperCrossroads);
        int bossCrossroads = currentId;
        generator.MoveInDirection(ref x, ref y, ref str, 3, currentId++, 3, 3);
        levelStructures.Enqueue(str);
        //boss2
        generator.MoveInDirection(ref x, ref y, ref str, 3, currentId++, 4, 4);
        levelStructures.Enqueue(str);
        //SwarmMess
        str = levelStructures.ElementAt(bossCrossroads);
        generator.MoveInDirection(ref x, ref y, ref str, 0, currentId++, 3, 3);
        levelStructures.Enqueue(str);


        biom.FirstBiomLevel = Level1(levelStructures.Dequeue());
        Level2(levelStructures.Dequeue());
        Right1(levelStructures.Dequeue());
        Right2(levelStructures.Dequeue());
        RightUpPreboss(levelStructures.Dequeue());
        Boss1(levelStructures.Dequeue());
        Up1(levelStructures.Dequeue());
        Up2(levelStructures.Dequeue());
        Up3(levelStructures.Dequeue());
        Up2Left(levelStructures.Dequeue());
        Boss2(levelStructures.Dequeue());
        SwarmMess(levelStructures.Dequeue());
        GCon.game.Player.InsertAtPosition(new Vector2(1, 1), true);

        return biom;
    }
    private static Level Level1(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0f), biom, str);

        l.AddItem(new Base(new Vector2(3, 3)));
        l.AddItem(new TutorialTrigger(new TutorialBlock("Kombinování veličin", "Ze svitku ses naučil vyrábět Rychlost.", "Jdi na základnu a stiskni nalevo symbol svitku."), false), new Vector2(3, 1));
        l.AddItem(new Block(new Vector2(1, 2), GameObjects.justBlock));
        l.AddItem(new Block(new Vector2(2, 2), GameObjects.justBlock));
        l.AddItem(new Block(new Vector2(3, 2), GameObjects.justBlock));
        l.AddItem(new InvisibleBlock(l.GetOutExitPosition()));

        l.AddItem(new Unit(new Vector2(3, 1), Units.Length()));
        l.AddItem(new Unit(new Vector2(4, 1), Units.Time()));
        return l;
    }
    private static Level Level2(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.6f), biom, str);
        l.AddItem(new TimeEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new MassEnemy(biomCoef), l.GetEmptyPosition());

        return l;
    }
    private static Level Right1(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.1f), biom, str);

        l.AddItem(new ShootingEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new ShootingEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new ShootingEnemy(biomCoef), l.GetEmptyPosition());
        return l;
    }
    private static Level Right2(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.05f), biom, str);

        l.AddItem(new SwarmEnemy(0.6f), l.GetEmptyPosition());
        l.AddItem(new Collectable(new Scroll(Units.acceleration, () => { GCon.game.TutorialPhase = 9; })), l.GetEmptyPositionAtFarEnd());


        return l;
    }
    private static Level RightUpPreboss(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0f), biom, str);

        l.AddItem(new Base(new Vector2(3, 3)));
        l.AddItem(new Block(new Vector2(3, 4)));
        l.AddItem(new Block(new Vector2(2, 3)));
        l.AddItem(new Block(new Vector2(4, 3)));
        return l;
    }
    private static Level Boss1(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0f), biom, str);
        /*l.AddItem(new TimeEnemy(biomCoef), new Vector2(1, l.Maze.GetLength(0) - 3));
        l.AddItem(new TimeEnemy(biomCoef), new Vector2(l.Maze.GetLength(1) - 3, l.Maze.GetLength(0) - 3));*/
        l.AddItem(new Boss2(biomCoef), new Vector2(l.Maze.GetLength(1) / 2, l.Maze.GetLength(0) - 5));


        return l;
    }
    private static Level Up1(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.1f), biom, str);
        l.AddItem(new SwarmEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new MassEnemy(biomCoef), l.GetEmptyPosition());

        return l;
    }
    private static Level Up2(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.5f), biom, str);
        l.AddItem(new ShootingEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new MassEnemy(biomCoef), l.GetEmptyPosition());

        return l;
    }
    private static Level Up3(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0f), biom, str);
        l.AddItem(new SwarmEnemy(0.6f), l.GetEmptyPosition());
        l.AddItem(new SwarmEnemy(0.6f), l.GetEmptyPosition());

        return l;
    }
    private static Level Up2Left(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.1f), biom, str);
        l.AddItem(new ShootingEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new TimeEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new MassEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new Base(l.GetEmptyPosition()));

        return l;
    }
    private static Level Boss2(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0f), biom, str);
        l.AddItem(new Boss1(biomCoef, new List<ToolsPhyward.Drop>() {
                new ToolsPhyward.Drop(1, 1, 1, () => { return new Collectable(new Scroll(Units.volume)); })
            }), new Vector2(l.Maze.GetLength(1) / 5, l.Maze.GetLength(0) - 5));
        l.AddItem(new Boss1(biomCoef, new List<ToolsPhyward.Drop>() {
                new ToolsPhyward.Drop(1, 1, 1, () => { return new Collectable(new Scroll(Units.density)); })
            }), new Vector2(l.Maze.GetLength(1) * 4 / 5, l.Maze.GetLength(0) - 5));

        return l;
    }
    private static Level SwarmMess(LevelStructure str)
    {
        Level l = generator.CreateLevel(new BlockInsert(0.06f), biom, str);

        l.AddItem(new SwarmEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new SwarmEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new SwarmEnemy(biomCoef), l.GetEmptyPosition());
        l.AddItem(new SwarmEnemy(biomCoef), l.GetEmptyPosition());

        return l;
    }
}

