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
    //Coeficients modifying weapon
    public float CharDamage { get; set; }
    public float CharShotSpeed { get; set; }
    public float CharShotDuration { get; set; }
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
    public Character((float, float) pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isSolid = true, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, prefab, isSolid, map)
    {
        this.CharDamage = charDamage;
        this.CharReloadTime = charReloadTime;
        this.Weapon = weapon;
        Lives = lives;
        CharShotSpeed = charShotSpeed;
        CharShotDuration = charShotDuration;
    }
    protected override void SetupItem()
    {
        if (Weapon != null)
        {
            this.Weapon.SetupWeapon();
        }
        base.SetupItem();
    }
}

