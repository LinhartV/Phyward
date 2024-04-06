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

    public BasicShot((float, float) pos, int characterId, float duration, float baseSpeed, double acceleration, double friction, GameObject prefab, Tilemap map = null) : base(pos, characterId, duration, baseSpeed, acceleration, friction, prefab, map)
    {
    }
}

