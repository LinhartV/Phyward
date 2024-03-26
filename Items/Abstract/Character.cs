using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Character : Movable
{
    public float CharDamage { get; set; }
    public float CharShotSpeed { get; set; }
    public float Lives { get; set; }
    public float CharReloadTime { get; set; }
    [JsonProperty]
    private IWeapon weapon;
    public IWeapon Weapon
    {
        get { return weapon; }
        set
        {
            weapon = value;
            weapon.Character = this;
        }
    }
    public Character() { }
    public Character(GameObject obj, Tilemap map, (float, float) pos, float baseSpeed, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float lives) : base(obj, pos, baseSpeed, map)
    {
        this.CharDamage = charDamage;
        this.CharReloadTime = charReloadTime;
        this.Weapon = weapon;
        Lives = lives;
        CharShotSpeed = charShotSpeed;
    }
}

