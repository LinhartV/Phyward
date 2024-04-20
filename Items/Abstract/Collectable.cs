using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Something player can pick up from ground
/// </summary>
public abstract class Collectable : Item
{
    private Slotable slotableRef;
    public Collectable() { }

    protected Collectable(Slotable slotableRef, GameObject prefab, bool isSolid = false) : base(prefab, isSolid)
    {
        DeleteOnLeave = false;
        this.slotableRef = slotableRef;
    }

    protected Collectable(Slotable slotableRef, (float, float) pos, GameObject prefab, Tilemap map = null) : base(pos, prefab, false, map)
    {
        DeleteOnLeave = false;
        this.slotableRef = slotableRef;
    }
    public override void OnCollisionEnter(Item collider)
    {
        if (!GCon.gameStarted)
            return;
        if (collider is Player p)
        {
            this.Dispose();
            p.PlayerControl.AddMaterial(slotableRef);
        }
    }
}

