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
public class SwarmEnemy : Enemy
{
    public SwarmEnemy()
    {
    }
    /// <param name="coef">Boost of certain parameters - larger number means harder enemy</param>
    public SwarmEnemy(float coef, List<ToolsPhyward.Drop> l = null) : base(coef, 0.2f, new ToAndFroIdleMovement(), 6, 0.6f, 0.1f, 70, GameObjects.swarmEnemy)
    {
        if (l == null)
        {
            dropList = new List<ToolsPhyward.Drop>
            {
                new ToolsPhyward.Drop(1, 1, 1f, () => { return new Unit(Units.Length()); }),
                new ToolsPhyward.Drop(1, 1, 0.5f, () => { return new Unit(Units.Mass()); }),
                new ToolsPhyward.Drop(1, 1, 0.5f, () => { return new Unit(Units.Time()); })
            };
        }
        else dropList = l;
        AddAction(new ItemAction("checkDistance", 1, this));
        this.Weapon = new SwarmWeapon(1, 1, 7 * Coef, 8, ToolsMath.SecondsToFrames(4), 0.4f, GameObjects.fireSwarmShot);
    }

    [JsonIgnore]
    private static bool staticCalled = false;
    protected override void SetupItem()
    {
        base.SetupItem();
        if (!staticCalled)
        {
            staticCalled = true;
            AddItemLambdaAction("checkDistance", this, (item, parameter) =>
            {
                var en = item as Enemy;
                if (ToolsMath.GetDistance(en, GCon.game.Player) < 5)
                {
                    en.StopIdleMovement();
                    en.AddAction(new ItemAction("startSprintTowardsPlayer", 2, en, ItemAction.ExecutionType.OnlyFirstTime));
                    en.AddControlledMovement(new RealAcceleratedMovement(0, ToolsMath.GetAngleFromLengts(en, GCon.game.Player), en.Acceleration * en.Coef, en.BaseSpeed * en.Coef), "sprint");
                    en.DeleteAction("checkDistance", en);
                }
            });
            AddItemLambdaAction("stopAndShoot", this, (item, parameter) =>
            {
                var en = item as Enemy;
                en.DeleteAction("sprintTowardsPlayer", en);
                en.SetAngle = false;
                en.AddAction(new ItemAction("fire", 10 + 10 * (1 - Coef), en, ItemAction.ExecutionType.EveryTime));
                en.AddAction(new ItemAction("startSprintTowardsPlayer", 75, en, ItemAction.ExecutionType.OnlyFirstTime));
                en.AddAction(new ItemAction("lookAtPlayer", 1, en));
            });
            AddItemLambdaAction("fire", this, (item, parameter) =>
            {
                var en = item as Enemy;
                en.Weapon.Fire();
            });
            AddItemLambdaAction("startSprintTowardsPlayer", this, (item, parameter) =>
            {
                var en = item as Enemy;
                en.SetAngle = true;
                en.DeleteAction("lookAtPlayer", en);
                en.DeleteAction("fire", en);
                en.AddAction(new ItemAction("stopAndShoot", 200 + (1 - en.Coef) * 100, en, ItemAction.ExecutionType.OnlyFirstTime));
                en.AddAction(new ItemAction("sprintTowardsPlayer", 1, en));

            });
            AddItemLambdaAction("sprintTowardsPlayer", this, (item, parameter) =>
            {
                var en = item as Enemy;
                en.UpdateControlledMovement("sprint");
                en.RotateControlledMovement("sprint", ToolsMath.GetAngleFromLengts(en, GCon.game.Player), false);

            });
            LambdaActions.AddItemLambdaAction("lookAtPlayer", this, (item, parameter) =>
            {
                Enemy en = item as Enemy;
                en.Angle = ToolsMath.GetAngleFromLengts(en, GCon.game.Player);
            });
        }
    }


}


