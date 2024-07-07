using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Block : Item
{
    public Block()
    {
        this.IsSolid = true;
    }

    public Block(GameObject prefab) : base(prefab == null ? GameObjects.block : prefab, true)
    {
    }

    public Block(Vector2 pos, GameObject prefab = null, Tilemap map = null) : base(pos, prefab == null ? GameObjects.justBlock : prefab, true, map)
    {
    }
}

