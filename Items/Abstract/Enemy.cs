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

    public Enemy() { }

    public Enemy(float bodyDamage, IIdleMovementAI idleMovement, (float, float) pos, float baseSpeed, float acceleration, float friction, float lives, GameObject prefab, bool isSolid = true
        , Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, null, 1, 1, 1, 1, lives, prefab, false, isSolid, map)
    {
        Constructor(bodyDamage, idleMovement);
    }

    public Enemy(float bodyDamage, IIdleMovementAI idleMovement, float baseSpeed, float acceleration, float friction, float lives, GameObject prefab, bool isSolid = true) : base(baseSpeed, acceleration, friction, null, 1, 1, 1, 1, lives, prefab, false, isSolid)
    {
        Constructor(bodyDamage, idleMovement);
    }
    public void Constructor(float bodyDamage, IIdleMovementAI idleMovement)
    {
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

    public override void Dispose()
    {
        base.Dispose();
        Drop();
    }
    public void StartIdleMovement()
    {
        this.idleMovement.StartIdleMovement(this);
    }
    public void StopIdleMovement()
    {
        this.idleMovement.StopIdleMovement(this);
    }


}

