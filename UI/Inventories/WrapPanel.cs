using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WrapPanel : UIItem
{
    public SlotTemplate qBonusSlot;
    public SlotTemplate eBonusSlot;
    public SlotTemplate weaponSlot;
    public TickButtonTemplate autoPickup;
    private float inventoryDownY = -940;
    private float inventoryUpY = 0;

    public WrapPanel()
    {
        this.Go = GameObject.FindGameObjectWithTag("WrapInventory");
        this.referenceGo = this.Go;
        SetupInventory();
    }   
    public void OpenInventory()
    {
        StartTransition("reveal");
    }
    public void CloseInventory()
    {
        ReturnTransition("reveal");
    }

    private void SetupInventory()
    {
        SetupPresetSlots();
        Go.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, inventoryDownY);
        AddTransition(new Lerpable(0.7f, new Vector2(0, inventoryUpY), ToolsUI.easeOut, false, true, () => { ToolsUI.ActiveInventory.OnOpenInventory(); }, () =>
        {
            ToolsUI.ActiveInventory.OnClosedInventory();
            GCon.Paused = false;
            ToolsUI.SetCursor(ToolsUI.aimCursor);
            ToolsUI.DropDraggedObject();
        }, 0.4f), "reveal");
    }

    private void SetupPresetSlots()
    {
        
        qBonusSlot = new SlotTemplate(GameObject.FindGameObjectWithTag("qBonus"), false, false, false, (SlotTemplate slotable) => { return ToolsUI.draggedSlot.SlotableRef is Edible; });
        eBonusSlot = new SlotTemplate(GameObject.FindGameObjectWithTag("eBonus"), false, false, false, (SlotTemplate slotable) => { return ToolsUI.draggedSlot.SlotableRef is Edible; });
        weaponSlot = new SlotTemplate(GameObject.FindGameObjectWithTag("WeaponSlot"), false, false, false, (SlotTemplate slotable) =>
        {
            bool accepted = ToolsUI.draggedSlot.SlotableRef is CraftedWeapon;
            if (accepted && ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot) && GCon.game.Player.PlayerControl.backpack.Count >= GCon.game.Player.PlayerControl.SlotSpace)
            {
                accepted = false;
            }
            return accepted;
        }, (SlotTemplate slot) =>
        {
            GCon.game.Player.Weapon = (slot.SlotableRef as CraftedWeapon).Weapon;
            GCon.game.Player.PlayerControl.WeaponSlotRef = (slot.SlotableRef as CraftedWeapon);
            if (ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.backpack.Add(ToolsUI.draggedSlot.SlotableRef);
                GCon.game.Player.PlayerControl.RemoveFromBase(ToolsUI.draggedSlot.SlotableRef);
                ToolsUI.ActiveInventory.UpdateInventory();
            }
        }, (SlotTemplate slot) => { GCon.game.Player.Weapon = null; });
        autoPickup = new TickButtonTemplate(GameObject.FindGameObjectWithTag("AutoPickup"), true, () => { GCon.game.Player.PlayerControl.AutoPickup = true; }, () => { GCon.game.Player.PlayerControl.AutoPickup = false; });

    }

    public void LoadUI()
    {
        autoPickup.Ticked = GCon.game.Player.PlayerControl.AutoPickup;
        if (GCon.game.Player.PlayerControl.WeaponSlotRef != null)
        {
            weaponSlot.AddSlotable(GCon.game.Player.PlayerControl.WeaponSlotRef);
        }
    }
    
}

