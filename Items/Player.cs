using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class for managing exits from a particular level.
/// </summary>
[Serializable]
public class Player : Character
{
    private new geniikw.DataRenderer2D.UILine damagebar;
    public PlayerControl PlayerControl { get; private set; } = new PlayerControl(true);
    public Edible QBonus { get; set; }
    public Edible EBonus { get; set; }

    public Player() { }

    public Player(Vector2 pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, Armor armor, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, weapon, charDamage, charReloadTime, charShotSpeed, charShotDuration, lives, GameObjects.player, true, true, armor, map)
    {
        Dictionary<string, IMovement> movements = new Dictionary<string, IMovement>();
        movements.Add("up", new AcceleratedMovement(0, 0, this.Acceleration, BaseSpeed));
        movements.Add("right", new AcceleratedMovement(0, (float)Math.PI / 2, this.Acceleration, BaseSpeed));
        movements.Add("down", new AcceleratedMovement(0, (float)Math.PI, this.Acceleration, BaseSpeed));
        movements.Add("left", new AcceleratedMovement(0, (float)Math.PI * 3 / 2, this.Acceleration, BaseSpeed));
        AddControlledMovement(new CompositeMovement(new AcceleratedMovement(0, 0, this.Acceleration, baseSpeed), movements), "movement");
        this.AddAction(new ItemAction("faceCursor", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        SetAngle = false;
        IsInLevel = true;

    }

    public void UseEBonus()
    {
        UseBonus(EBonus, ToolsUI.wrapPanel.eBonusSlot);
    }
    public void UseQBonus()
    {
        UseBonus(QBonus, ToolsUI.wrapPanel.qBonusSlot);
    }

    private void UseBonus(Edible bonus, LoadingSlotTemplate lst)
    {
        if (lst.SlotReady && bonus != null)
        {
            bonus.OnTrigger();
            lst.Time = bonus.CoolDown;
            lst.StartCountDown();
            lst.RemoveSlotable(false);
        }
    }

    protected override void SetupItem()
    {
        base.SetupItem();
        setActiveOnLoad = true;
    }
    public override void OnCollisionEnter(Item collider)
    {
        if (!GCon.GameStarted)
            return;
        base.OnCollisionEnter(collider);
        if (collider != null)
        {
            if (collider is Exit e)
            {
                Vector2 pos = new Vector2(0, 0);

                foreach (var exits in GCon.game.CurBiom.levels[e.LevelId].ExitsAr)
                {
                    foreach (var exit in exits)
                    {
                        if (exit.ExitId == e.ExitId)
                        {
                            pos = (new Vector2(exit.X, exit.Y));
                            break;
                        }
                    }
                }
                ToolsPhyward.EnterLevel(GCon.game.CurBiom.levels[e.LevelId], pos);
            }
            if (collider is Enemy en)
            {
                this.AddAction(new ItemAction("receiveDamage", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.Delete, null, en.BodyDamage), "receiveDamage" + en.Id);
            }
            if (collider is Collectable col)
            {
                if (PlayerControl.AutoPickup)
                {
                    PlayerControl.PickupCollectable(col);
                    if (col.SlotableRef.PrefabName == CraftedWeapons.Blowgun().PrefabName && GCon.game.TutorialPhase == 0)
                    {
                        GCon.game.TutorialPhase = 1;
                    }
                    if (col.SlotableRef.PrefabName == Units.time.PrefabName && GCon.game.TutorialPhase == 3)
                    {
                        Tutorial.ShowTutorial(new Tutorial.TutorialBlock("Nález veličiny", "Poražený nepřítel se proměnil v Čas... bude se hodit na výrobu silnějších zbraní.", "Do inventáře se ti přidala veličina, která bude užitečná při vyrábění. Otevři inventář a najeď na čas, aby ses dočetl více."));
                        GCon.game.TutorialPhase = 4;
                    }
                }
            }
        }
    }


    public override void OnCollisionLeave(Item collider)
    {
        base.OnCollisionLeave(collider);
        if (collider != null)
        {
            if (collider is Enemy en)
            {
                this.DeleteAction("receiveDamage" + en.Id);
            }
        }
    }
    public override bool Death()
    {
        if (GCon.GetPausedType() == ToolsSystem.PauseType.InGame)
        {
            GCon.game.DeathCount++;
            ToolsUI.TriggerDeathPanel();
            GCon.game.Coef *= 0.9f;
            /*List<Slotable> temp = new List<Slotable>(PlayerControl.backpack);
            foreach (var item in temp)
            {
                if (item is PreUnit || item is Edible)
                {
                    PlayerControl.RemoveFromBackpack(item);
                }
            }*/
            foreach (var level in GCon.game.CurBiom.levels)
            {
                foreach (var item in level.Items.Values)
                {
                    if (item is ILived il)
                    {
                        il.LivedHandler.ChangeLives(il.LivedHandler.MaxLives, false);
                    }
                    if (item is Enemy e && e.IsMinion)
                    {
                        e.Dispose();
                    }
                }
            }
            ToolsSystem.ReleaseAllKeys();
            this.LivedHandler.ChangeLives(LivedHandler.MaxLives, false);
        }
        return true;
    }
    public override void UpdateHealthBar()
    {
        if (LivedHandler.MaxLives != 0)
        {
            damagebar.line.EditPoint(1, new Vector3(250 - 500 * (LivedHandler.MaxLives - LivedHandler.Lives) / LivedHandler.MaxLives, 0, 0), 25);
        }
    }
    public override void AddHealthBar()
    {
        var bar = GameObject.FindGameObjectWithTag("UIHealthBar");

        damagebar = bar.transform.GetChild(1).GetComponent<geniikw.DataRenderer2D.UILine>();
    }

}

