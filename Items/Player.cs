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
public class Player : Character
{
    public Player() { }
    public Player((float, float) pos, float baseSpeed, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float lives, Tilemap map = null) : base(GameObjects.player, map, pos, baseSpeed, weapon, charDamage, charReloadTime, charShotSpeed, lives)
    {
        
    }

    protected override void SetupItem()
    {
        base.SetupItem();
        //Prefab.GetComponent<ExitScript>().X = exitX;
    }
}

