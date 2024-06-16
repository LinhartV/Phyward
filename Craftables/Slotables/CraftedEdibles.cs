using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CraftedEdibles
{
    public static Edible Medkit()
    {
        return new Edible("Lékárnička", "Střelná rána? Nevadí, obvážeme", "Během 5 sekund vyléčí 10 životů... trvá to hold dlouho", 5, 10, () =>
        {
            string name = GCon.game.Player.AddAction(new ItemAction("heal", 1, ItemAction.ExecutionType.EveryTime, ItemAction.OnLeaveType.KeepRunning, null, 5 * GCon.frameTime), 0, ActionHandler.RewriteEnum.AddNew);
            GCon.game.Player.AddAction(new ItemAction("stopBonus", ToolsMath.SecondsToFrames(5), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning, null, name), 0, ActionHandler.RewriteEnum.AddNew);
        }, 0, GameObjects.medkit, (Units.time, 1));
    }

    public static List<Edible> craftedEdibles = new List<Edible>() { Medkit() };
}

