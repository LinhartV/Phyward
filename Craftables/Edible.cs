using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Everything that can be used once
/// </summary>
public class Edible : Craftable
{
    public float CoolDown { get; private set; }
    public Action OnTrigger { get; private set; }
    public Edible(string name, string subheading, string description, int capacity, float coolDown, Action onTrigger, int tier, GameObject prefab, params (Slotable, int)[] neededMaterials) : base(name, subheading, description, tier, ToolsUI.FilterType.bonuses, prefab, true, neededMaterials)
    {
        Stackable = true;
        OnTrigger = onTrigger;
        this.Count = capacity;
        this.CoolDown = coolDown;
    }

    public override ICollectableRef DeepClone()
    {
        return new Edible(this.Name, this.Subheading, this.Description, Count, CoolDown, OnTrigger, this.Tier, GameObjects.GetPrefabByName(PrefabName), this.NeededMaterials);
    }
}

