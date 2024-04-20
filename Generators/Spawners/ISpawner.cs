using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class ISpawner
{
    protected ISpawner(params (Func<Item>, int)[] items)
    {
        Items = items;
    }

    protected (Func<Item>, int)[] Items { get; set; }
    public abstract void Spawn(Level lvl);
}

