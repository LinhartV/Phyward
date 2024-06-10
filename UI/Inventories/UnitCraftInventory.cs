using geniikw.DataRenderer2D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitCraftInventory : Inventory
{
    private SlotTemplate result;
    private ButtonTemplate craft;
    private GameObject divider;
    private GameObject fraction;
    private List<SlotTemplate> numeratorSlots = new List<SlotTemplate>();
    private List<SlotTemplate> denominatorSlots = new List<SlotTemplate>();
    private List<PreUnit> numeratorUnits = new List<PreUnit>();
    private List<PreUnit> denominatorUnits = new List<PreUnit>();
    private List<SlotTemplate> unitSlots = new List<SlotTemplate>();
    public UnitCraftInventory(UIItem panel) : base(panel)
    {
        panel.Go.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, -940);
        panel.Go.SetActive(false);
    }


    public override void LoadUI()
    {
    }

    public override bool OpenInventory()
    {
        if (base.OpenInventory())
        {
            panel.StartTransition("reveal");
            UpdateUnitsInventory();
            return true;
        }
        return false;
    }

    public override void UpdateInventory()
    {
        UpdateUnitsInventory();
        UpdateFraction();
    }

    protected override void SetupInventory()
    {
        //UpdateInventory();
        fraction = GameObject.FindGameObjectWithTag("Fraction");
        result = new SlotTemplate(GameObject.FindGameObjectWithTag("Result"), true, false, false);
        divider = GameObject.FindGameObjectWithTag("Fraction").transform.GetChild(1).gameObject;
        craft = new ButtonTemplate(GameObject.FindGameObjectWithTag("Submit"), true, false);
        craft.OnMouseDown = (UIItem item) =>
        {
            var unit = Units.ComposeUnit(numeratorUnits, denominatorUnits);
            if (GCon.game.Player.PlayerControl.discoveredUnits.Any(x => x.Name == unit.Name))
            {
                GCon.game.Player.PlayerControl.AddSlotableToBase(unit);
                foreach (var material in numeratorUnits)
                {
                    GCon.game.Player.PlayerControl.SpendMaterial(material, 1);
                }
                foreach (var material in denominatorUnits)
                {
                    GCon.game.Player.PlayerControl.SpendMaterial(material, 1);
                }
                UpdateInventory();
            }
        };

        UpdateFraction();
        UpdateUnitsInventory();
    }


    private void UpdateUnitsInventory()
    {
        numeratorUnits.Clear();
        denominatorUnits.Clear();
        foreach (var item in unitSlots)
        {
            item.Dispose();
        }
        unitSlots.Clear();
        const float slotWidth = Constants.SLOT_WIDTH;
        const float minOffset = 50;
        var slotsInventory = new UIItem(GameObject.FindGameObjectWithTag("CraftingUnitsSlots").transform.gameObject);
        float width = slotsInventory.Go.GetComponent<RectTransform>().rect.width;

        int widthCount = (int)Math.Floor(width / (slotWidth + minOffset));
        int slotCount = GCon.game.Player.PlayerControl.discoveredUnits.Count;
        float offset = ((int)width - widthCount * slotWidth) / (widthCount + 1);
        bool doubleBreak = false;
        int ii = 0;
        while (true)
        {
            for (int j = 0; j < widthCount; j++)
            {
                if (slotCount <= j + ii * widthCount)
                {
                    doubleBreak = true;
                    break;
                }
                var slotUI = new SlotTemplate(slotWidth, true, true, true, slotsInventory.Go, new Vector3(offset + j * (slotWidth + offset), -offset - ii * (slotWidth + offset)), (SlotTemplate slot) =>
                {
                    return slot.SlotableRef.Name == ToolsUI.draggedSlot.SlotableRef.Name && ToolsUI.draggedSlot != slot;
                }, (SlotTemplate slot) =>
                {
                    if (numeratorSlots.Contains(ToolsUI.draggedSlot))
                    {
                        numeratorUnits.RemoveAt(numeratorSlots.IndexOf(ToolsUI.draggedSlot));
                    }
                    else if (denominatorSlots.Contains(ToolsUI.draggedSlot))
                    {
                        denominatorUnits.RemoveAt(denominatorSlots.IndexOf(ToolsUI.draggedSlot));
                    }
                    UpdateFraction();
                    //UpdateInventory();
                });
                slotUI.DragAll = false;

                unitSlots.Add(slotUI);
            }
            if (doubleBreak)
            {
                break;
            }
            ii++;
        }
        for (int i = 0; i < GCon.game.Player.PlayerControl.discoveredUnits.Count; i++)
        {
            unitSlots[i].ShowStackTextForCountLessThenTwo = true;
            unitSlots[i].AddSlotable(GCon.game.Player.PlayerControl.discoveredUnits[i], true, GCon.game.Player.PlayerControl.unitCount[GCon.game.Player.PlayerControl.discoveredUnits[i].Name], false);
        }
    }


    private void UpdateFraction()
    {
        int slotWidth = 125;
        int offset = 50;
        int divitorOffset = (500 - slotWidth) / 2 - offset;
        foreach (var item in numeratorSlots)
        {
            item.Dispose();
        }
        numeratorSlots.Clear();
        foreach (var item in denominatorSlots)
        {
            item.Dispose();
        }
        denominatorSlots.Clear();
        //numerator
        for (int i = 0; i < numeratorUnits.Count + 1; i++)
        {
            AddSlotToFraction(numeratorUnits, numeratorSlots, offset, slotWidth, offset, i, 0, divitorOffset);
        }
        for (int i = 0; i < denominatorUnits.Count + 1; i++)
        {
            AddSlotToFraction(denominatorUnits, denominatorSlots, -3 * offset, slotWidth, offset, i, 1, divitorOffset);
        }
        divider.GetComponent<UILine>().line.EditPoint(1, new Point(new Vector3(divitorOffset - slotWidth / 2 + (slotWidth + offset) * Math.Max(numeratorSlots.Count, denominatorSlots.Count), 0, 0), Vector3.zero, Vector3.zero, 15)) /*divider.transform.position + new Vector3((slotWidth + offset * 2) / slotWidth * Math.Max(numeratorSlots.Count, denominatorSlots.Count)*/;
    }

    private void AddSlotToFraction(List<PreUnit> units, List<SlotTemplate> slots, int y, int slotWidth, int offset, int i, int indexOfParent, int dividorOffset)
    {
        var slot = new SlotTemplate(slotWidth, true, true, true, fraction.transform.GetChild(indexOfParent).gameObject, new Vector3(dividorOffset + slotWidth / 2 + offset + i * (slotWidth + offset), y), (SlotTemplate slot) =>
        {
            return slot.SlotableRef == null && !slots.Contains(ToolsUI.draggedSlot);
        }, (SlotTemplate slot) =>
        {
            units.Add(slot.SlotableRef as PreUnit);
            if (denominatorSlots.Contains(ToolsUI.draggedSlot) && numeratorSlots.Contains(slot))
            {
                denominatorUnits.RemoveAt(denominatorSlots.IndexOf(ToolsUI.draggedSlot));
            }
            if (numeratorSlots.Contains(ToolsUI.draggedSlot) && denominatorSlots.Contains(slot))
            {
                numeratorUnits.RemoveAt(numeratorSlots.IndexOf(ToolsUI.draggedSlot));
            }
            UpdateFraction();
        }, (SlotTemplate slot) =>
        {
            //numeratorUnits.Remove(slot.SlotableRef as PreUnit);
        });
        slot.Stackable = false;
        if (i != units.Count)
        {
            slot.AddSlotable(units[i], false);
        }
        slots.Add(slot);
    }
}

