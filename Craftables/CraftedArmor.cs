using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CraftedArmor : Upgradable
{
    public Armor Armor { get; private set; }
    public CraftedArmor()
    {
    }

    public CraftedArmor(string name, string subheading, int tier, GameObject prefab, Armor armor, params (Slotable, int)[] neededMaterials) : base(name, subheading, "", tier, ToolsUI.FilterType.armor, prefab, neededMaterials)
    {
        this.Armor = armor;
        Description = armor.GetStats();
    }

    public override ICollectableRef DeepClone()
    {
        return new CraftedArmor(Name, Subheading, Tier, Prefab, Armor, NeededMaterials);
    }
}

