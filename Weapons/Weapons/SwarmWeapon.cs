using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class SwarmWeapon : IWeapon
{
    private float swirlCoef;
    public SwarmWeapon()
    {
    }

    public SwarmWeapon(float swirlCoef,float reloadTime, float damage, float shotSpeed, float shotDuration, float dispersion, GameObject shotPrefab) : base(reloadTime, damage, shotSpeed, shotDuration, dispersion, shotPrefab)
    {
        this.swirlCoef = swirlCoef;
    }

    public override void Fire()
    {
        base.Fire(new SwarmShot(Character.Prefab.transform.position, swirlCoef, this.Damage, Character.Id, ShotDuration * Character.CharShotDuration, ShotSpeed * Character.CharShotSpeed, ShotSpeed * Character.CharShotSpeed, Character.Angle + ToolsGame.Rng() * Dispersion * 2 - Dispersion, 0, 0, 0.3f, this.shotPrefab));

    }
}

