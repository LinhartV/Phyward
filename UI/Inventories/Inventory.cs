using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Inventory
{
    protected Inventory(UIItem panel)
    {
        this.panel = panel;
        SetupInventory();
        panel.Go.transform.localPosition = new Vector3(0, -940);
        panel.AddTransition(new Lerpable(0.7f, new Vector2(0, 940), ToolsUI.easeOut, true, true, () => { OnOpenInventory(); }, () =>
        {
            OnClosedInventory();
        }), "reveal");
    }
    public UIItem panel;
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Whether inventory is already opened</returns>
    public virtual bool OpenInventory()
    {
        if (!ToolsUI.openInventories.Contains(this))
        {
            ToolsUI.openInventories.Push(this);
            ToolsUI.ActiveInventory = this;
            if (ToolsUI.openInventories.Count > 1)
            {
                panel.Go.transform.localPosition = new Vector3(0, -940);
                this.StartRevealing();
            }
            else
            {
                panel.Go.transform.localPosition = new Vector3(0, 0);
                ToolsUI.wrapPanel.StartTransition("reveal");
            }
            if (GCon.GetPausedType() != ToolsSystem.PauseType.Inventory)
            {
                GCon.AddPausedType(ToolsSystem.PauseType.Inventory);
            }
            ToolsUI.SetCursor(ToolsUI.normalCursor);
            UpdateInventory();
            return true;
        }
        return false;
    }
    public virtual void CloseInventory()
    {
        if (ToolsUI.openInventories.Count > 1)
        {
            ToolsUI.openInventories.ElementAt(1).panel.Go.SetActive(true);
            ToolsUI.openInventories.ElementAt(1).UpdateInventory();
            panel.ReturnTransition("reveal");
        }
        else
        {
            ToolsUI.wrapPanel.ReturnTransition("reveal");
        }
    }
    public virtual void StartRevealing()
    {
        this.panel.StartTransition("reveal");
    }
    public virtual void OnOpenInventory()
    {
        ToolsUI.descriptionPanel.Go.SetActive(false);
        if (ToolsUI.openInventories.Count > 1)
        {
            ToolsUI.openInventories.ElementAt(1).panel.Go.SetActive(false);
        }
    }
    /// <summary>
    /// Call this when the inventory is fully closed
    /// </summary>
    public void OnClosedInventory()
    {
        ToolsUI.openInventories.Pop();
        if (ToolsUI.openInventories.Count > 0)
        {
            ToolsUI.ActiveInventory = ToolsUI.openInventories.Peek();
        }
        else
        {
            ToolsUI.ActiveInventory = ToolsUI.playerInventory;
        }
    }
    protected abstract void SetupInventory();
    public abstract void UpdateInventory();
    public abstract void LoadUI();
}

