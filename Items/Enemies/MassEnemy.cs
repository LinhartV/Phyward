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
public class MassEnemy : Enemy
{
    public MassEnemy()
    {
    }
    /// <param name="coef">Boost of certain parameters - larger number means harder enemy</param>
    public MassEnemy(float coef) : base(coef, 0.6f, new ToAndFroIdleMovement(), 6, 1.5f, 0.8f, 70, GameObjects.purpleEnemy)
    {
        SetAngle = true;
        AddAction(new ItemAction("checkDistanceMassEnemy", 1));
    }

    public override void Drop()
    {
        List<ToolsPhyward.Drop> l = new List<ToolsPhyward.Drop>
        {
            new ToolsPhyward.Drop(1, 5, 1, () => { return new Unit(Units.Mass()); })
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
            lambdaActions.Add("checkDistanceMassEnemy", (item, parameter) =>
            {
                var en = item as Enemy;
                if (ToolsMath.GetDistance(en, GCon.game.Player) < 4)
                {
                    en.StopIdleMovement();
                    en.AddAction(new ItemAction("sprintTowardsPlayer", 1));
                    en.AddControlledMovement(new RealAcceleratedMovement(1.5f * en.Coef, ToolsMath.GetAngleFromLengts(GCon.game.Player, en), en.Acceleration * 2 * en.Coef, BaseSpeed * en.Coef), "sprint");
                    en.DeleteAction("checkDistanceMassEnemy");
                }
            });
        }
    }


}


