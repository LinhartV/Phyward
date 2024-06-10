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
public abstract class Collectable : Item, IInteractable
{
    public Slotable SlotableRef { get; private set; }
    public Collectable() { }

    protected Collectable(Slotable slotableRef, bool isSolid = false) : base(slotableRef.Prefab, isSolid)
    {
        DeleteOnLeave = false;
        this.SlotableRef = slotableRef;
    }

    protected Collectable(Slotable slotableRef, Vector2 pos, Tilemap map = null) : base(pos, slotableRef.Prefab, false, map)
    {
        DeleteOnLeave = false;
        this.SlotableRef = slotableRef;
    }

    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);
        if (!GCon.GameStarted)
            return;

    }

    protected override void SetupItem()
    {
        base.SetupItem();
        if (SlotableRef!=null)
        {
            SlotableRef.AssignPrefab();
        }
    }

    public void Interact()
    {
        GCon.game.Player.PlayerControl.PickupCollectable(this);
    }
}

