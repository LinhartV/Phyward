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

    protected Shot((float, float) pos, int characterId, float duration, float baseSpeed, double acceleration, double friction, GameObject prefab, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, prefab, map)
    {
        Character = GCon.game.CurLevel.Items[characterId] as Character;
        Duration = duration;
        DeleteOnLeave = true;
    }

    public float Duration { get; set; }
}

