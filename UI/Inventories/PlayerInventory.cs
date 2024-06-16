using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ToolsUI;

public class PlayerInventory : Inventory
{
    private SlotTemplate armorSlot;
    /// <summary>
    /// Slots in "Actual baseSlots"
    /// </summary>
    public List<SlotTemplate> slots = new List<SlotTemplate>();
    public SlotTemplate binSlot;
    public ButtonTemplate[] filterButtons = new ButtonTemplate[4];

    public PlayerInventory(UIItem panel) : base(panel)
    {
    }

    protected override void SetupInventory()
    {
        panel.Go.SetActive(true);
        SetupPresetSlots();
        UpdateInventory();
    }
    private void SetupSlots(int count)
    {
        ToolsUI.DraggedSlot = null;
        slots.Clear();
        const float slotWidth = Constants.SLOT_WIDTH;
        const float minOffset = 30;
        var slotsInventory = new UIItem(GameObject.FindGameObjectWithTag("Slots").transform.gameObject);
        float width = slotsInventory.Go.GetComponent<RectTransform>().rect.width;
        float height = slotsInventory.Go.GetComponent<RectTransform>().rect.height;
        int widthCount = (int)Math.Floor(width / (slotWidth + minOffset));
        int heightCount = (int)Math.Floor(height / (slotWidth + minOffset));
        float offset = ((int)width - widthCount * slotWidth) / (widthCount + 1);
        int slotCount = count;//GCon.game.Player.PlayerControl.SlotSpace;
        bool doubleBreak = false;
        for (int i = 0; i < heightCount; i++)
        {
            for (int j = 0; j < widthCount; j++)
            {
                if (slotCount <= j + i * widthCount)
                {
                    doubleBreak = true;
                    break;
                }
                var slotUI = new SlotTemplate(slotWidth, true, true, false, slotsInventory.Go, new Vector3(offset + j * (slotWidth + offset) + slotWidth / 2, -offset - i * (slotWidth + offset) - slotWidth / 2), null, null, null, true);
                slots.Add(slotUI);
            }
            if (doubleBreak)
            {
                break;
            }
        }
    }

    public override bool OpenInventory()
    {
        if (base.OpenInventory())
        {
            UpdateInventory();
            return true;
        }
        return false;
    }

    public override void UpdateInventory()
    {
        GCon.game.Player.PlayerControl.backpack.Sort();
        foreach (var slot in slots)
        {
            slot.Dispose();
        }
        //count baseSlots
        int negativeCount = 0;
        for (int i = 0; i < GCon.game.Player.PlayerControl.backpack.Count; i++)
        {
            if (!(filter == GCon.game.Player.PlayerControl.backpack[i].filter || filter == FilterType.all))
            {
                negativeCount++;
            }
        }
        SetupSlots(GCon.game.Player.PlayerControl.SlotSpace - negativeCount);
        int counter = 0;
        for (int i = 0; i < GCon.game.Player.PlayerControl.backpack.Count; i++)
        {
            var item = GCon.game.Player.PlayerControl.backpack[i];
            if (filter == item.filter || filter == FilterType.all)
            {
                slots[i - counter].AddSlotable(GCon.game.Player.PlayerControl.backpack[i]);
            }
            else
            {
                counter++;
            }
        }
    }
    private void SetupPresetSlots()
    {
        SetupFilterButtons();
        armorSlot = new SlotTemplate(GameObject.FindGameObjectWithTag("Armor"), false, false, false, (SlotTemplate slotable) => { return ToolsUI.draggedSlot.SlotableRef is Armor; }, (SlotTemplate slot) => { /*TODO Add armor*/});
        binSlot = new SlotTemplate(GameObject.FindGameObjectWithTag("Bin"), true, false, true, (SlotTemplate slotable) => { return true; }, (SlotTemplate slot) =>
        {
            if (ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.RemoveFromBase(DraggedSlot.SlotableRef);
            }
            if (ToolsUI.playerInventory.slots.Contains(ToolsUI.draggedSlot) || ToolsUI.baseInventory.backpackSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.RemoveFromBackpack(DraggedSlot.SlotableRef);
            }
            //slot.RemoveSlotable();
            ActiveInventory.UpdateInventory();
        }, null, true);
    }


    public override void LoadUI()
    {
    }
    private void SetupFilterButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            filterButtons[i] = new ButtonTemplate(GameObject.FindGameObjectWithTag("Filters").transform.GetChild(i).gameObject, true, true);
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
                UpdateInventory();
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
