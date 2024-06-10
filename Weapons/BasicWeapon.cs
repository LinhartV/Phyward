using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class NormalDispersionBasicWeapon : IWeapon
{
    public NormalDispersionBasicWeapon()
    {
    }

    public NormalDispersionBasicWeapon(float reloadTime, float damage, float shotSpeed, float shotDuration, float dispersion, GameObject shotPrefab) : base(reloadTime, damage, shotSpeed, shotDuration, dispersion, shotPrefab)
    {
    }

    public override void Fire()
    {
        base.Fire(new BasicShot(Character.Prefab.transform.position, this.Damage, Character.Id, ShotDuration * Character.CharShotDuration, ShotSpeed * Character.CharShotSpeed, ShotSpeed * Character.CharShotSpeed, Character.Angle + ToolsGame.Rng() * Dispersion * 2 - Dispersion, 0, 0, 0.1f, this.shotPrefab));

    }
}

