﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[Serializable]
public abstract class Biom
{
    public Biom() { }
    public string Music { get; protected set; }
    public Dictionary<int, Level> levels = new Dictionary<int, Level>();

    public Biom(string music, IBiomGenerator generator)
    {
    }
    //tilesets?
    //monsters to be spawned?
    //other objects to be spawned?
}
