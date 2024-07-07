using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class CraftedWeapons
{
    public static CraftedWeapon Blowgun()
    {
        return new CraftedWeapon("Foukačka", "Hlavně nevdechnout, prosím!", "Primitivní, neúčinné, ale lepší než nic.", 0, GameObjects.blowgun, new BasicWeapon(ToolsMath.SecondsToFrames(0.8f), 5, 8, ToolsMath.SecondsToFrames(0.6f), 5, GameObjects.blowgunShot));
    }
    public static CraftedWeapon SlingShot()
    {
        return new CraftedWeapon("Vydlicový prak", "Nic horšího bys neměl?", "Bude ti trvat hodně dlouho, než někoho vůbec zraníš.", 0, GameObjects.slingshot, new BasicWeapon(ToolsMath.SecondsToFrames(1), 6, 7, ToolsMath.SecondsToFrames(1.2f), 3, GameObjects.pebble), (Units.time, 3));
    }
    public static CraftedWeapon Sling()
    {
        return new CraftedWeapon("Vrhací prak", "Roztočit a vrhnout!", "Jak to přimět letět rovně? No idea...", 0, GameObjects.sling, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(0.9f), 10, 8, ToolsMath.SecondsToFrames(1f), 30, GameObjects.pebble), (Units.mass, 3));
    }


    public static CraftedWeapon Rock()
    {
        return new CraftedWeapon("Šutr", "Nic snadnějšího než hodit obří těžký kámen", "Moc daleko nedoletí, co si budem...", 1, GameObjects.block, new NormalDispersionBasicWeapon(ToolsMath.SecondsToFrames(2f), 20, 4, ToolsMath.SecondsToFrames(0.7f), 20, GameObjects.block), (Units.mass, 5), (Units.length, 3));
    }
    public static CraftedWeapon CrumblingRock()
    {
        return new CraftedWeapon("Rozpadající se kámen", "Objemný balvan, který se každou chvíli rozpadne na menší", "Dobrá zbraň, pokud neumíš mířit", 2, GameObjects.crumblingRock, new CrumblingWeapon((pos, id, angle) => { return new BasicShot(pos, 10, id, ToolsMath.SecondsToFrames(0.7f), 10, 10, angle, 0, 0.2f, 0, GameObjects.pebble); }, 4, ToolsMath.SecondsToFrames(1.5f), 15, 4, ToolsMath.SecondsToFrames(0.6f), 0, GameObjects.crumblingRock), (Units.volume, 1), (Units.time, 3));
    }
    /*public static CraftedWeapon Spear()
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
    }*/


    public static List<CraftedWeapon> craftedWeapons = new List<CraftedWeapon>() { SlingShot(), Sling(), CrumblingRock(), /*Spear(),*/ Rock(),/*Bow(), Atlatl()*/ };
}

