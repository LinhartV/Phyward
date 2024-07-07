using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class CrumblingWeapon : IWeapon
{
    private Func<Vector2, int, float, Shot> crumbledShot;
    private int crumbledCount;
    public CrumblingWeapon()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="crumbledShot">Set null to crumble this shot again infinitely</param>
    public CrumblingWeapon(Func<Vector2, int, float, Shot> crumbledShot, int crumbledCount, float reloadTime, float damage, float shotSpeed, float shotDuration, float dispersion, GameObject shotPrefab) : base(reloadTime, damage, shotSpeed, shotDuration, dispersion, shotPrefab)
    {
        this.crumbledShot = crumbledShot;
        this.crumbledCount = crumbledCount;
    }

    public override void Fire()
    {
        base.Fire(new CrumbledShot(crumbledShot, crumbledCount, Character.Prefab.transform.position, this.Damage, Character.Id, ShotDuration * Character.CharShotDuration, ShotSpeed * Character.CharShotSpeed, ShotSpeed * Character.CharShotSpeed, Character.Angle + ToolsGame.Rng() * Dispersion * 2 - Dispersion, 0, 0.1f, 0.3f, this.shotPrefab));

    }
}

