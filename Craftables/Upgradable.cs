using System;

/// <summary>
/// Something that player can upgrade by adding units to it (so far probably just weapons)
/// </summary>
[Serializable]
public abstract class Upgradable : Craftable
{
	public Upgradable(){}

    protected Upgradable(string spriteName) : base(spriteName)
    {
    }
}
