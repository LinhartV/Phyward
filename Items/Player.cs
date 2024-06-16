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

    public Player(Vector2 pos, float baseSpeed, float acceleration, float friction, IWeapon weapon, float charDamage, float charReloadTime, float charShotSpeed, float charShotDuration, float lives, Tilemap map = null) : base(pos, baseSpeed, acceleration, friction, weapon, charDamage, charReloadTime, charShotSpeed, charShotDuration, lives, GameObjects.player, true, true, map)
    {/*
        this.AddControlledMovement(new AcceleratedMovement(0, 0, this.Acceleration, BaseSpeed), "up");
        this.AddControlledMovement(new AcceleratedMovement(0, (float)Math.PI / 2, this.Acceleration, BaseSpeed), "right");
        this.AddControlledMovement(new AcceleratedMovement(0, (float)Math.PI, this.Acceleration, BaseSpeed), "down");
        this.AddControlledMovement(new AcceleratedMovement(0, (float)(3 * Math.PI / 2), this.Acceleration, BaseSpeed), "left");
        this.AddAction(new ItemAction("faceCursor", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        SetAngle = false;*/
        Dictionary<string, IMovement> movements = new Dictionary<string, IMovement>();
        movements.Add("up", new AcceleratedMovement(0, 0, this.Acceleration, BaseSpeed));
        movements.Add("right", new AcceleratedMovement(0, (float)Math.PI / 2, this.Acceleration, BaseSpeed));
        movements.Add("down", new AcceleratedMovement(0, (float)Math.PI, this.Acceleration, BaseSpeed));
        movements.Add("left", new AcceleratedMovement(0, (float)Math.PI * 3 / 2, this.Acceleration, BaseSpeed));
        AddControlledMovement(new CompositeMovement(new AcceleratedMovement(0, 0, this.Acceleration, baseSpeed), movements), "movement");
        /*Dictionary<string, IMovement> movements = new Dictionary<string, IMovement>();
        movements.Add("up", new ConstantMovement(BaseSpeed, 0));
        movements.Add("right", new ConstantMovement(BaseSpeed, (float)Math.PI / 2));
        movements.Add("down", new ConstantMovement(BaseSpeed, (float)Math.PI));
        movements.Add("left", new ConstantMovement(BaseSpeed, (float)Math.PI * 3 / 2));
        AddControlledMovement(new CompositeMovement(new ConstantMovement(BaseSpeed, 0), movements), "movement");*/
        this.AddAction(new ItemAction("faceCursor", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning));
        SetAngle = false;
        IsInLevel = true;
        for (int i = 0; i < 5; i++)
        {
            var time = new Unit(Units.Time());
            time.InsertAtPosition(this.Prefab.transform.position + new Vector3(0, 0), true);
            GCon.game.CurLevel.AddItem(time);
        }
        for (int i = 0; i < 5; i++)
        {
            var time = new Collectable(CraftedEdibles.Medkit());
            time.InsertAtPosition(this.Prefab.transform.position + new Vector3(0, 0), true);
            GCon.game.CurLevel.AddItem(time);
        }
        /*var scroll = new Collectable(new Scroll(Units.volume));
        scroll.InsertAtPosition(this.Prefab.transform.position + new Vector3(3f, 0), true);
        GCon.game.CurLevel.AddItem(scroll);

        var scroll2 = new Collectable(new Scroll(Units.frequency));
        scroll2.InsertAtPosition(this.Prefab.transform.position + new Vector3(2f, 0), true);
        GCon.game.CurLevel.AddItem(scroll2);
        var scroll3 = new Collectable(new Scroll(Units.area));
        scroll3.InsertAtPosition(this.Prefab.transform.position + new Vector3(0, 2), true);
        GCon.game.CurLevel.AddItem(scroll3);*/

        PlayerControl.DiscoverNewUnit(Units.volume);
        PlayerControl.DiscoverNewUnit(Units.area);
        PlayerControl.DiscoverNewUnit(Units.frequency);
        PlayerControl.DiscoverNewUnit(Units.mass);
        PlayerControl.DiscoverNewUnit(Units.time);
        PlayerControl.DiscoverNewUnit(Units.length);
        PlayerControl.DiscoverNewUnit(Units.speed);

        GCon.game.CurLevel.AddItem(new Base(this.Prefab.transform.position));
        PlayerControl.backpack.Add(CraftedWeapons.Sling());
        PlayerControl.backpack.Add(CraftedWeapons.SlingShot());
        PlayerControl.backpack.Add(CraftedWeapons.Blowgun());
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

        //Prefab.GetComponent<ExitScript>().X = exitX;
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
                var lvl = GCon.game.CurBiom.levels[e.LevelId];
                Vector2 pos = new Vector2(0, 0);

                GCon.freezeCamera = true;
                GCon.game.gameActionHandler.AddAction(new ItemAction("unfreeze", 1, ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning));
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
                GameObject.Find("UnityControl").GetComponent<UnityControl>().BuildLevel(lvl, GCon.game.CurLevel, pos);
                GCon.game.CurLevel.OnLeave();
                GCon.game.CurLevel = lvl;
                lvl.OnEnter();
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
    public override void Death()
    {
        Debug.Log("You died");
    }
    public override void UpdateHealthBar()
    {
        if (MaxLives != 0)
        {
            damagebar.line.EditPoint(1, new Vector3(250 - 500 * (MaxLives - Lives) / MaxLives, 0, 0), 25);
        }
    }
    public override void AddHealthBar()
    {
        var bar = GameObject.FindGameObjectWithTag("UIHealthBar");

        damagebar = bar.transform.GetChild(1).GetComponent<geniikw.DataRenderer2D.UILine>();
    }

}

