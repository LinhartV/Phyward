using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CraftedWeapon : Upgradable
{
    public IWeapon Weapon { get; private set; }
    public CraftedWeapon()
    {
    }

    public CraftedWeapon(string name, string subheading, string description, int tier, GameObject prefab, IWeapon weapon, params (Slotable, int)[] neededMaterials) : base(name, subheading, description, tier, ToolsUI.FilterType.weapons, prefab, neededMaterials)
    {
        Weapon = weapon;
    }
    public override void AssignPrefab()
    {
        base.AssignPrefab();
        Weapon.SetupWeapon();
    }

    public override ICollectableRef DeepClone()
    {
        return new CraftedWeapon(this.Name, this.Subheading, this.Description, this.Tier, GameObjects.GetPrefabByName(PrefabName),this.Weapon, this.NeededMaterials);
    }
}

