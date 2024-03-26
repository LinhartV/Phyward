using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class IWeapon
{
    public int ReloadTime { get; set; }
    public float Damage { get; set; }
    public float BulletSpeed { get; set; }
    [JsonProperty]
    public Character Character { get; set; }
    [JsonProperty]
    protected bool reloaded;
    public IWeapon() { }
    protected IWeapon(int reloadTime, float damage, float bulletSpeed)
    {
        ReloadTime = reloadTime;
        Damage = damage;
        BulletSpeed = bulletSpeed;
    }

    protected void Fire(Shot shot)
    {
        if (reloaded)
        {
            GlobalControl.game.CurLevel.AddItem(shot);
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

