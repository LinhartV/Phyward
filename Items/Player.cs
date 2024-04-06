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
    public Player() { }

    public Player((float, float) pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float lives, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, weapon, charDamage, charReloadTime, charShotSpeed, lives, GameObjects.player, map)
    {
        this.AddControlledMovement(new AcceleratedMovement(0, 0, this.Acceleration, BaseSpeed), "up");
        this.AddControlledMovement(new AcceleratedMovement(0, Math.PI / 2, this.Acceleration, BaseSpeed), "right");
        this.AddControlledMovement(new AcceleratedMovement(0, Math.PI, this.Acceleration, BaseSpeed), "down");
        this.AddControlledMovement(new AcceleratedMovement(0, 3 * Math.PI / 2, this.Acceleration, BaseSpeed), "left");
        this.AddAction(new ItemAction("faceCursor", 1));
    }

    protected override void SetupItem()
    {
        base.SetupItem();

        //Prefab.GetComponent<ExitScript>().X = exitX;
    }
    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);
        if (collider != null)
        {
            if (collider is Exit e)
            {
                var lvl = GCon.game.CurBiom.levels[e.LevelId];
                GameObject.Find("UnityControl").GetComponent<UnityControl>().BuildLevel(lvl, GCon.game.CurLevel);
                GCon.game.CurLevel = lvl;
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

