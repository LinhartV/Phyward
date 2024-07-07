using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class CraftedEdibles
{
    public static Edible Medkit()
    {
        return new Edible("Lékárnička", "Střelná rána? Nevadí, dáme náplast.", "Během 5 sekund vyléčí 10 životů... trvá to hold dlouho", 5, 10, () =>
        {
            string name = GCon.game.Player.AddAction(new ItemAction("heal", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning, null, 5 * GCon.frameTime), 0, ActionHandler.RewriteEnum.AddNew);
            GCon.game.Player.AddAction(new ItemAction("stopBonus", ToolsMath.SecondsToFrames(5), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning, null, name), 0, ActionHandler.RewriteEnum.AddNew);
        }, 0, GameObjects.medkit, (Units.time, 1));
    }
    public static Edible AcceleratedHeal()
    {
        return new Edible("Zrychlené léčení", "O 'trošičku' účinnější alternativa k lékárničce", "Léčí se zrychlením - ze začátku pomalu, postupně nabyde na účinnosti.", 5, 20, () =>
        {
            string name = GCon.game.Player.AddAction(new ItemAction("acceleratedHeal", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning, null, 0f), 0, ActionHandler.RewriteEnum.AddNew);
            GCon.game.Player.AddAction(new ItemAction("stopBonus", ToolsMath.SecondsToFrames(10), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning, null, name), 0, ActionHandler.RewriteEnum.AddNew);
        }, 2, GameObjects.acceleratedHealing, (Units.acceleration, 2));
    }
    public static Edible Bandage()
    {
        return new Edible("Obvaz", "Nic jiného než dlooouhý kus látky", "Instantně vyléčí 20 životů", 7, 15, () =>
        {
            GCon.game.Player.LivedHandler.ChangeLives(20);
        }, 1, GameObjects.bandage, (Units.length, 2));
    }
    public static Edible FastReload()
    {
        return new Edible("Opakovačka", "RATATATATA", "Po dobu pěti sekund budeš střílet s dvojnásobnou frekvencí", 4, 20, () =>
        {
            GCon.game.Player.CharReloadTime /= 2;
            GCon.game.Player.AddAction(new ItemAction("stopFastReload", ToolsMath.SecondsToFrames(5), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning, null), 0, ActionHandler.RewriteEnum.AddNew);

        }, 2, GameObjects.fastReload, (Units.frequency, 2));
    }
    public static Edible SpeedIncrease()
    {
        return new Edible("Sprint", "Pokud máš pocit, že chodíš příliš pomalu...", "Na 10 sekund zvýší rychlost pohybu", 4, 15, () =>
        {
            GCon.game.Player.BaseSpeed *= 1.5f;
            GCon.game.Player.AddAction(new ItemAction("stopSpeedIncrease", ToolsMath.SecondsToFrames(10), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning, null), 0, ActionHandler.RewriteEnum.AddNew);

        }, 2, GameObjects.speedUp, (Units.speed, 2));
    }
    public static Edible Portal()
    {
        return new Edible("Portál", "Brána do jiné dimenze...", "Kdo ví, co tě tam čeká.", 1, 1, () =>
        {
            ToolsUI.TriggerTransitionPanel(() =>
            {
                ToolsPhyward.EnterLevel(GCon.game.CurBiom.levels[40]);
                GCon.game.Player.InsertAtPosition(new Vector2(3, 1), true);

            });
            string name = Portal().Name;
            var elem = craftedEdibles.FirstOrDefault(edible => edible.Name == name);
            if (elem != null)
            {
                craftedEdibles.Remove(elem);
            }
            var elem2 = AllCrafts.craftables.FirstOrDefault(edible => edible.Name == name);
            if (elem2 != null)
            {
                AllCrafts.craftables.Remove(elem2);
            }
            var elem3 = GCon.game.Player.PlayerControl.craftables.FirstOrDefault(edible => edible.Name == name);
            if (elem3 != null)
            {
                GCon.game.Player.PlayerControl.craftables.Remove(elem3);
            }
            var temp = new List<Slotable>(GCon.game.Player.PlayerControl.backpack);
            foreach (var item in temp)
            {
                if (item.Name == name)
                {
                    GCon.game.Player.PlayerControl.RemoveFromBackpack(item);
                }
            }
            var temp2 = new List<Slotable>(GCon.game.Player.PlayerControl.inBase);
            foreach (var item in temp2)
            {
                if (item.Name == name)
                {
                    GCon.game.Player.PlayerControl.RemoveFromBase(item);
                }
            }
            GCon.game.TutorialPhase = 10;

        }, 3, GameObjects.portal, (Units.force, 2), (Units.volume, 1));
    }

    public static List<Edible> craftedEdibles = new List<Edible>() { Medkit(), Bandage(), FastReload(), SpeedIncrease(), Portal(), AcceleratedHeal() };
}

