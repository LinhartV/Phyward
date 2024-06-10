using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class AllCrafts
{
    public static List<Craftable> craftables = new List<Craftable>(); 
    public static void SetupAllCrafts()
    {
        craftables.AddRange(CraftedWeapons.craftedWeapons);
    }
}
