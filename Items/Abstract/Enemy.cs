using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Enemy : Character
{
    public float BodyDamage { get; protected set; }
    [JsonProperty]
    private IIdleMovementAI idleMovement;
    public float Coef { get; set; }
    public Enemy() { }

    public Enemy(float coef,float bodyDamage, IIdleMovementAI idleMovement, Vector2 pos, float baseSpeed, float acceleration, float friction, float lives, GameObject prefab, bool isSolid = true
        , Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, null, 1, 1, 1, 1, lives, prefab, false, isSolid, map)
    {
        Constructor(bodyDamage, idleMovement, coef);
    }

    public Enemy(float coef, float bodyDamage, IIdleMovementAI idleMovement, float baseSpeed, float acceleration, float friction, float lives, GameObject prefab, bool isSolid = true) : base(baseSpeed, acceleration, friction, null, 1, 1, 1, 1, lives, prefab, false, isSolid)
    {
        Constructor(bodyDamage, idleMovement, coef);
    }
    public void Constructor(float bodyDamage, IIdleMovementAI idleMovement, float coef)
    {
        this.Coef = coef;
        this.BodyDamage = bodyDamage;
        this.idleMovement = idleMovement;
        this.StartIdleMovement();
    }

    protected override void SetupItem()
    {
        base.SetupItem();
        var colObj = new GameObject();
        colObj.transform.parent = this.Prefab.transform;
        var col = this.Prefab.GetComponent<Collider2D>();
        var coll = colObj.AddComponent(col.GetType());
        if (coll is CircleCollider2D cc)
        {
            cc.radius = (col as CircleCollider2D).radius;
        }
        if (coll is BoxCollider2D bc)
        {
            bc.size = (col as BoxCollider2D).size;
            bc.offset = (col as BoxCollider2D).offset;
        }
        if (coll is CapsuleCollider2D cac)
        {
            cac.size = (col as CapsuleCollider2D).size;
            cac.offset = (col as CapsuleCollider2D).offset;
            cac.direction = (col as CapsuleCollider2D).direction;
        }
        (coll as Collider2D).isTrigger = true;
        colObj.name = "PlayerTrigger";
        colObj.layer = LayerMask.NameToLayer("PlayerLayer");
    }

    public abstract void Drop();

    public override void InnerDispose()
    {
        base.InnerDispose();
    }
    public void StartIdleMovement()
    {
        this.idleMovement.StartIdleMovement(this);
    }
    public void StopIdleMovement()
    {
        this.idleMovement.StopIdleMovement(this);
    }
    public override void Death()
    {
        Drop();
        base.Death();
    }

    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);
        if (collider is Shot s && s.Character.IsFriendly)
        {
            this.ChangeLives(-s.DealDamage());
        }
    }

}

