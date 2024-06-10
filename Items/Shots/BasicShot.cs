using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicShot : Shot
{
    public BasicShot() { }

    public BasicShot(Vector2 pos, float damage, int characterId, float duration, float maxspeed, float initialSpeed, float angle, float acceleration, float friction, float speedUpWithMovement, GameObject prefab, Tilemap map = null) : base(pos, damage, characterId, duration, maxspeed, initialSpeed, angle, acceleration, friction, speedUpWithMovement, prefab, map)
    {
    }
}

