using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CrumbledShot : Shot
{
    private Func<Vector2, int, float, Shot> crumbledShot;
    private int crumbledCount;

    public CrumbledShot() { }

    public CrumbledShot(Func<Vector2, int, float, Shot> crumbledShot, int crumbledCount, Vector2 pos, float damage, int characterId, float duration, float maxspeed, float initialSpeed, float angle, float acceleration, float friction, float speedUpWithMovement, GameObject prefab, Tilemap map = null) : base(pos, damage, characterId, duration, maxspeed, initialSpeed, angle, acceleration, friction, speedUpWithMovement, prefab, map)
    {
        this.crumbledCount = crumbledCount;
        this.crumbledShot = crumbledShot;
        if (crumbledShot == null)
        {
            this.crumbledShot = (pos, id, angle) => { return new CrumbledShot(null, crumbledCount, pos, damage, id, duration, maxspeed, initialSpeed, angle, acceleration, friction, speedUpWithMovement, prefab); };
        }
    }

    public override void Dispose()
    {
        base.Dispose();
    }
    public override void OnLand()
    {
        base.OnLand();
        for (int i = 0; i < crumbledCount; i++)
        {
            GCon.game.CurLevel.AddItem(crumbledShot(this.Prefab.transform.position, this.Character.Id, this.Angle + (i - (crumbledCount - 1) / 2) * (float)Math.PI / 4), this.Prefab.transform.position);
        }
    }
}

