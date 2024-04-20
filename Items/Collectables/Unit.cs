using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : Collectable
{
    public Unit()
    {
    }

    public Unit(Slotable slotableRef, GameObject prefab, bool isSolid = false) : base(slotableRef, prefab, isSolid)
    {
    }

    public Unit((float, float) pos, GameObject prefab, PreUnit preUnit, Tilemap map = null) : base(preUnit, pos, prefab, map)
    {
    }
}

