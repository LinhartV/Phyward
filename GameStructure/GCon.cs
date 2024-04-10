using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class for controlling entire Phyward (GameControl)
/// </summary>
public static class GCon
{
    public static Dictionary<string, GameControl> games = new Dictionary<string, GameControl>();
    /// <summary>
    /// Game of the selected account
    /// </summary>
    public static GameControl game;
    public static bool gameStarted = false;
    public static float percentageOfFrame = Time.fixedDeltaTime;
}
