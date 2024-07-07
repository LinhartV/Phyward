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
public class FinalBoss : Enemy
{
    private Vector2 randomPos;
    private IWeapon thirdWeapon;
    public FinalBoss()
    {
    }
    /// <param name="coef">Boost of certain parameters - larger number means harder enemy</param>
    public FinalBoss(float coef, List<ToolsPhyward.Drop> l = null) : base(coef, 0.7f, new NoIdleMovement(), 25, 8f, 5f, 400, GameObjects.finalBoss)
    {
        if (l == null)
        {
            dropList = new List<ToolsPhyward.Drop>();
        }
        else dropList = l;

        Weapon = new CrumblingWeapon(null, 3, 1, 8 * Coef, 11, ToolsMath.SecondsToFrames(0.7f), 0f, GameObjects.blueShot);
        thirdWeapon = new CrumblingWeapon((pos, id, angle) => { return new BasicShot(pos,10,id,ToolsMath.SecondsToFrames(5),10,5,angle,2,1,0,GameObjects.redSmallShot); }, 2, 1, 7 * Coef, 5, ToolsMath.SecondsToFrames(0.8f), 0f, GameObjects.redSmallShot);
        thirdWeapon.Character = this;

        StopIdleMovement();
        AddControlledMovement(new RealAcceleratedMovement(0, ToolsMath.GetAngleFromLengts(this, GCon.game.Player), Acceleration, BaseSpeed, 1, true), "sprint");

        SetAngle = false;
    }

    public override void OnLevelEnter()
    {
        base.OnLevelEnter();
        randomPos = GCon.game.CurLevel.GetEmptyPosition();
        RotateControlledMovement("sprint", ToolsMath.GetAngleFromLengts(randomPos.x - Prefab.transform.position.x, randomPos.y - Prefab.transform.position.y), false);
        AddAction(new ItemAction("sprint", 1, this));
        AddAction(new ItemAction("lookAtPlayer", 1, this));
        AddAction(new ItemAction("checkHealth", 1, this));
        DeleteAction("firing");
        DeleteAction("firingMain");
        DeleteAction("checkHealth2");
        DeleteAction("spawning");
        DeleteAction("startSprint");
        this.Prefab.transform.position = new Vector2(GCon.game.CurLevel.Maze.GetLength(1) / 2, GCon.game.CurLevel.Maze.GetLength(0) - 5);
    }

    [JsonIgnore]
    private static bool staticCalled = false;

    public override bool Death()
    {
        if (!base.Death())
        {
            ToolsUI.TriggerVictoryPanel();
            return false;
        }
        return true;

    }



    protected override void SetupItem()
    {
        base.SetupItem();
        if (!staticCalled)
        {
            staticCalled = true;
            LambdaActions.AddItemLambdaAction("sprint", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                en.RotateControlledMovement("sprint", ToolsMath.GetAngleFromLengts(en.randomPos.x - en.Prefab.transform.position.x, en.randomPos.y - en.Prefab.transform.position.y), false);
                if (Math.Abs(en.randomPos.x - this.Prefab.transform.position.x) < 3 && Math.Abs(en.randomPos.y - this.Prefab.transform.position.y) < 3)
                {
                    en.randomPos = GCon.game.CurLevel.GetEmptyPosition();
                    if (en.LivedHandler.Lives < en.LivedHandler.MaxLives * 3 / 4)
                    {
                        en.DeleteAction("sprint", en);
                        en.AddAction(new ItemAction("firingMain", 40 / en.Coef, en), 50);
                        en.AddAction(new ItemAction("startSprint", 200, en, ItemAction.ExecutionType.OnlyFirstTime));
                    }
                    else
                    {
                        en.Weapon.Fire();
                    }
                }

            });
            LambdaActions.AddItemLambdaAction("startSprint", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                en.AddAction(new ItemAction("sprint", 1, en));
                en.DeleteAction("firingMain", en);
            });
            LambdaActions.AddItemLambdaAction("lookAtPlayer", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                en.Angle = ToolsMath.GetAngleFromLengts(en, GCon.game.Player);
            });
            LambdaActions.AddItemLambdaAction("firing", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                en.Weapon.Fire();
            });
            LambdaActions.AddItemLambdaAction("firingMain", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                en.thirdWeapon.Fire();
            });
            LambdaActions.AddItemLambdaAction("checkHealth", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                if (en.LivedHandler.Lives < en.LivedHandler.MaxLives / 2)
                {
                    en.AddAction(new ItemAction("firing", 150, en));
                    en.AddAction(new ItemAction("checkHealth2", 1, en));
                    en.DeleteAction("checkHealth", en);
                }
            });
            LambdaActions.AddItemLambdaAction("checkHealth2", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                if (en.LivedHandler.Lives < en.LivedHandler.MaxLives / 4)
                {
                    en.AddAction(new ItemAction("spawning", 400, en));
                    en.DeleteAction("checkHealth2", en);
                }
            });
            LambdaActions.AddItemLambdaAction("spawning", this, (item, parameter) =>
            {
                FinalBoss en = item as FinalBoss;
                GCon.game.CurLevel.AddItem(new TimeEnemy(en.Coef, new List<Drop>()) { IsMinion = true }, en.Prefab.transform.position);

            });
        }
    }


}


