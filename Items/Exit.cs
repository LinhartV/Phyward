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
public class Exit : Item
{
    public Exit(){ }

    public Exit((float, float) pos, int exitX, int exitY, int levelId, int exitId, Tilemap map = null) : base(pos, GameObjects.exit, map)
    {
        this.ExitX = exitX;
        this.ExitY = exitY;
        this.LevelId = levelId;
        this.ExitId = exitId;
    }

    [JsonProperty]
    public int ExitX { get; private set; } 
    [JsonProperty]
    public int ExitY { get; private set; }
    /// <summary>
    /// where this exit leads to
    /// </summary>
    public int LevelId { get; set; }
    /// <summary>
    /// id of this exit
    /// </summary>
    public int ExitId { get; set; }

    protected override void SetupItem()
    {
        base.SetupItem();
    }
}

