using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CraftedWeapons
{
    public static CraftedWeapon SlingShot()
    {
        return new CraftedWeapon("Vydlicový prak", "Nic horšího bys neměl?", "Jedna z nejhorších zbraní, se kterou je čistý zázrak, že někoho vůbec škrábneš.", 0, GameObjects.slingshot, new BasicWeapon(ToolsMath.SecondsToFrames(1), 10, 5, ToolsMath.SecondsToFrames(1), 3, GameObjects.redSmallShot), (Units.time, 4));
    }
    public static CraftedWeapon Blowgun()
    {
        return new CraftedWeapon("Foukačka", "Hlavně nevdechnout, prosím!", "Primitivní, neúčinné, ale lepší než nic.", 0, GameObjects.blowgun, new BasicWeapon(ToolsMath.SecondsToFrames(0.8f), 7, 8, ToolsMath.SecondsToFrames(0.4f), 5, GameObjects.blueShot), (Units.time, 6));
    }
    public static CraftedWeapon Sling()
    {
        return new CraftedWeapon("Vrhací prak", "Roztočit a vrhnout!", "Jak to přimět letět rovně? No idea...", 0, GameObjects.sling, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.time, 7));
    }
    public static CraftedWeapon CrumblingRock()
    {
        return new CraftedWeapon("Rozpadající se kámen", "Kámen, který se každou chvíli rozpadne na menší", "Dobrá zbraň, pokud neumíš mířit", 1, GameObjects.crumblingRock, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.time, 5), (Units.mass, 7));
    }
    public static List<CraftedWeapon> craftedWeapons = new List<CraftedWeapon>() { SlingShot(), Blowgun(), Sling(), CrumblingRock() };
}

