using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class for managing exits from a particular level - not a GameObject, just for calculations
/// </summary>
[Serializable]
public class PreExit
{
    public PreExit(int position, int levelId)
    {
        Position = position;
        LevelId = levelId;
    }
    /// <summary>
    /// position of this exit
    /// </summary>
    public int Position { get; set; }
    /// <summary>
    /// where this exit leads to
    /// </summary>
    public int LevelId { get; set; }
    /// <summary>
    /// id of this exit
    /// </summary>
    public int ExitId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

