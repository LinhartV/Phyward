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

    public BasicShot((float, float) pos, int characterId, float duration, float maxspeed, float initialSpeed, float acceleration, float friction, float speedUpWithMovement, GameObject prefab, Tilemap map = null) : base(pos, characterId, duration, maxspeed, initialSpeed, acceleration, friction, speedUpWithMovement, prefab, map)
    {
    }
}

