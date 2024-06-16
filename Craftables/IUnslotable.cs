using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IUnslotable : ICollectableRef
{
    public IUnslotable() { }
    protected IUnslotable(GameObject prefab) : base(prefab)
    {
    }

    public abstract void ActionWhenCollected();
}
