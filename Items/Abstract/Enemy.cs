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
    public bool IsMinion { get; set; } = false;
    [JsonIgnore]
    protected List<ToolsPhyward.Drop> dropList = new List<ToolsPhyward.Drop>();
    public Enemy() { }

    public Enemy(float coef, float bodyDamage, IIdleMovementAI idleMovement, Vector2 pos, float baseSpeed, float acceleration, float friction, float lives, GameObject prefab, bool isSolid = true
        , Armor armor = null, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, null, 1, 1, 1, 1, lives * coef, prefab, false, isSolid, armor, map)
    {
        Constructor(bodyDamage, idleMovement, coef);
    }

    public Enemy(float coef, float bodyDamage, IIdleMovementAI idleMovement, float baseSpeed, float acceleration, float friction, float lives, GameObject prefab, Armor armor = null, bool isSolid = true) : base(baseSpeed, acceleration, friction, null, 1, 1, 1, 1, lives * coef, prefab, false, isSolid, armor)
    {
        Constructor(bodyDamage, idleMovement, coef);
    }
    public void Constructor(float bodyDamage, IIdleMovementAI idleMovement, float coef)
    {
        SetAngle = true;
        this.Coef = coef * GCon.game.Coef;
        this.BodyDamage = bodyDamage * Coef;
        this.idleMovement = idleMovement;
        if (idleMovement != null)
        {
            this.StartIdleMovement();
        }
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

    public void Drop()
    {
        if (dropList.Count != 0)
        {
            ToolsPhyward.DropDrops(dropList, this.Prefab.transform.position);
        }
    }

    public override void InnerDispose()
    {
        base.InnerDispose();
    }
    public void StartIdleMovement()
    {
        if (idleMovement != null)
        {
            this.idleMovement.StartIdleMovement(this);
        }
    }
    public void StopIdleMovement()
    {
        if (idleMovement != null)
        {
            this.idleMovement.StopIdleMovement(this);
        }
    }
    public override bool Death()
    {
        if (!base.Death())
        {
            Drop();
            return false;
        }
        return true;
    }

    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);

    }

}

