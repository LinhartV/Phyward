using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GameObjects
{
    private static Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
    public static GameObject empty;
    public static Tilemap solidLayer;


    public static void SetPrefabs(GameObject _empty, Tilemap _solidLayer)
    {
        tilemaps.Add(_solidLayer.name, _solidLayer);

        empty = _empty;
        solidLayer = _solidLayer;
    }
    public static Tilemap GetTilemapByName(string name)
    {
        return tilemaps[name];
    }

}

