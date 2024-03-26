﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToolsGame;

/// <summary>
/// Class for controlling game of particular player account
/// </summary>
[Serializable]
public class GameControl
{
    public GameControl() {}
    public string PlayerName { get; private set; }
    //public Dictionary<int, Level> Levels { get;private set; } = new Dictionary<int, Level>();
    /// <summary>
    /// Increasing id to be assigned for levels during creation
    /// </summary>
    public int Id { get; set; } = 0;
    public Dictionary<BiomType, Biom> bioms = new Dictionary<BiomType, Biom>();//{ { BiomType.meadow, new Biom("none", BiomType.meadow, 5, 2, new LinearGenerator()) } };
    
    public Biom CurBiom { get; set; }
    
    public Level CurLevel { get; set; }
    public Player Player { get; set; }
    public GameControl(string playerName)
    {
        PlayerName = playerName;
    }

}
