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
    private GameObject scrollPanel;
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
            UpdateScrolls();
            if (GCon.game.TutorialPhase == 6)
            {
                GCon.game.TutorialPhase = 7;
                GCon.game.CurLevel.DestroyAllItemsOfType<InvisibleBlock>();
                Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Kombinování veličin", "Zde pomocí fyzikálních vzorců získáváš vzácnější veličiny", "V horní liště vidíš své suroviny, vpravo objevené svitky, uprostřed výrobu surovin.", null, () =>
                {
                    Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Rychlost lze vyjádřit jako dráha za čas.\nPřetáhni dráhu (s) do čitatele a čas (t) do jmenovatele a klikni Vyrobit.", null, null, 25));
                }, 25));
            }
            if (GCon.game.TutorialPhase > 8 && GCon.game.showedFractionExplanation == false)
            {
                GCon.game.showedFractionExplanation = true;
                Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Zjednodušení výroby.","Zrychlení se spočítá jako rychlost za čas. A rychlost se spočítá jako dráha za čas.","Zrychlení tedy lze vyjádřit jako dráha lomeno čas krát čas - a takto to lze se vším!", null, null, 25));
            }
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
        scrollPanel = GameObject.FindGameObjectWithTag("ScrollList");
        fraction = GameObject.FindGameObjectWithTag("Fraction");
        result = new SlotTemplate(GameObject.FindGameObjectWithTag("Result"), false, false, false);
        divider = GameObject.FindGameObjectWithTag("Fraction").transform.GetChild(1).gameObject;
        craft = new ButtonTemplate(GameObject.FindGameObjectWithTag("Submit"), true, false);
        craft.AddTransition(new ColorChangable(0.2f, new Color(255, -10, -10, 0), null, true, () => { craft.ReturnTransition("clickWrong"); }), "clickWrong");
        craft.AddTransition(new ColorChangable(0.3f, new Color(-10, 255, -10, 0), null, true, () => { craft.ReturnTransition("clickRight"); }), "clickRight");
        craft.OnMouseDown = (UIItem item) =>
        {

            var unit = Units.ComposeUnit(numeratorUnits, denominatorUnits);

            if (unit != null && GCon.game.Player.PlayerControl.discoveredUnits.Any(x => x.Name == unit.Name))
            {
                craft.StartTransition("clickRight", true);
                GCon.AddPausedType(ToolsSystem.PauseType.Animation);
                GCon.game.Player.PlayerControl.AddSlotableToBase(unit);
                Action<SlotTemplate> animate = (SlotTemplate slot) =>
                {
                    ToolsUI.AnimateSettingSlotable(0.8f, ToolsUI.wrapPanel.Go.transform.InverseTransformPoint(result.Go.transform.position), slot.SlotableRef.Prefab, () => { }, true);
                    slot.RemoveSlotable();
                };
                for (int i = 0; i < numeratorUnits.Count; i++)
                {
                    GCon.game.Player.PlayerControl.SpendMaterial(numeratorUnits[i], 1);
                    animate(numeratorSlots[i]);
                }
                for (int i = 0; i < denominatorUnits.Count; i++)
                {
                    GCon.game.Player.PlayerControl.SpendMaterial(denominatorUnits[i], 1);
                    animate(denominatorSlots[i]);
                };
                if (GCon.game.TutorialPhase == 7)
                {
                    GCon.game.TutorialPhase = 8;
                    Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Dobrá práce! Fyzikální svět je neomezený! Nalézej svitky, kombinuj a experimentuj. Silnější veličiny znamenají silnější zbraně.", null, null, 25));
                }
                GCon.game.gameActionHandler.AddAction(new ItemAction((item, parameter) =>
                {
                    result.AddSlotable(unit);
                    ToolsUI.AnimateSettingSlotable(0.7f, unitSlots.First(x => x.SlotableRef.Name == unit.Name).Go, result.SlotableRef.Prefab, () =>
                    {
                        UpdateInventory();
                        GCon.PopPausedType();
                    }, true);
                    result.RemoveSlotable();
                }, ToolsMath.SecondsToFrames(0.8f), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning, null, unit));
                //UpdateInventory();
                
            }
            else
            {
                craft.StartTransition("clickWrong", true);
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

    private void UpdateScrolls()
    {
        foreach (Transform child in scrollPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        var offsetCount = 0;
        for (int i = 0; i < GCon.game.Player.PlayerControl.discoveredUnits.Count; i++)
        {
            var unit = GCon.game.Player.PlayerControl.discoveredUnits[i];
            if (unit.originalUnitNumeratorList.Count != 0 || unit.originalUnitDenominatorList.Count != 0)
            {
                var scroll = GameObject.Instantiate(GameObjects.craftingScroll);
                scroll.transform.SetParent(scrollPanel.transform);
                scroll.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = unit.Prefab.GetComponentInChildren<SpriteRenderer>().sprite;
                var rect = scroll.GetComponent<RectTransform>().rect;
                scroll.transform.localScale = new Vector3(1, 1, 1);
                scroll.transform.localPosition = new Vector3(-125, -50 - offsetCount * (rect.height + 50));
                UIItem scrollUI = new UIItem(scroll);
                scrollUI.AddTransition(new Scalable(0.2f, new Vector3(1.1f, 1.1f), ToolsUI.easeOut), "hover");
                scrollUI.OnMouseEnter = (UIItem item) => { item.StartTransition("hover", true); ToolsUI.SetCursor(ToolsUI.selectCursor); };
                scrollUI.OnMouseExit = (UIItem item) => { item.ReturnTransition("hover"); ToolsUI.SetCursor(ToolsUI.normalCursor); };
                scrollUI.OnMouseDown = (UIItem item) =>
                {
                    scrollUI.OnMouseExitDefault();
                    ToolsUI.TriggerScrollPanel(unit);
                    GCon.AddPausedType(ToolsSystem.PauseType.Animation);
                };
                offsetCount++;
            }
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
            if (i > 3)
            {
                break;
            }
            AddSlotToFraction(numeratorUnits, numeratorSlots, offset, slotWidth, offset, i, 0, divitorOffset);
        }
        for (int i = 0; i < denominatorUnits.Count + 1; i++)
        {
            if (i > 3)
            {
                break;
            }
            AddSlotToFraction(denominatorUnits, denominatorSlots, -3 * offset, slotWidth, offset, i, 2, divitorOffset);
        }
        if (numeratorSlots.Count == 1)
        {
            numeratorSlots[0].ChangePlaceHolder("1");
        }
        if (denominatorSlots.Count == 1)
        {
            denominatorSlots[0].ChangePlaceHolder("1");
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

