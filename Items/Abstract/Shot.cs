using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Shot : Movable
{
    [JsonProperty]
    protected Character Character { get; set; }
    public Shot() { }

    protected Shot((float, float) pos, int characterId, float duration, float maxspeed, float initialSpeed, float acceleration, float friction, GameObject prefab, Tilemap map = null) : base(pos, maxspeed, acceleration, friction, prefab, false, map)
    {
        Character = GCon.game.Items[characterId] as Character;
        DeleteOnLeave = true;
        this.AddAction(new ItemAction("dispose", duration, ItemAction.ExecutionType.OnlyFirstTime));
        this.AddAutomatedMovement(new AcceleratedMovement(initialSpeed, Character.Angle, acceleration, maxspeed));
    }
    public override void OnCollisionEnter(Item collider)
    {
        if (!GCon.gameStarted)
            return; 
        base.OnCollisionEnter(collider);
        if (collider.IsSolid && collider.Id != this.Character.Id)
        {
            this.Dispose();
        }
    }
}

