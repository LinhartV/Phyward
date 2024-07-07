using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : IUnslotable
{
    public PreUnit Unit { get; private set; }
    public Action OnClose { get; private set; }
    public Scroll(PreUnit unit, Action onClose = null) : base(GameObjects.scroll)
    {
        this.Unit = unit;
        this.OnClose = onClose;
    }

    public override void ActionWhenCollected()
    {
        GCon.game.Player.PlayerControl.DiscoverNewUnit(Unit);
        GCon.game.CurBiom.ScrollsCollected++;
        ToolsUI.TriggerScrollPanel(this);
    }

    public override ICollectableRef DeepClone()
    {
        return new Scroll(Unit, OnClose);
    }
}

