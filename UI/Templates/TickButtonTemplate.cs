using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TickButtonTemplate : UIItem
{
    private bool ticked;
    public bool Ticked
    {
        get { return ticked; }
        set
        {
            ticked = value;
            if (value == true)
            {
                Go.transform.GetChild(1).gameObject.SetActive(true);
                if (onTicked != null)
                    onTicked();
            }
            else
            {
                Go.transform.GetChild(1).gameObject.SetActive(false);
                if (offTicked != null)
                    offTicked();
            }
        }
    }
    private Action onTicked;
    private Action offTicked;

    public TickButtonTemplate(GameObject go, bool ticked, Action onTicked, Action offTicked) : base(go, null)
    {
        Ticked = ticked;
        this.onTicked = onTicked;
        this.offTicked = offTicked;
    }

    public override void OnMouseEnterDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            if (ToolsUI.DraggedSlot == null)
            {
                base.OnMouseEnterDefault();
            }
            ToolsUI.SetCursor(ToolsUI.selectCursor);
        }
    }
    public override void OnMouseExitDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            base.OnMouseExitDefault();
            ToolsUI.SetCursor(ToolsUI.normalCursor);
        }
    }
    public override void OnMouseDownDefault()
    {
        if (this.pauseTypes.Contains(GCon.GetPausedType()))
        {
            base.OnMouseDownDefault();
            Ticked = !Ticked;
        }
    }
}
