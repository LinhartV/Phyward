using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Shot : Movable
{
    [JsonProperty]
    protected Character Character { get; set; }
    public Shot() { }
    public Shot(GameObject obj, Tilemap map, (float, float) pos, float baseSpeed, float duration, int characterId) : base(obj, pos, baseSpeed, map)
    {
        Character = GlobalControl.game.CurLevel.GameObjects[characterId] as Character;
        Duration = duration;
        DeleteOnLeave = true;
    }
    public float Duration { get; set; }
}

