using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class for managing exits from a particular level.
/// </summary>
[Serializable]
public class Base : Item
{
    public Base() { }

    public Base((float, float) pos, Tilemap map = null) : base(pos, GameObjects.exit, false, map)
    {

    }


}

