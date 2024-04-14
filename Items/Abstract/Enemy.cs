using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Enemy : Character
{
    public Enemy() { }

    protected Enemy((float, float) pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isSolid = true
        , Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, weapon, charDamage, charReloadTime, charShotSpeed, charShotDuration, lives, prefab, isSolid, map)
    {

    }

}

