using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BasicWeapon : IWeapon
{
    public BasicWeapon()
    {
    }

    public BasicWeapon(int reloadTime, float damage, float bulletSpeed) : base(reloadTime, damage, bulletSpeed)
    {
    }

    public override void Fire()
    {
        base.Fire(new BasicShot(BulletSpeed * Character.CharShotSpeed, 2, (Character.Prefab.transform.position.y, Character.Prefab.transform.position.x), Character.Id));
    }
}

