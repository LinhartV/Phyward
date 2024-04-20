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
    public TimeEnemy(float coef, Tilemap map = null) : base(0.5f, new ToAndFroIdleMovement(), 7, 3, 0.5f, 100, GameObjects.timeEnemy)
    {
        SetAngle = true;
        AddAction(new ItemAction("checkDistance", 1));
    }

    public override void Drop()
    {
        List<ToolsPhyward.Drop> l = new List<ToolsPhyward.Drop>
        {
            new ToolsPhyward.Drop(1, 1, 1, () => { return new Unit(Units.time, GameObjects.time); })
        };
        ToolsPhyward.DropDrops(l, this.Prefab.transform.position);
    }
    [JsonIgnore]
    private static bool staticCalled = false;
    protected override void SetupItem()
    {
        base.SetupItem();
        if (!staticCalled)
        {
            staticCalled = true;
            lambdaActions.Add("checkDistance", (item, parameter) =>
            {
                var en = item as TimeEnemy;
                if (ToolsMath.GetDistance(en, GCon.game.Player) < 5)
                {
                    en.StopIdleMovement();
                    en.AddAction(new ItemAction("sprintTowardsPlayer", 1));
                    en.AddControlledMovement(new RealAcceleratedMovement(1, ToolsMath.GetAngleFromLengts(GCon.game.Player, en), en.Acceleration, 5), "sprint");
                    en.DeleteAction("checkDistance");
                    en.AddAction(new ItemAction("dispose", ToolsMath.SecondsToFrames(400), ItemAction.ExecutionType.OnlyFirstTime));
                }
            });
            lambdaActions.Add("sprintTowardsPlayer", (item, parameter) =>
            {
                var en = item as TimeEnemy;
                en.RotateControlledMovement("sprint", ToolsMath.GetAngleFromLengts(en, GCon.game.Player), false);
                en.UpdateControlledMovement("sprint");
            });
        }
    }


}


