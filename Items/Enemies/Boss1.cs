using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static LambdaActions;
using static ToolsPhyward;

/// <summary>
/// Chodí vždy v intervalech - jde rovně, chvilku stojí, jde rovně, chvilku stojí atd.
/// </summary>
public class Boss1 : Enemy
{
    public Boss1()
    {
    }
    /// <param name="coef">Boost of certain parameters - larger number means harder enemy</param>
    public Boss1(float coef, List<ToolsPhyward.Drop> l = null) : base(coef, 0.5f, new NoIdleMovement(), 3, 2f, 0.9f, 150, GameObjects.boss1)
    {
        if (l == null)
        {
            dropList = new List<ToolsPhyward.Drop>
            {
                new ToolsPhyward.Drop(1, 1, 1, () => { return new Collectable(new Scroll(Units.speed,()=>{GCon.game.TutorialPhase = 6;})); })
            };
        }
        else dropList = l;
        this.Weapon = new BasicWeapon(1, 10 * Coef, 5, ToolsMath.SecondsToFrames(5), 0.1f, GameObjects.blackShot);
        StopIdleMovement();
        AddControlledMovement(new ConstantMovement(BaseSpeed * Coef, ToolsMath.GetAngleFromLengts(this, GCon.game.Player)), "sprint");

        AddAction(new ItemAction("sprint", 1, this, ItemAction.ExecutionType.EveryTime));
        AddAction(new ItemAction("nextMove", 200, this, ItemAction.ExecutionType.OnlyFirstTime));
    }



    public override bool Death()
    {
        if (!base.Death())
        {
            if (GCon.game.TutorialPhase == 5)
            {
                Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Vítězství!", "Nepřítel měl u sebe svitek... snad nemusím vysvětlovat, že bys ho měl sebrat...", ""));
            }
            return false;
        }
        return true;
    }
    [JsonIgnore]
    private static bool staticCalled = false;
    protected override void SetupItem()
    {
        base.SetupItem();
        if (!staticCalled)
        {
            staticCalled = true;
            LambdaActions.AddItemLambdaAction("sprint", this, (item, parameter) =>
            {
                Boss1 en = item as Boss1;
                en.UpdateControlledMovement("sprint");
                en.RotateControlledMovement("sprint", ToolsMath.GetAngleFromLengts(en, GCon.game.Player), false);

            });
            LambdaActions.AddItemLambdaAction("nextMove", this, (item, parameter) =>
            {
                Boss1 en = item as Boss1;
                en.DeleteAction("sprint", en);
                en.AddAction(new ItemAction("lookAtPlayer", 1, en, ItemAction.ExecutionType.EveryTime));
                en.AddAction(new ItemAction("nextMoveDelayed", ToolsGame.Rng(40, 80), en, ItemAction.ExecutionType.OnlyFirstTime));
            });
            LambdaActions.AddItemLambdaAction("nextMoveDelayed", this, (item, parameter) =>
            {
                Boss1 en = item as Boss1;
                float decide = ToolsGame.Rng();
                if (decide < 1 / 3f)
                {
                    en.AddAction(new ItemAction("fastSprint", 1, en, ItemAction.ExecutionType.EveryTime));
                    en.AddControlledMovement(new RealAcceleratedMovement(0, ToolsMath.GetAngleFromLengts(GCon.game.Player, en), en.Acceleration * 3 * en.Coef, en.BaseSpeed * 5 * en.Coef, 1, true), "fastSprint");
                    en.AddAction(new ItemAction("backToNormal", ToolsGame.Rng(300, 500) + en.Coef * 40, en, ItemAction.ExecutionType.OnlyFirstTime));
                    en.DeleteAction("lookAtPlayer", en);
                }
                else if (decide < 2 / 3f)
                {
                    en.DeleteAction("lookAtPlayer", en);
                    en.AddAction(new ItemAction("sprint", 1, en));
                    en.AddAction(new ItemAction("fire", 30, en, ItemAction.ExecutionType.EveryTime));
                    en.AddAction(new ItemAction("backToNormal", ToolsGame.Rng(100, 200) + en.Coef * 40, en, ItemAction.ExecutionType.OnlyFirstTime));
                }
                else
                {
                    en.AddAction(new ItemAction("fire", 5, en, ItemAction.ExecutionType.EveryTime));
                    en.AddAction(new ItemAction("backToNormal", ToolsGame.Rng(50, 150) + en.Coef * 40, en, ItemAction.ExecutionType.OnlyFirstTime));
                }
            });
            LambdaActions.AddItemLambdaAction("fastSprint", this, (item, parameter) =>
            {
                Boss1 en = item as Boss1;
                en.UpdateControlledMovement("fastSprint");
                en.RotateControlledMovement("fastSprint", ToolsMath.GetAngleFromLengts(en, GCon.game.Player), false);
            });
            LambdaActions.AddItemLambdaAction("backToNormal", this, (item, parameter) =>
            {
                Boss1 en = item as Boss1;
                en.AddAction(new ItemAction("sprint", 1, en));
                en.DeleteAction("fastSprint", en);
                en.DeleteAction("fire", en);
                en.DeleteAction("lookAtPlayer", en);
                en.AddAction(new ItemAction("nextMove", ToolsGame.Rng(50, 250) - en.Coef * 10, en, ItemAction.ExecutionType.OnlyFirstTime));

            });
            LambdaActions.AddItemLambdaAction("fire", this, (item, parameter) =>
            {
                Boss1 en = item as Boss1;
                en.Weapon.Fire();
            });
            LambdaActions.AddItemLambdaAction("lookAtPlayer", this, (item, parameter) =>
            {
                Boss1 en = item as Boss1;
                en.Angle = ToolsMath.GetAngleFromLengts(en, GCon.game.Player);
            });
        }
    }


}


