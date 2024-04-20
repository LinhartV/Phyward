using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RandomSpawner : ISpawner
{
    public RandomSpawner(params (Func<Item>, int)[] items) : base(items)
    {
    }

    public override void Spawn(Level lvl)
    {
        foreach (var item in Items)
        {
            for (int i = 0; i < item.Item2; i++)
            {
                Item obj = item.Item1();
                lvl.AddItem(obj);
                obj.InsertAtPosition(lvl.GetEmptyPosition(false));
            }
        }
    }

}

