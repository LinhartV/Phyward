using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SwarmShot : Shot
{
    public SwarmShot() { }

    public SwarmShot(Vector2 pos, float swirlCoef, float damage, int characterId, float duration, float maxspeed, float initialSpeed, float angle, float acceleration, float friction, float speedUpWithMovement, GameObject prefab, Tilemap map = null) : base(pos, damage, characterId, duration, maxspeed, initialSpeed, angle, acceleration, friction, speedUpWithMovement, prefab, map)
    {
        AddControlledMovement(new AcceleratedMovement(2, angle, 1, true), "swirl");
        AddAction(new ItemAction("swirl", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.Freeze, null, ToolsGame.Rng() > 0.5 ? 1 : -1,swirlCoef));
    }
}

