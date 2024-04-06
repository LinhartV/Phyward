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
    public int ReloadTime { get; set; }
    public float Damage { get; set; }
    public float ShotDuration { get; set; }
    public float ShotSpeed { get; set; }
    public Character Character { get; set; }
    [JsonIgnore]
    protected GameObject shotPrefab;
    [JsonProperty]
    private string prefabName;
    [JsonProperty]
    protected bool reloaded;

    public IWeapon() { }
    protected IWeapon(int reloadTime, float damage, float shotSpeed, float shotDuration, GameObject shotPrefab)
    {
        ReloadTime = reloadTime;
        Damage = damage;
        ShotSpeed = shotSpeed;
        ShotDuration = shotDuration;
        this.prefabName = shotPrefab.name;
        this.shotPrefab = shotPrefab;
        //this.collider = collider;
    }

    protected void Fire(Shot shot)
    {
        if (reloaded)
        {
            GCon.game.CurLevel.AddItem(shot);
            reloaded = false;
            Task.Run(async () =>
            {
                await Task.Delay(ReloadTime);
                reloaded = true;
            });
        }
    }
    public abstract void Fire();
}

