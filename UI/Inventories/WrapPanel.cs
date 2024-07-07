using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WrapPanel : UIItem
{
    public LoadingSlotTemplate qBonusSlot;
    public LoadingSlotTemplate eBonusSlot;
    public SlotTemplate weaponSlot;
    public SlotTemplate armorSlot;
    public TickButtonTemplate autoPickup;
    private float inventoryDownY = -940;
    private float inventoryUpY = 0;

    public WrapPanel() : base(null, ToolsSystem.PauseType.Inventory, ToolsSystem.PauseType.InGame)
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
            GCon.PopPausedType();
            ToolsUI.SetCursor(ToolsUI.aimCursor);
            ToolsUI.DropDraggedObject();
        }, 0.4f), "reveal");
    }

    private void SetupPresetSlots()
    {

        qBonusSlot = new LoadingSlotTemplate(GameObject.FindGameObjectWithTag("qBonus"), false, false, false, (SlotTemplate slotable) =>
        {
            bool accepted = ToolsUI.draggedSlot.SlotableRef is Edible && eBonusSlot.SlotableRef != ToolsUI.draggedSlot.SlotableRef;
            if (accepted && ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot) && GCon.game.Player.PlayerControl.backpack.Count >= GCon.game.Player.PlayerControl.SlotSpace)
            {
                accepted = false;
            }
            return accepted;
        }, (SlotTemplate slot) =>
        {
            slot.ChangePlaceHolder("");
            GCon.game.Player.QBonus = (ToolsUI.draggedSlot.SlotableRef as Edible).DeepClone() as Edible;
            GCon.game.Player.PlayerControl.QBonusRef = (slot.SlotableRef as Edible);
            if (ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.backpack.Add(ToolsUI.draggedSlot.SlotableRef);
                GCon.game.Player.PlayerControl.RemoveFromBase(ToolsUI.draggedSlot.SlotableRef);
                ToolsUI.ActiveInventory.UpdateInventory();
            }
        }, (SlotTemplate slot) =>
        {
            GCon.game.Player.PlayerControl.backpack.Remove(slot.SlotableRef);
            GCon.game.Player.QBonus = null;
            slot.ChangePlaceHolder("Q");
        }, true, true, ToolsSystem.PauseType.InGame, ToolsSystem.PauseType.Inventory);



        eBonusSlot = new LoadingSlotTemplate(GameObject.FindGameObjectWithTag("eBonus"), false, false, false, (SlotTemplate slotable) =>
        {
            bool accepted = ToolsUI.draggedSlot.SlotableRef is Edible && qBonusSlot.SlotableRef != ToolsUI.draggedSlot.SlotableRef;
            if (accepted && ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot) && GCon.game.Player.PlayerControl.backpack.Count >= GCon.game.Player.PlayerControl.SlotSpace)
            {
                accepted = false;
            }
            return accepted;
        }, (SlotTemplate slot) =>
        {
            slot.ChangePlaceHolder("");
            GCon.game.Player.EBonus = (ToolsUI.draggedSlot.SlotableRef as Edible).DeepClone() as Edible;
            GCon.game.Player.PlayerControl.EBonusRef = (slot.SlotableRef as Edible);
            if (ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.backpack.Add(ToolsUI.draggedSlot.SlotableRef);
                GCon.game.Player.PlayerControl.RemoveFromBase(ToolsUI.draggedSlot.SlotableRef);
                ToolsUI.ActiveInventory.UpdateInventory();
            }
        }, (SlotTemplate slot) =>
        {
            GCon.game.Player.PlayerControl.backpack.Remove(slot.SlotableRef);
            GCon.game.Player.EBonus = null;
            slot.ChangePlaceHolder("E");
        }, true, true, ToolsSystem.PauseType.InGame, ToolsSystem.PauseType.Inventory);


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
            weaponSlot.ChangePlaceHolderImage(null);
            GCon.game.Player.Weapon = (slot.SlotableRef as CraftedWeapon).Weapon;
            GCon.game.Player.PlayerControl.WeaponSlotRef = (slot.SlotableRef as CraftedWeapon);
            if (ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.backpack.Add(ToolsUI.draggedSlot.SlotableRef);
                GCon.game.Player.PlayerControl.RemoveFromBase(ToolsUI.draggedSlot.SlotableRef);
                ToolsUI.ActiveInventory.UpdateInventory();
            }
        }, (SlotTemplate slot) =>
        {
            GCon.game.Player.Weapon = null;
            weaponSlot.ChangePlaceHolderImage(GameObjects.sling.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);
        }, true);
        weaponSlot.ChangePlaceHolderImage(GameObjects.sling.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);

        armorSlot = new SlotTemplate(GameObject.Find("ArmorSlot"), false, false, false, (SlotTemplate slotable) =>
        {
            bool accepted = ToolsUI.draggedSlot.SlotableRef is CraftedArmor;
            if (accepted && ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot) && GCon.game.Player.PlayerControl.backpack.Count >= GCon.game.Player.PlayerControl.SlotSpace)
            {
                accepted = false;
            }
            return accepted;
        }, (SlotTemplate slot) =>
        {
            armorSlot.ChangePlaceHolderImage(null);
            GCon.game.Player.Armor = (slot.SlotableRef as CraftedArmor).Armor;
            GCon.game.Player.PlayerControl.ArmorSlotRef = (slot.SlotableRef as CraftedArmor);
            if (ToolsUI.baseInventory.baseSlots.Contains(ToolsUI.draggedSlot))
            {
                GCon.game.Player.PlayerControl.backpack.Add(ToolsUI.draggedSlot.SlotableRef);
                GCon.game.Player.PlayerControl.RemoveFromBase(ToolsUI.draggedSlot.SlotableRef);
                ToolsUI.ActiveInventory.UpdateInventory();
            }
        }, (SlotTemplate slot) =>
        {
            GCon.game.Player.Armor = null;
            armorSlot.ChangePlaceHolderImage(GameObjects.basicArmor.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);
        }, true);
        armorSlot.ChangePlaceHolderImage(GameObjects.basicArmor.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);

        eBonusSlot.ChangePlaceHolder("E");
        qBonusSlot.ChangePlaceHolder("Q");
        autoPickup = new TickButtonTemplate(GameObject.FindGameObjectWithTag("AutoPickup"), true, () => { GCon.game.Player.PlayerControl.AutoPickup = true; }, () => { GCon.game.Player.PlayerControl.AutoPickup = false; });

    }

    public void LoadUI()
    {
        autoPickup.Ticked = GCon.game.Player.PlayerControl.AutoPickup;
        if (GCon.game.Player.PlayerControl.WeaponSlotRef != null)
        {
            weaponSlot.AddSlotable(GCon.game.Player.PlayerControl.WeaponSlotRef);
        }
        if (GCon.game.Player.PlayerControl.ArmorSlotRef != null)
        {
            armorSlot.AddSlotable(GCon.game.Player.PlayerControl.ArmorSlotRef);
        }
        if (GCon.game.Player.PlayerControl.QBonusRef != null)
        {
            qBonusSlot.AddSlotable(GCon.game.Player.PlayerControl.QBonusRef);
        }
        if (GCon.game.Player.PlayerControl.EBonusRef != null)
        {
            eBonusSlot.AddSlotable(GCon.game.Player.PlayerControl.EBonusRef);
        }
    }

}

