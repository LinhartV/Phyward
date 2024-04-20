using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IIdleMovementAI
{
    public void StartIdleMovement(Movable item);
    public void StopIdleMovement(Movable item);
}

