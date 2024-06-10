using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// Class for controling player stats (not player as ingame character, but player as current game session)
/// </summary>
[Serializable]
public class PlayerControl
{
    public PlayerControl() { }
    public PlayerControl(bool dontBeCalledByJSON)
    {
        foreach (var unit in Units.allUnits)
        {
            unitCount.Add(unit.Name, 0);
        }
    }
    [JsonProperty]
    public CraftedWeapon WeaponSlotRef;
    /// <summary>
    /// Whether to pickup everything automatically or player has to press F
    /// </summary>
    public bool AutoPickup { get; set; } = true;
    /// <summary>
    /// What I carry right now
    /// </summary>
    public List<Slotable> backpack = new List<Slotable>();
    /// <summary>
    /// Space of backpack
    /// </summary>
    public int SlotSpace { get; set; } = 28;
    /// <summary>
    /// All I have right now on my base
    /// </summary>
    public List<Slotable> inBase = new List<Slotable>();
    /// <summary>
    /// What I can craft
    /// </summary>
    public List<Craftable> craftables = new List<Craftable>();
    public List<PreUnit> discoveredUnits = new List<PreUnit>();
    /// <summary>
    /// Count of each unit (accessed by name)
    /// </summary>
    public Dictionary<string, int> unitCount = new();

    public void DiscoverNewUnit(PreUnit unit)
    {
        discoveredUnits.Add(unit);
        craftables.Clear();
        foreach (var craftable in AllCrafts.craftables)
        {
            bool dontAdd = false;
            foreach (var material in craftable.NeededMaterials)
            {
                if (material.Item1 is PreUnit pu && !discoveredUnits.Contains(pu))
                {
                    dontAdd = true;
                    break;
                }
            }
            if (!dontAdd)
            {
                craftables.Add(craftable);
            }
        }
        if (ToolsUI.baseInventory != null && ToolsUI.ActiveInventory == ToolsUI.baseInventory)
        {
            ToolsUI.baseInventory.UpdateCraftingMenu();
        }
    }
    /// <summary>
    /// Adds new slotable to base
    /// </summary>
    public void AddSlotableToBase(Slotable material)
    {
        bool breaking = false;
        if (material.Stackable)
        {
            for (int i = 0; i < inBase.Count; i++)
            {
                if (inBase[i].Name == material.Name)
                {
                    inBase[i].Count += material.Count;
                    if (material is PreUnit)
                    {
                        unitCount[material.Name] += material.Count;
                    }

                    breaking = true;
                    break;
                }
            }
        }
        if (!breaking || !(material.Stackable))
        {
            inBase.Add(material);
            if (material is PreUnit)
            {
                unitCount[material.Name] += material.Count;
            }
        }

    }

    public bool Craft(Craftable craftable)
    {
        bool canCraft = true;
        foreach (var material in craftable.NeededMaterials)
        {
            if (material.Item2 > unitCount[material.Item1.Name])
            {
                return false;
            }
        }
        foreach (var material in craftable.NeededMaterials)
        {
            SpendMaterial(material.Item1, material.Item2);
        }
        AddSlotableToBase(craftable);
        return canCraft;
    }


    public void SpendMaterial(Slotable material, int count)
    {
        if (unitCount[material.Name] < count)
        {
            return;
        }
        List<Slotable> temp = new List<Slotable>(inBase);
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i].Name == material.Name)
            {
                if (temp[i].Count > count)
                {
                    temp[i].Count -= count;
                    unitCount[material.Name] -= count;

                    return;
                }
                else
                {
                    count -= temp[i].Count;
                    unitCount[material.Name] -= temp[i].Count;

                    inBase.RemoveAt(i);
                }
            }
        }
        return;
    }
    public void RemoveFromBase(Slotable slotable)
    {
        for (int i = 0; i < inBase.Count; i++)
        {
            if (inBase[i] == slotable)
            {
                if (slotable is PreUnit)
                {
                    unitCount[slotable.Name] -= slotable.Count;
                }
                inBase.RemoveAt(i);
                break;
            }
        }

    }
    public void RemoveFromBackpack(Slotable slotable)
    {
        if (ToolsUI.wrapPanel.weaponSlot.SlotableRef == slotable)
        {
            ToolsUI.wrapPanel.weaponSlot.RemoveSlotable();
        }
        backpack.Remove(slotable);
    }
    public void PickupCollectable(Collectable col)
    {
        if (backpack.Count < SlotSpace)
        {
            ToolsUI.AnimateSettingSlotable(1f, new Vector2(0, -250), col.Prefab, () =>
            {
                backpack.Add(col.SlotableRef);
                ToolsUI.ActiveInventory.UpdateInventory();
            }, false);
            col.Dispose();
        }

    }
}
