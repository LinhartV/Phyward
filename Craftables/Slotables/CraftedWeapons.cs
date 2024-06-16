using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CraftedWeapons
{
    public static CraftedWeapon SlingShot()
    {
        return new CraftedWeapon("Vydlicový prak", "Nic horšího bys neměl?", "Bude ti trvat hodně dlouho, než někoho vůbec zraníš.", 0, GameObjects.slingshot, new BasicWeapon(ToolsMath.SecondsToFrames(1), 10, 5, ToolsMath.SecondsToFrames(1), 3, GameObjects.redSmallShot), (Units.time, 2));
    }
    public static CraftedWeapon Blowgun()
    {
        return new CraftedWeapon("Foukačka", "Hlavně nevdechnout, prosím!", "Primitivní, neúčinné, ale letí to víceméně po rovné dráze.", 0, GameObjects.blowgun, new BasicWeapon(ToolsMath.SecondsToFrames(0.8f), 7, 8, ToolsMath.SecondsToFrames(0.4f), 5, GameObjects.blueShot), (Units.length, 4));
    }
    public static CraftedWeapon Sling()
    {
        return new CraftedWeapon("Vrhací prak", "Roztočit a vrhnout!", "Jak to přimět letět rovně? No idea...", 0, GameObjects.sling, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.time, 3));
    }
    public static CraftedWeapon Rock()
    {
        return new CraftedWeapon("Šutr", "Nic snadnějšího než hodit obří těžký kámen", "Doporučil bych přidat trochu času, ať ti to někam doletí.", 0, GameObjects.crumblingRock, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.mass, 5));
    }
    public static CraftedWeapon CrumblingRock()
    {
        return new CraftedWeapon("Rozpadající se kámen", "Těžký kámen, který se každou chvíli rozpadne na menší", "Dobrá zbraň, pokud neumíš mířit", 1, GameObjects.crumblingRock, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.time, 5), (Units.mass, 3));
    }
    public static CraftedWeapon Spear()
    {
        return new CraftedWeapon("Oštěp", "Kus klacku s hrotem - žádný zázrak", "Krátký čas letu, zato letí přesně.", 1, GameObjects.crumblingRock, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.time, 5), (Units.mass, 7));
    }
    public static CraftedWeapon Bow()
    {
        return new CraftedWeapon("Luk", "Střílející rychlé šípy", "Nabíjení pomalé, ale rychlost střel oceníš.", 2, GameObjects.crumblingRock, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.speed, 3));
    }
    public static CraftedWeapon Atlatl()
    {
        return new CraftedWeapon("Atlatl", "Vrhač oštěpů", "Nabíjení pomalé, ale rychlost střel oceníš.", 2, GameObjects.crumblingRock, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 15, 7, ToolsMath.SecondsToFrames(1f), 40, GameObjects.redSmallShot), (Units.length, 3), (Units.frequency, 1));
    }
    
    
    public static List<CraftedWeapon> craftedWeapons = new List<CraftedWeapon>() { SlingShot(), Blowgun(), Sling(), CrumblingRock(), Spear(), Rock(), Bow(), Atlatl() };
}

