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
    public bool IsFriendly { get; private set; }
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
            if (weapon != null)
            {
                weapon.Character = this;
            }
        }
    }
    public Character() { }
    public Character((float, float) pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isFriendly, bool isSolid, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, prefab, isSolid, map)
    {
        this.IsFriendly = isFriendly;
        this.CharDamage = charDamage;
        this.CharReloadTime = charReloadTime;
        this.Weapon = weapon;
        Lives = lives;
        CharShotSpeed = charShotSpeed;
        CharShotDuration = charShotDuration;
    }

    protected Character(float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isFriendly, bool isSolid) : base(baseSpeed, acceleration, friction, prefab, isSolid)
    {
        this.IsFriendly = isFriendly;
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
    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);
        // if (collider is Shot s && s.) { }
    }
}

