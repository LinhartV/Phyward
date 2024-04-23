using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public abstract class Shot : Movable
{
    [JsonProperty]
    public Character Character { get; private set; }
    [JsonProperty]
    protected float damage;
    public Shot() { }
    /// <param name="speedUpWithMovement">Percentage of how much shot speed is affected by player movement (in reality it's 100%)</param>
    protected Shot((float, float) pos, float damage, int characterId, float duration, float maxspeed, float initialSpeed, float acceleration, float friction, float speedUpWithMovement, GameObject prefab, Tilemap map = null) : base(pos, maxspeed, acceleration, friction, prefab, false, map)
    {
        Constructor(characterId, duration, maxspeed, initialSpeed, acceleration, speedUpWithMovement, damage);
    }

    /// <param name="speedUpWithMovement">Percentage of how much shot speed is affected by player movement (in reality it's 100%)</param>
    protected Shot(float damage, int characterId, float duration, float maxspeed, float initialSpeed, float acceleration, float friction, float speedUpWithMovement, GameObject prefab) : base(maxspeed, acceleration, friction, prefab, false)
    {
        Constructor(characterId, duration, maxspeed, initialSpeed, acceleration, speedUpWithMovement, damage);
    }
    private void Constructor(int characterId, float duration, float maxspeed, float initialSpeed, float acceleration, float speedUpWithMovement, float damage)
    {
        Character = GCon.game.Items[characterId] as Character;
        DeleteOnLeave = true;
        this.damage = damage;
        this.AddAction(new ItemAction("dispose", duration, ItemAction.ExecutionType.OnlyFirstTime));
        this.AddAutomatedMovement(new AcceleratedMovement(initialSpeed, Character.Angle, acceleration, maxspeed));
        if (speedUpWithMovement > 0)
        {
            this.AddAutomatedMovement(new ConstantMovement(Character.GetMovementSpeed() * speedUpWithMovement, Character.GetMovementAngle(), true));
        }
    }

    public override void OnCollisionEnter(Item collider)
    {
        if (!GCon.gameStarted)
            return;
        base.OnCollisionEnter(collider);
        bool dispose = false;
        if (collider.IsSolid) { dispose = true; }
        if (collider is Character c)
        {
            if (c.IsFriendly != this.Character.IsFriendly)
            {
                dispose = true;
            }
        }
        if (dispose && collider.Id != this.Character.Id)
        {
            this.Dispose();
        }
    }

    public float DealDamage()
    {
        return this.damage * Character.CharDamage;
    }
}

