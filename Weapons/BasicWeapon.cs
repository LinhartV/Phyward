using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class BasicWeapon : IWeapon
{
    public BasicWeapon()
    {
    }

    public BasicWeapon(float reloadTime, float damage, float shotSpeed, float shotDuration, GameObject shotPrefab) : base(reloadTime, damage, shotSpeed, shotDuration, shotPrefab)
    {
    }

    public override void Fire()
    {
        base.Fire(new BasicShot((Character.Prefab.transform.position.y, Character.Prefab.transform.position.x), this.Damage, Character.Id, ShotDuration * Character.CharShotDuration, ShotSpeed * Character.CharShotSpeed, ShotSpeed * Character.CharShotSpeed, 0, 0, 0.1f, this.shotPrefab));

    }
}

