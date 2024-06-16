using System;
using UnityEngine;

/// <summary>
/// Something that player can craft.
/// </summary>
[Serializable]
public abstract class Craftable : Slotable
{
    public (Slotable, int)[] NeededMaterials{get; private set;}
    public int Tier { get;private set;}
    public Craftable() { }

    /// <param name="neededMaterials">What and how many</param>
    /// <param name="tier">Rarity of this item</param>
    /// <param name="filter">Kind of this item</param>
    protected Craftable(string name, string subheading, string description, int tier, ToolsUI.FilterType filter, GameObject prefab, bool exchangable, params (Slotable, int)[] neededMaterials) : base(name, subheading, description, filter, prefab,false, exchangable)
    {
        NeededMaterials = neededMaterials;
        Tier = tier;
    }
}
