using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Character : Movable, ILived
{
    //Coeficients modifying weapon
    public bool IsFriendly { get; private set; }
    public float CharDamage { get; set; }
    public float CharShotSpeed { get; set; }
    public float CharShotDuration { get; set; }
    public float CharDispersion { get; set; }
    [JsonProperty]
    private float charReloadTime;
    public float CharReloadTime
    {
        get => charReloadTime; set
        {
            charReloadTime = value;
            if (Weapon!=null)
            {
                this.ChangeRepeatTime(charReloadTime * Weapon.ReloadTime, "fire");
            }
        }
    }
    /// <summary>
    /// because sometimes my characters dies multiple times...
    /// </summary>
    private bool alreadyDied = false;
    public StandardHealthBar HealthBar { get; set; }
    [JsonProperty]
    private Armor armor;
    public Armor Armor
    {
        get { return armor; }
        set
        {
            armor = value;
        }
    }
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

    public LivedHandler LivedHandler { get; protected set; }

    public Character() { }
    public Character(Vector2 pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isFriendly, bool isSolid, Armor armor, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, prefab, isSolid, map)
    {
        Constructor(isFriendly, charDamage, charReloadTime, lives, charShotSpeed, charShotDuration, weapon, armor);
    }

    protected Character(float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isFriendly, bool isSolid, Armor armor) : base(baseSpeed, acceleration, friction, prefab, isSolid)
    {
        Constructor(isFriendly, charDamage, charReloadTime, lives, charShotSpeed, charShotDuration, weapon, armor);
    }

    private void Constructor(bool isFriendly, float charDamage, float charReloadTime, float lives, float charShotSpeed, float charShotDuration, IWeapon weapon, Armor armor)
    {
        this.armor = armor;
        this.IsFriendly = isFriendly;
        this.CharDamage = charDamage;
        this.CharReloadTime = charReloadTime;
        this.Weapon = weapon;
        LivedHandler = new LivedHandler(this, lives);
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
    }





    public virtual void UpdateHealthBar()
    {
        HealthBar.UpdateHealthBar(LivedHandler);
    }
    /// <summary>
    /// What should happen when lives are at zero
    /// </summary>
    public virtual bool Death()
    {
        if (alreadyDied == false)
        {
            this.Dispose();
            alreadyDied = true;
            return false;
        }
        return true;
    }
    public virtual void AddHealthBar()
    {
        HealthBar.AddHealthBar(this.Prefab);
    }
}

