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

    public BasicWeapon(float reloadTime, float damage, float shotSpeed, float shotDuration, float dispersion, GameObject shotPrefab) : base(reloadTime, damage, shotSpeed, shotDuration, dispersion, shotPrefab)
    {
    }

    public override void Fire()
    {
        base.Fire(new BasicShot(Character.Prefab.transform.position, this.Damage, Character.Id, ShotDuration * Character.CharShotDuration, ShotSpeed * Character.CharShotSpeed, ShotSpeed * Character.CharShotSpeed, Character.Angle + ToolsGame.NormalRng(0, Dispersion), 0, 0, 0.2f, this.shotPrefab));

    }
}

