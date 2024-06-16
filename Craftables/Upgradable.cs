using System;
using UnityEngine;

/// <summary>
/// Something that player can upgrade by adding units to it (so far probably just weapons)
/// </summary>
[Serializable]
public abstract class Upgradable : Craftable
{
	public Upgradable(){}

    protected Upgradable(string name, string subheading, string description, int tier, ToolsUI.FilterType filter, GameObject prefab, params (Slotable, int)[] neededMaterials) : base(name, subheading, description, tier, filter, prefab, true, neededMaterials)
    {
    }
}
