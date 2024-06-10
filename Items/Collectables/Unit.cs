using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Unit I can really collect (laying on the ground)
/// </summary>
public class Unit : Collectable
{
    public Unit()
    {
    }

    public Unit(Slotable slotableRef, bool isSolid = false) : base(slotableRef, isSolid)
    {
    }

    public Unit(Vector2 pos, PreUnit preUnit, Tilemap map = null) : base(preUnit, pos, map)
    {
    }
}

