using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class IWeapon
{
    public float ReloadTime { get; set; }
    public float Damage { get; set; }
    public float ShotDuration { get; set; }
    public float ShotSpeed { get; set; }
    public float Dispersion { get; set; }
    public Character Character { get; set; }
    [JsonIgnore]
    protected GameObject shotPrefab;
    [JsonProperty]
    private string prefabName;
    [JsonProperty]
    public bool Reloaded { get; set; }
    [JsonProperty]
    public bool AutoFire { get; set; }

    public IWeapon() { }
    protected IWeapon(float reloadTime, float damage, float shotSpeed, float shotDuration, float dispersion, GameObject shotPrefab, bool autoFire = true)
    {
        Reloaded = true;
        ReloadTime = reloadTime;
        Damage = damage;
        ShotSpeed = shotSpeed;
        ShotDuration = shotDuration;
        this.prefabName = shotPrefab.name;
        this.shotPrefab = shotPrefab;
        AutoFire = autoFire;
        Dispersion = ToolsMath.DegreeToRad(dispersion);
    }

    public void SetupWeapon()
    {
        this.shotPrefab = GameObjects.GetPrefabByName(this.prefabName);
    }

    protected void Fire(Shot shot)
    {
        if (Character.Armor != null && Character.Armor.ShotModificator != null)
        {
            GCon.game.CurLevel.AddItem(Character.Armor.ShotModificator(shot));
        }
        else
            GCon.game.CurLevel.AddItem(shot);
    }
    public abstract void Fire();
}

