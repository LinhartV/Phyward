using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Weapon : Upgradable
{
    public IWeapon CraftedWeapon { get;private set; }
    public Weapon()
    {
    }

    public Weapon(string spriteName, IWeapon weapon) : base(spriteName)
    {
        CraftedWeapon = weapon;
    }
}

