using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
[Serializable]
public class InvisibleBlock : Block
{
    public InvisibleBlock()
    {
        this.IsSolid = true;
    }

    public InvisibleBlock(Vector2 pos) : base(pos, GameObjects.empty)
    {
    }
}

