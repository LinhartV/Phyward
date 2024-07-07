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
public class Collectable : Item, IInteractable
{
    public ICollectableRef SlotableRef { get; private set; }
    public Collectable() { }

    public Collectable(ICollectableRef slotableRef, bool isSolid = false) : base(slotableRef.Prefab, isSolid)
    {
        DeleteOnLeave = false;
        this.SlotableRef = slotableRef;
    }

    public Collectable(ICollectableRef slotableRef, Vector2 pos, Tilemap map = null) : base(pos, slotableRef.Prefab, false, map)
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
        Collider2D col;
        if(!Prefab.TryGetComponent<Collider2D>(out col))
        {
            col = Prefab.AddComponent<CircleCollider2D>();
        }
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

