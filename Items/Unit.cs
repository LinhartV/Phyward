using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : Item
{
    public PreUnit PreUnit { get; private set; }
    public Unit()
    {
    }

    public Unit((float, float) pos, GameObject prefab, PreUnit preUnit, Tilemap map = null) : base(pos, prefab, false, map)
    {
        this.PreUnit = preUnit;
    }
    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);
        if (collider is Player p)
        {
            GCon.game.PlayerControl.unitCount[PreUnit]++;
        }
    }
}

