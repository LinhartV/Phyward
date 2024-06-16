using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ButtonTemplate : UIItem
{
    public bool Selected { get; private set; } = false;
    public bool Hoverable { get; set; }
    private bool Selectable { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go">Reference to game object</param>
    /// <param name="hoverable">Whether to start hovering animation on hover</param>
    /// <param name="selectable">Whether this button can be selected</param>
    /// <param name="hoverTransition">Leave null for default transition</param>
    /// <param name="selectedTransition">Leave null for default transition</param>
    public ButtonTemplate(GameObject go, bool hoverable, bool selectable, Transitable hoverTransition = null, Transitable selectedTransition = null, ToolsSystem.PauseType pauseType = ToolsSystem.PauseType.Inventory) : base(go, pauseType)
    {
        this.Hoverable = hoverable;
        this.Selectable = selectable;
        if (hoverable)
        {
            if (hoverTransition == null)
            {
                hoverTransition = ColorChangable.StandardHover();
            }
            AddTransition(hoverTransition, "hover");
        }
        if (selectable)
        {
            if (selectedTransition == null)
            {
                selectedTransition = ColorChangable.StandardSelect();
            }
            AddTransition(selectedTransition, "select");
        }
        var box = Go.AddComponent<BoxCollider2D>();
        var rect = Go.GetComponent<RectTransform>().rect;
        box.offset = new Vector2(rect.width / 2, -rect.height / 2);
        box.size = new Vector2(rect.width, rect.height);
        box.isTrigger = true;
    }

    public void Deselect()
    {
        if (Selected)
        {
            ReturnTransition("select");
            Selected = false;
        }
    }

    public override void OnMouseEnterDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            if (ToolsUI.DraggedSlot == null)
            {
                base.OnMouseEnterDefault();
                if (Hoverable)
                {
                    StartTransition("hover", true);
                }
            }
        }
        //ToolsUI.SetCursor(ToolsUI.selectCursor);
    }
    public override void OnMouseExitDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            base.OnMouseExitDefault();
            if (Hoverable)
            {
                ReturnTransition("hover");
            }
        }
        //ToolsUI.SetCursor(ToolsUI.normalCursor);
    }
    public override void OnMouseDownDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            base.OnMouseDownDefault();
            if (Selectable && !Selected)
            {
                Selected = true;
                StartTransition("select");
            }
        }
    }
}
