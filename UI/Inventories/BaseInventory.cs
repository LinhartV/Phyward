using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class BaseInventory : Inventory
{
    /// <summary>
    /// Slots in baseSlots
    /// </summary>
    public List<SlotTemplate> baseSlots = new List<SlotTemplate>();
    /// <summary>
    /// Slots in craftableSlots
    /// </summary>
    public List<UIItem> craftableSlots = new List<UIItem>();
    /// <summary>
    /// Slots in backpackSlots
    /// </summary>
    public List<SlotTemplate> backpackSlots = new List<SlotTemplate>();
    private GameObject scrollbarCrafting;
    private SlotTemplate stackToBase;
    private UIItem craftingSpace;
    public UIItem baseInventory;
    private ToolsUI.FilterType craftingFilter;
    private ButtonTemplate[] craftingFilters = new ButtonTemplate[4];
    private UIItem unitCraftButton;
    public SlotTemplate binSlot;
    public ButtonTemplate[] filterButtons = new ButtonTemplate[4];

    public BaseInventory(UIItem panel) : base(panel)
    {
    }

    public override bool OpenInventory()
    {
        if (base.OpenInventory())
        {
            ToolsUI.wrapPanel.OpenInventory();
            craftingFilters[0].OnMouseDownDefault();
            UpdateInventory();
            return true;
        }
        return false;
    }



    private void SetupInventorySlots()
    {
        GCon.game.Player.PlayerControl.backpack.Sort();
        GCon.game.Player.PlayerControl.inBase.Sort();
        foreach (var slot in baseSlots)
        {
            slot.Dispose();
        }
        foreach (var slot in backpackSlots)
        {
            slot.Dispose();
        }
        //Setup backpack
        ToolsUI.DraggedSlot = null;
        backpackSlots.Clear();
        const float slotWidth = Constants.SLOT_WIDTH * 2 / 3;
        const float minOffset = 20;
        var slotsInventory = new UIItem(GameObject.FindGameObjectWithTag("BackpackSlots").transform.gameObject);
        float width = slotsInventory.Go.GetComponent<RectTransform>().rect.width;
        float height = slotsInventory.Go.GetComponent<RectTransform>().rect.height;
        int widthCount = (int)Math.Floor(width / (slotWidth + minOffset));
        int heightCount = (int)Math.Floor(height / (slotWidth + minOffset));
        float offset = ((int)width - widthCount * slotWidth) / (widthCount + 1);
        bool doubleBreak = false;
        int negativeCount = 0;
        for (int i = 0; i < GCon.game.Player.PlayerControl.backpack.Count; i++)
        {
            if (!(ToolsUI.filter == GCon.game.Player.PlayerControl.backpack[i].filter || ToolsUI.filter == ToolsUI.FilterType.all || (GCon.game.Player.PlayerControl.backpack[i].filter == ToolsUI.FilterType.armor && ToolsUI.filter == ToolsUI.FilterType.weapons)))
            {
                negativeCount++;
            }
        }
        int slotCount = GCon.game.Player.PlayerControl.SlotSpace - negativeCount;
        for (int i = 0; i < heightCount; i++)
        {
            for (int j = 0; j < widthCount; j++)
            {
                if (slotCount <= j + i * widthCount)
                {
                    doubleBreak = true;
                    break;
                }
                var slotUI = new SlotTemplate(slotWidth, true, true, true, slotsInventory.Go, new Vector3(offset + j * (slotWidth + offset) + slotWidth / 2, -offset - i * (slotWidth + offset) - slotWidth / 2), (SlotTemplate slot) =>
                {
                    return !backpackSlots.Contains(ToolsUI.draggedSlot) && !(ToolsUI.draggedSlot.SlotableRef is PreUnit);
                }, (SlotTemplate slot) =>
                {
                    GCon.game.Player.PlayerControl.backpack.Add(ToolsUI.draggedSlot.SlotableRef);
                    GCon.game.Player.PlayerControl.RemoveFromBase(ToolsUI.draggedSlot.SlotableRef);
                    UpdateInventory();
                }, null, true);
                backpackSlots.Add(slotUI);
            }
            if (doubleBreak)
            {
                break;
            }
        }
        int counter = 0;
        for (int i = 0; i < GCon.game.Player.PlayerControl.backpack.Count; i++)
        {
            var item = GCon.game.Player.PlayerControl.backpack[i];
            if (ToolsUI.filter == item.filter || ToolsUI.filter == ToolsUI.FilterType.all || (GCon.game.Player.PlayerControl.backpack[i].filter == ToolsUI.FilterType.armor && ToolsUI.filter == ToolsUI.FilterType.weapons))
            {
                backpackSlots[i - counter].AddSlotable(GCon.game.Player.PlayerControl.backpack[i]);
                backpackSlots[i - counter].dropableCondition = null;
            }
            else
            {
                counter++;
            }
        }
        //Setup base slots
        baseSlots.Clear();
        slotsInventory = new UIItem(GameObject.FindGameObjectWithTag("BaseSlots").transform.gameObject);
        width = slotsInventory.Go.GetComponent<RectTransform>().rect.width;
        widthCount = (int)Math.Floor(width / (slotWidth + minOffset));
        offset = ((int)width - widthCount * slotWidth) / (widthCount + 1);
        doubleBreak = false;
        negativeCount = 0;
        foreach (var item in GCon.game.Player.PlayerControl.inBase)
        {
            if (!(ToolsUI.filter == item.filter || ToolsUI.filter == ToolsUI.FilterType.all || (item.filter == ToolsUI.FilterType.armor && ToolsUI.filter == ToolsUI.FilterType.weapons)))
            {
                negativeCount++;
            }
        }
        slotCount = GCon.game.Player.PlayerControl.inBase.Count - negativeCount + 1;
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
                var slotUI = new SlotTemplate(slotWidth, true, true, true, slotsInventory.Go, new Vector3(offset + j * (slotWidth + offset) + slotWidth / 2, -offset - ii * (slotWidth + offset) - slotWidth / 2), (SlotTemplate slot) =>
                {
                    return !baseSlots.Contains(ToolsUI.draggedSlot);
                }, (SlotTemplate slot) =>
                {
                    GCon.game.Player.PlayerControl.RemoveFromBackpack(ToolsUI.draggedSlot.SlotableRef);
                    GCon.game.Player.PlayerControl.AddSlotableToBase(ToolsUI.draggedSlot.SlotableRef);
                    UpdateInventory();
                }, null, true);
                baseSlots.Add(slotUI);
            }
            if (doubleBreak)
            {
                break;
            }
            ii++;
        }
        counter = 0;
        foreach (var item in GCon.game.Player.PlayerControl.inBase)
        {
            if (ToolsUI.filter == item.filter || ToolsUI.filter == ToolsUI.FilterType.all || (item.filter == ToolsUI.FilterType.armor && ToolsUI.filter == ToolsUI.FilterType.weapons))
            {
                baseSlots[counter].AddSlotable(item, true);
                baseSlots[counter].dropableCondition = null;
                counter++;
            }
        }
    }
    public override void UpdateInventory()
    {
        PutUnitsToBase();
        SetupInventorySlots();
        UpdateCraftingMenu();
    }
    private void PutUnitsToBase()
    {
        List<Slotable> temp = new List<Slotable>(GCon.game.Player.PlayerControl.backpack);
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i].filter == ToolsUI.FilterType.units)
            {
                GCon.game.Player.PlayerControl.AddSlotableToBase(temp[i]);
                GCon.game.Player.PlayerControl.backpack.Remove(temp[i]);
            }
        }
    }
    protected override void SetupInventory()
    {

        baseInventory = new UIItem(GameObject.FindGameObjectWithTag("BaseInventory"));
        baseInventory.Go.SetActive(true);
        baseInventory.Go.transform.position = new Vector3(0, baseInventory.Go.transform.position.y);
        scrollbarCrafting = GameObject.Find("ScrollbarCrafting");

        SetupInventorySlots();
        SetupPresetSlots();
        SetupCraftingButtons();
        SetupFilterButtons();
    }

    private void UpdateScrollBarCrafting()
    {
        Scrollbar bar = scrollbarCrafting.GetComponent<Scrollbar>();
        bar.size = 0.2f;//craftingSpace.Go.GetComponent<RectTransform>().rect.height / ;
    }
    private void SetupPresetSlots()
    {
        binSlot = new SlotTemplate(GameObject.FindGameObjectWithTag("BinBase"), true, false, true, (SlotTemplate slotable) => { return true; }, (SlotTemplate slot) =>
        {
            if (ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.RemoveFromBase(slot.SlotableRef);
            }
            if (ToolsUI.baseInventory.backpackSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.RemoveFromBackpack(slot.SlotableRef);
            }
            //slot.RemoveSlotable();
            UpdateInventory();
        }, null, true);
        stackToBase = new SlotTemplate(GameObject.FindGameObjectWithTag("StackToBase"), true, false, false, null, null, null);
        stackToBase.OnMouseDown = (UIItem slot) =>
        {
            List<Slotable> temp = new List<Slotable>(GCon.game.Player.PlayerControl.backpack);
            for (int i = 0; i < temp.Count; i++)
            {
                GCon.game.Player.PlayerControl.AddSlotableToBase(temp[i]);
                GCon.game.Player.PlayerControl.RemoveFromBackpack(temp[i]);
            }
            SetupInventorySlots();
        };
        craftingSpace = new UIItem(GameObject.FindGameObjectWithTag("CraftingSpace"));
        unitCraftButton = new ButtonTemplate(GameObject.FindGameObjectWithTag("UnitCraftButton"), true, false);
        unitCraftButton.OnMouseDown = (UIItem item) =>
        {
            ToolsUI.unitCraftInventory.OpenInventory();
        };
    }
    private void SetupCraftingButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            craftingFilters[i] = new ButtonTemplate(GameObject.FindGameObjectWithTag("CraftingFilters").transform.GetChild(i).gameObject, true, true);

            if (i == 0)
            {
                craftingFilters[i].OnMouseDownDefault();
            }
            craftingFilters[i].OnMouseDown = (UIItem item) =>
            {
                ButtonTemplate b = item as ButtonTemplate;
                if (b.Go.name == "All")
                    craftingFilter = ToolsUI.FilterType.all;
                if (b.Go.name == "Armor")
                    craftingFilter = ToolsUI.FilterType.armor;
                if (b.Go.name == "Weapons")
                    craftingFilter = ToolsUI.FilterType.weapons;
                if (b.Go.name == "Bonuses")
                    craftingFilter = ToolsUI.FilterType.bonuses;
                UpdateCraftingMenu();
                foreach (var button in craftingFilters)
                {
                    if (button != b)
                    {
                        button.Deselect();
                    }
                }

            };
            craftingFilters[i].OnMouseUp = (UIItem item) =>
            {

            };
        }
    }
    public void UpdateCraftingMenu()
    {
        var temp = new List<UIItem>(craftableSlots);
        foreach (var item in temp)
        {
            item.Dispose();
            GameObject.Destroy(item.Go);
        }
        craftableSlots.Clear();
        GCon.game.Player.PlayerControl.craftables.Sort((x, y) =>
        {
            return x.Tier.CompareTo(y.Tier);
        });
        float offset = 30;
        for (int i = 0; i < GCon.game.Player.PlayerControl.craftables.Count; i++)
        {
            var craftableItem = GCon.game.Player.PlayerControl.craftables[i];

            if (craftableItem.filter != craftingFilter && craftingFilter != ToolsUI.FilterType.all)
            {
                continue;
            }
            var craftable = GameObject.Instantiate(GameObjects.craftable);
            craftable.transform.SetParent(craftingSpace.Go.transform);
            var rect = craftable.GetComponent<RectTransform>().rect;
            craftable.transform.localPosition = new Vector3(0, -i * (rect.height + 50) - rect.height - offset);
            craftable.transform.localScale = new Vector3(1, 1, 1);
            var craftableSlot = new SlotTemplate(craftable.transform.GetChild(2).gameObject, true, false, false, null, null, null);
            craftableSlot.AddSlotable(craftableItem, false);
            for (int j = 0; j < craftableItem.NeededMaterials.Length; j++)
            {
                var unit = GameObject.Instantiate(GameObjects.GetPrefabByName(craftableItem.NeededMaterials[j].Item1.Prefab.name));
                var unitRect = unit.AddComponent<RectTransform>();
                var image = unit.transform.GetChild(0).gameObject.AddComponent<Image>();
                image.preserveAspect = true;
                image.sprite = unit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                Component.Destroy(unit.transform.GetChild(0).GetComponent<SpriteRenderer>());
                unitRect.anchorMax = new Vector2(0, 0.5f);
                unitRect.anchorMin = new Vector2(0, 0.5f);
                unitRect.pivot = new Vector2(0, 0.5f);
                unit.transform.SetParent(craftable.transform.GetChild(1).GetChild(0));
                unit.transform.localPosition = new Vector3(j * 95, 0);
                unit.transform.localScale = new Vector3(1, 1, 1);
                unit.transform.GetChild(0).localScale = new Vector3(90, 90, 90);
                var textPrefab = GameObject.Instantiate(GameObjects.text);
                var text = textPrefab.GetComponent<TMPro.TextMeshProUGUI>();
                text.text = GCon.game.Player.PlayerControl.unitCount[craftableItem.NeededMaterials[j].Item1.Name].ToString() + "/" + craftableItem.NeededMaterials[j].Item2.ToString();
                textPrefab.transform.SetParent(unit.transform);
                textPrefab.transform.localScale = Vector3.one;
                textPrefab.transform.localPosition = new Vector3(unitRect.rect.width / 2, -35, 0);
                text.fontSize = 40;
                if (GCon.game.Player.PlayerControl.unitCount[craftableItem.NeededMaterials[j].Item1.Name] >= craftableItem.NeededMaterials[j].Item2)
                {
                    text.color = Color.green;
                }
                else
                {
                    text.color = Color.red;
                }
            }
            var uicraftable = new UIItem(craftable);
            uicraftable.AddTransition(new Scalable(0.15f, new Vector2(0.05f, 0.05f), ToolsUI.easeOut, true), "hover");
            uicraftable.OnMouseEnter = (UIItem item) =>
            {
                item.StartTransition("hover");
                craftableSlot.OnMouseEnterDefault();
            };
            uicraftable.OnMouseExit = (UIItem item) =>
            {
                item.ReturnTransition("hover");
                craftableSlot.OnMouseExitDefault();
            };
            uicraftable.OnMouseDown = (UIItem item) =>
            {
                if (GCon.game.Player.PlayerControl.Craft(craftableItem))
                {
                    int negativeCount = 0;
                    foreach (var slot in GCon.game.Player.PlayerControl.inBase)
                    {
                        if (!(ToolsUI.filter == slot.filter || ToolsUI.filter == ToolsUI.FilterType.all || (slot.filter == ToolsUI.FilterType.armor && ToolsUI.filter == ToolsUI.FilterType.weapons)))
                        {
                            negativeCount++;
                        }
                    }
                    GCon.AddPausedType(ToolsSystem.PauseType.Animation);
                    ToolsUI.AnimateSettingSlotable(1f, baseSlots[GCon.game.Player.PlayerControl.inBase.Count - negativeCount - 1].Go, craftableSlot.SlotableRef.Prefab, () =>
                    {
                        UpdateInventory();
                        GCon.PopPausedType();
                    }, true);
                }
            };
            craftableSlots.Add(new UIItem(craftable));
        }
        UpdateScrollBarCrafting();
    }

    public override void LoadUI()
    {

    }
    private void SetupFilterButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            filterButtons[i] = new ButtonTemplate(GameObject.FindGameObjectWithTag("FiltersBase").transform.GetChild(i).gameObject, true, true);
            var box = filterButtons[i].Go.AddComponent<BoxCollider2D>();
            var rect = filterButtons[i].Go.GetComponent<RectTransform>().rect;
            box.offset = new Vector2(rect.width / 2, -rect.height / 2);
            box.size = new Vector2(rect.width, rect.height);
            box.isTrigger = true;
            if (i == 0)
            {
                filterButtons[i].OnMouseDownDefault();
                ToolsUI.filter = ToolsUI.FilterType.all;
            }
            filterButtons[i].OnMouseDown = (UIItem item) =>
            {
                ButtonTemplate b = item as ButtonTemplate;
                if (b.Go.name == "All")
                    ToolsUI.filter = ToolsUI.FilterType.all;
                if (b.Go.name == "Units")
                    ToolsUI.filter = ToolsUI.FilterType.units;
                if (b.Go.name == "Weapons")
                    ToolsUI.filter = ToolsUI.FilterType.weapons;
                if (b.Go.name == "Bonuses")
                    ToolsUI.filter = ToolsUI.FilterType.bonuses;
                PutUnitsToBase();
                SetupInventorySlots();
                foreach (var button in filterButtons)
                {
                    if (button != b)
                    {
                        button.Deselect();
                    }
                }

            };
            filterButtons[i].OnMouseUp = (UIItem item) =>
            {

            };
        }
    }
}
