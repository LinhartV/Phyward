using System;

/// <summary>
/// Something that player can craft.
/// </summary>
[Serializable]
public abstract class Craftable : Slotable
{
	public Craftable(){}
    /// <param name="neededMaterials">What and how many</param>
    protected Craftable(string spriteName, params (Craftable,int)[] neededMaterials) : base(spriteName)
    {
    }
}
