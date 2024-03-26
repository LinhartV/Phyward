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
    public BasicShot( float baseSpeed, float duration, (float, float) pos, int characterId, Tilemap map = null ) : base(GameObjects.shot, map, pos, baseSpeed, duration, characterId)
    {
    }

}

