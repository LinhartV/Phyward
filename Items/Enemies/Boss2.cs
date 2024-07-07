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
public class Boss2 : Enemy
{
    private Vector2 playerPos;
    public Boss2()
    {
    }
    /// <param name="coef">Boost of certain parameters - larger number means harder enemy</param>
    public Boss2(float coef, List<ToolsPhyward.Drop> l = null) : base(coef, 0.6f, new NoIdleMovement(), 20, 10f, 1.5f, 250, GameObjects.boss2)
    {
        if (l == null)
        {
            dropList = new List<ToolsPhyward.Drop>
            {
                new ToolsPhyward.Drop(1, 1, 1, () => { return new Collectable(new Scroll(Units.frequency)); })
            };
        }
        else dropList = l;
        this.Weapon = new SwarmWeapon(1, 1, 15 * Coef, 5, ToolsMath.SecondsToFrames(4), 0.3f, GameObjects.fireSwarmShot);
        StopIdleMovement();
        AddControlledMovement(new RealAcceleratedMovement(0, ToolsMath.GetAngleFromLengts(this, GCon.game.Player), Acceleration, BaseSpeed), "sprint");

        AddAction(new ItemAction("startSprint", 2, this, ItemAction.ExecutionType.OnlyFirstTime));
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
                Boss2 en = item as Boss2;
                en.UpdateControlledMovement("sprint");
                if (Math.Abs(en.playerPos.x - en.Prefab.transform.position.x) < 3 && Math.Abs(en.playerPos.y - en.Prefab.transform.position.y) < 3)
                {
                    en.DeleteAction("sprint", en);
                    en.AddAction(new ItemAction("checkStop", 1, this));
                }

            });
            LambdaActions.AddItemLambdaAction("startSprint", this, (item, parameter) =>
            {
                Boss2 en = item as Boss2;
                en.playerPos = GCon.game.Player.Prefab.transform.position;
                en.RotateControlledMovement("sprint", ToolsMath.GetAngleFromLengts(en, GCon.game.Player), false);
                en.AddAction(new ItemAction("sprint", 1, this, ItemAction.ExecutionType.EveryTime));
                en.DeleteAction("startSprint", en);
                en.DeleteAction("firing", en);
                en.DeleteAction("spawning", en);
                en.DeleteAction("lookAtPlayer", en);
                en.SetAngle = true;

            });
            LambdaActions.AddItemLambdaAction("checkStop", this, (item, parameter) =>
            {

                Boss2 en = item as Boss2;
                if (en.GetMovementSpeed() < 1)
                {
                    en.DeleteAction("checkStop", en);
                    float rng = ToolsGame.Rng();
                    if (rng < 0.5)
                    {
                        en.AddAction(new ItemAction("startSprint", 5, this, ItemAction.ExecutionType.OnlyFirstTime));
                    }
                    else if (rng < 0.86)
                    {
                        en.AddAction(new ItemAction("firing", 30 - (en.Coef * 10), this, ItemAction.ExecutionType.EveryTime));
                        en.AddAction(new ItemAction("lookAtPlayer", 1, this, ItemAction.ExecutionType.EveryTime));
                        en.AddAction(new ItemAction("startSprint", ToolsGame.Rng(200, 500) * en.Coef, this, ItemAction.ExecutionType.OnlyFirstTime));

                    }
                    else
                    {
                        en.AddAction(new ItemAction("spawning", 100 - (en.Coef * 10), this, ItemAction.ExecutionType.EveryTime));
                        en.AddAction(new ItemAction("lookAtPlayer", 1, this, ItemAction.ExecutionType.EveryTime));
                        en.AddAction(new ItemAction("startSprint", ToolsGame.Rng(150, 200) * en.Coef, this, ItemAction.ExecutionType.OnlyFirstTime));

                    }
                }
            });
            LambdaActions.AddItemLambdaAction("spawning", this, (item, parameter) =>
            {
                Boss2 en = item as Boss2;
                GCon.game.CurLevel.AddItem(new ShootingEnemy(0.59f, new List<Drop>()) { IsMinion = true }, en.Prefab.transform.position);
            });
            LambdaActions.AddItemLambdaAction("firing", this, (item, parameter) =>
            {
                Boss2 en = item as Boss2;
                en.Weapon.Fire();
            });
            LambdaActions.AddItemLambdaAction("lookAtPlayer", this, (item, parameter) =>
            {
                Boss2 en = item as Boss2;
                en.SetAngle = false;
                en.Angle = ToolsMath.GetAngleFromLengts(en, GCon.game.Player);
            });
        }
    }


}


