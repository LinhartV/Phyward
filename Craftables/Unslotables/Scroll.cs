using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : IUnslotable
{
    private PreUnit unit;
    public Scroll(PreUnit unit) : base(GameObjects.scroll)
    {
        this.unit = unit;
    }

    public override void ActionWhenCollected()
    {
        GCon.game.Player.PlayerControl.DiscoverNewUnit(unit);
        ToolsUI.TriggerScrollPanel(unit);
    }
   
    public override ICollectableRef DeepClone()
    {
        return new Scroll(unit);
    }
}

