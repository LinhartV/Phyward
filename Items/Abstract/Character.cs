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
    public float CharDispersion { get; set; }
    public float Lives { get; private set; }
    public float MaxLives { get; set; }
    public float CharReloadTime { get; set; }
    [JsonProperty]
    private IWeapon weapon;
    [JsonIgnore]
    protected LineRenderer fillBar;
    [JsonIgnore]
    protected LineRenderer damagebar;
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
    public Character(Vector2 pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isFriendly, bool isSolid, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, prefab, isSolid, map)
    {
        Constructor(isFriendly, charDamage, charReloadTime, lives, charShotSpeed, charShotDuration, weapon);
    }

    protected Character(float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, GameObject prefab, bool isFriendly, bool isSolid) : base(baseSpeed, acceleration, friction, prefab, isSolid)
    {
        Constructor(isFriendly, charDamage, charReloadTime, lives, charShotSpeed, charShotDuration, weapon);
    }

    private void Constructor(bool isFriendly, float charDamage, float charReloadTime, float lives, float charShotSpeed, float charShotDuration, IWeapon weapon)
    {
        this.IsFriendly = isFriendly;
        this.CharDamage = charDamage;
        this.CharReloadTime = charReloadTime;
        this.Weapon = weapon;
        Lives = lives;
        MaxLives = lives;
        CharShotSpeed = charShotSpeed;
        CharShotDuration = charShotDuration;
        UpdateHealthBar();
    }

    protected override void SetupItem()
    {
        if (Weapon != null)
        {
            this.Weapon.SetupWeapon();
        }
        AddHealthBar();
        UpdateHealthBar();
        base.SetupItem();
    }
    public override void OnCollisionEnter(Item collider)
    {
        base.OnCollisionEnter(collider);
        // if (collider is Shot s && s.) { }
    }

    public void ChangeLives(float change)
    {
        this.Lives += change;
        if (Lives > MaxLives)
        {
            Lives = MaxLives;
        }
        if (Lives <= 0)
        {
            Lives = 0;
            Death();
        }
        UpdateHealthBar();
    }

    public virtual void UpdateHealthBar()
    {
        if (MaxLives != 0)
        {
            damagebar.SetPositions(new Vector3[2] { new Vector3(fillBar.GetPosition(0).x + (fillBar.GetPosition(1).x- fillBar.GetPosition(0).x) * Lives / MaxLives, fillBar.GetPosition(1).y, 0), new Vector3(fillBar.GetPosition(1).x, fillBar.GetPosition(1).y, 0) });
        }
    }
    /// <summary>
    /// What should happen when lives are at zero
    /// </summary>
    public virtual void Death()
    {
        this.Dispose();
    }
    public virtual void AddHealthBar()
    {
        var bar = UnityEngine.Object.Instantiate(GameObjects.healthBarStandard);
        bar.transform.parent = this.Prefab.transform;
        bar.transform.position = new Vector3(0, 0, Prefab.transform.position.z);
        bar.transform.localPosition = new Vector3(0, 0, 0);
        fillBar = bar.transform.GetChild(1).GetComponent<LineRenderer>();
        fillBar.transform.position = new Vector3(0, 0, Prefab.transform.position.z + 0.5f);
        fillBar.transform.localPosition = new Vector3(0, 0, 0.5f);

        damagebar = bar.transform.GetChild(0).GetComponent<LineRenderer>();
        damagebar.transform.position = new Vector3(0, 0, Prefab.transform.position.z + 0.5f);
        damagebar.transform.localPosition = new Vector3(0, 0, 0.6f);
    }
}

