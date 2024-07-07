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
public class ShootingEnemy : Enemy
{
    public ShootingEnemy()
    {
    }
    /// <param name="coef">Boost of certain parameters - larger number means harder enemy</param>
    public ShootingEnemy(float coef, List<ToolsPhyward.Drop> l = null) : base(coef, 0.2f, new ToAndFroIdleMovement(), 2, 2f, 0.8f, 50, GameObjects.shootingEnemy)
    {
        if (l == null)
        {
            dropList = new List<ToolsPhyward.Drop>
            {
                new ToolsPhyward.Drop(1, 2, 1f, () => { return new Unit(Units.Length()); })
            };
        }
        else dropList = l;
        AddAction(new ItemAction("checkDistanceShootingEnemy", 1));
        this.Weapon = new BasicWeapon(1, 10 * Coef, 5, ToolsMath.SecondsToFrames(2), 0.1f, GameObjects.blackShot);
    }

    [JsonIgnore]
    private static bool staticCalled = false;
    protected override void SetupItem()
    {
        base.SetupItem();
        if (!staticCalled)
        {
            staticCalled = true;
            lambdaActions.Add("checkDistanceShootingEnemy", (item, parameter) =>
            {
                var en = item as Enemy;
                if (ToolsMath.GetDistance(en, GCon.game.Player) < 7)
                {
                    en.StopIdleMovement();
                    en.AddAction(new ItemAction("sprintTowardsPlayer", 1));
                    en.AddAction(new ItemAction("stopAndShoot", 200 + (1 - en.Coef) * 100));
                    en.AddControlledMovement(new ConstantMovement(en.BaseSpeed * en.Coef, ToolsMath.GetAngleFromLengts(GCon.game.Player, en)), "sprint");
                    en.DeleteAction("checkDistanceShootingEnemy");
                }
            });
            lambdaActions.Add("stopAndShoot", (item, parameter) =>
            {
                var en = item as Enemy;
                en.DeleteAction("sprintTowardsPlayer");
                en.AddAction(new ItemAction("sprintTowardsPlayer", 1), 20);
                en.AddAction(new ItemAction("shootingEnemyFire", 10, ItemAction.ExecutionType.OnlyFirstTime));
            });
            lambdaActions.Add("shootingEnemyFire", (item, parameter) =>
            {
                var en = item as Enemy;
                en.Weapon.Fire();
            });
        }
    }


}


