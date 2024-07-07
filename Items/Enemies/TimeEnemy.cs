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
public class TimeEnemy : Enemy
{
    public TimeEnemy()
    {
    }
    /// <param name="coef">Boost of certain parameters - larger number means harder enemy</param>
    public TimeEnemy(float coef, List<ToolsPhyward.Drop> l = null) : base(coef, 0.5f, new ToAndFroIdleMovement(), 7, 3, 0.5f, 40, GameObjects.redSmallEnemy)
    {
        if (l == null)
        {
            dropList = new List<ToolsPhyward.Drop>
            {
                new ToolsPhyward.Drop(1, 1, 1, () => { return new Unit(Units.Time()); })
            };
        }
        else dropList = l;
        SetAngle = true;
        AddAction(new ItemAction("checkDistanceTimeEnemy", 1));
    }


    [JsonIgnore]
    private static bool staticCalled = false;
    protected override void SetupItem()
    {
        base.SetupItem();
        if (!staticCalled)
        {
            staticCalled = true;
            lambdaActions.Add("checkDistanceTimeEnemy", (item, parameter) =>
            {
                var en = item as TimeEnemy;
                if (ToolsMath.GetDistance(en, GCon.game.Player) < 6)
                {
                    en.StopIdleMovement();
                    en.AddAction(new ItemAction("sprintTowardsPlayer", 1));
                    en.AddControlledMovement(new RealAcceleratedMovement(1 * en.Coef, ToolsMath.GetAngleFromLengts(GCon.game.Player, en), en.Acceleration * 2 * en.Coef, 5 * en.Coef), "sprint");
                    en.DeleteAction("checkDistanceTimeEnemy");
                    en.AddAction(new ItemAction("receiveDamage", 1, ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.Freeze, null, 0.05 * (1.6f - en.Coef)));
                }
            });
            lambdaActions.Add("sprintTowardsPlayer", (item, parameter) =>
            {
                var en = item as Movable;
                en.RotateControlledMovement("sprint", ToolsMath.GetAngleFromLengts(en, GCon.game.Player), false);
                en.UpdateControlledMovement("sprint");
            });
        }
    }


}


