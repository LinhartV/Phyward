using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class ToAndFroIdleMovement : IIdleMovementAI
{
    public void StartIdleMovement(Movable item)
    {
        item.AddControlledMovement(new AcceleratedMovement(0, ToolsGame.Rng(),0.8f), "randomMovement");
        item.AddAction(new ItemAction("randomWalk", 1, ItemAction.ExecutionType.EveryTime));

    }

    public void StopIdleMovement(Movable item)
    {
        item.MovementsControlled["randomMovement"].SuddenStop();
        item.DeleteAction("randomWalk");
    }
}

