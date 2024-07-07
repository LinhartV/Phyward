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
    public Character Character { get; set; }
    [JsonProperty]
    protected float damage;
    public Shot() { }
    /// <param name="speedUpWithMovement">Percentage of how much shot Speed is affected by player movement (in reality it's 100%)</param>
    protected Shot(Vector2 pos, float damage, int characterId, float duration, float maxspeed, float initialSpeed, float angle, float acceleration, float friction, float speedUpWithMovement, GameObject prefab, Tilemap map = null) : base(pos, maxspeed, acceleration, friction, prefab, false, map)
    {
        Constructor(characterId, duration, maxspeed, initialSpeed, acceleration, speedUpWithMovement, damage, angle);
    }

    /// <param name="speedUpWithMovement">Percentage of how much shot Speed is affected by player movement (in reality it's 100%)</param>
    protected Shot(float damage, int characterId, float duration, float maxspeed, float initialSpeed, float angle, float acceleration, float friction, float speedUpWithMovement, GameObject prefab) : base(maxspeed, acceleration, friction, prefab, false)
    {
        Constructor(characterId, duration, maxspeed, initialSpeed, acceleration, speedUpWithMovement, damage, angle);
    }
    private void Constructor(int characterId, float duration, float maxspeed, float initialSpeed, float acceleration, float speedUpWithMovement, float damage, float angle)
    {
        if (GCon.game.Items.ContainsKey(characterId))
        {
            Character = GCon.game.Items[characterId] as Character;
        }
        DeleteOnLeave = true;
        this.damage = damage;
        this.AddAction(new ItemAction("disposeShot", duration, ItemAction.ExecutionType.OnlyFirstTime));
        this.AddAutomatedMovement(new AcceleratedMovement(initialSpeed, angle, acceleration, maxspeed));
        if (speedUpWithMovement > 0)
        {
            this.AddAutomatedMovement(new ConstantMovement(Character.GetMovementSpeed() * speedUpWithMovement, Character.GetMovementAngle(), true));
        }
    }
    /// <summary>
    /// When is shot destroyed due to timeout
    /// </summary>
    public virtual void OnLand()
    {
        this.Dispose();
    }
    public override void OnCollisionEnter(Item collider)
    {
        if (!GCon.GameStarted)
            return;
        base.OnCollisionEnter(collider);
        bool dispose = false;
        if ((collider.IsSolid && !(collider is Character)) || collider is Exit) { dispose = true; }
        if (collider is Character c && collider.Id != this.Character.Id)
        {
            if (c.IsFriendly != this.Character.IsFriendly)
            {
                dispose = true;
            }
        }
        if (dispose)
        {
            this.Dispose();
        }
    }

    public float DealDamage()
    {
        return this.damage * Character.CharDamage * (Character.Armor == null ? 1 : Character.Armor.DamageAmplifier);
    }
}

