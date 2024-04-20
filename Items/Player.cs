using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class for managing exits from a particular level.
/// </summary>
[Serializable]
public class Player : Character
{
    public PlayerControl PlayerControl { get; private set; } = new PlayerControl();
    public Player() { }

    public Player((float, float) pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, weapon, charDamage, charReloadTime, charShotSpeed, charShotDuration, lives, GameObjects.player, true, true, map)
    {/*
        this.AddControlledMovement(new AcceleratedMovement(0, 0, this.Acceleration, BaseSpeed), "up");
        this.AddControlledMovement(new AcceleratedMovement(0, (float)Math.PI / 2, this.Acceleration, BaseSpeed), "right");
        this.AddControlledMovement(new AcceleratedMovement(0, (float)Math.PI, this.Acceleration, BaseSpeed), "down");
        this.AddControlledMovement(new AcceleratedMovement(0, (float)(3 * Math.PI / 2), this.Acceleration, BaseSpeed), "left");
        this.AddAction(new ItemAction("faceCursor", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        SetAngle = false;*/
        Dictionary<string, IMovement> movements = new Dictionary<string, IMovement>();
        movements.Add("up", new AcceleratedMovement(0, 0, this.Acceleration, BaseSpeed));
        movements.Add("right", new AcceleratedMovement(0, (float)Math.PI / 2, this.Acceleration, BaseSpeed));
        movements.Add("down", new AcceleratedMovement(0, (float)Math.PI, this.Acceleration, BaseSpeed));
        movements.Add("left", new AcceleratedMovement(0, (float)Math.PI * 3 / 2, this.Acceleration, BaseSpeed));
        AddControlledMovement(new CompositeMovement(new AcceleratedMovement(0, 0, this.Acceleration, baseSpeed), movements), "movement");
        /*Dictionary<string, IMovement> movements = new Dictionary<string, IMovement>();
        movements.Add("up", new ConstantMovement(BaseSpeed, 0));
        movements.Add("right", new ConstantMovement(BaseSpeed, (float)Math.PI / 2));
        movements.Add("down", new ConstantMovement(BaseSpeed, (float)Math.PI));
        movements.Add("left", new ConstantMovement(BaseSpeed, (float)Math.PI * 3 / 2));
        AddControlledMovement(new CompositeMovement(new ConstantMovement(BaseSpeed, 0), movements), "movement");*/
        this.AddAction(new ItemAction("faceCursor", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        SetAngle = false;
    }

    protected override void SetupItem()
    {
        base.SetupItem();

        //Prefab.GetComponent<ExitScript>().X = exitX;
    }
    public override void OnCollisionEnter(Item collider)
    {
        if (!GCon.gameStarted)
            return;
        base.OnCollisionEnter(collider);
        if (collider != null)
        {
            if (collider is Exit e)
            {
                var lvl = GCon.game.CurBiom.levels[e.LevelId];
                GameObject.Find("UnityControl").GetComponent<UnityControl>().BuildLevel(lvl, GCon.game.CurLevel);
                GCon.game.CurLevel = lvl;
                lvl.OnEnter();
                foreach (var exits in GCon.game.CurBiom.levels[e.LevelId].ExitsAr)
                {
                    foreach (var exit in exits)
                    {
                        if (exit.ExitId == e.ExitId)
                        {
                            this.Prefab.transform.position = new Vector3(exit.X, exit.Y);
                            break;
                        }
                    }
                }
            }
        }
    }
}

