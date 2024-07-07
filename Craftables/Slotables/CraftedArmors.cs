using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CraftedArmors
{
    public static CraftedArmor TimeArmor()
    {
        return new CraftedArmor("Časové brnění", "Toto brnění se rozpadne za 10, 9...", 0, GameObjects.leatherArmor, new Armor(0.02f, 1.1f, null, ""), (Units.time, 4), (Units.length, 2), (Units.mass, 1));
    }
    public static CraftedArmor DenseArmor()
    {
        return new CraftedArmor("Hustý brnění", "Vyrobeno z materiálu o vysoké hustotě. Spolehlivě redukuje zranění.", 2, GameObjects.basicArmor, new Armor(0.1f, 1.2f, (Shot s) =>
        {
            s.AddAutomatedMovement(new AcceleratedMovement(s.BaseSpeed / 4, s.Angle, 0.5f));
            return s;
        }, "Střely letí rychleji."), (Units.density, 1), (Units.speed, 2));
    }

    public static List<CraftedArmor> craftedArmors = new List<CraftedArmor>() { TimeArmor(), DenseArmor() };
}

