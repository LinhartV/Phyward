
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/// <summary>
/// Movement composed of multiple movements with preset speed. Speeds of partial movements will determine the direction
/// </summary>
public class CompositeMovement : IMovement
{
    [JsonProperty]
    private IMovement thisMovement;
    [JsonProperty]
    private Dictionary<string, IMovement> partialMovements = new Dictionary<string, IMovement>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="thisMovement">Movement, that will determine the speed (partial movements are just for direction)</param>
    /// <param name="movements">Partial movements which will set the direction of the final movement</param>
    public CompositeMovement(IMovement thisMovement, Dictionary<string, IMovement> movements) : base(0, 0)
    {
        partialMovements = movements;
        this.thisMovement = thisMovement;
    }
    public CompositeMovement() : base() { }

    public override bool Frame(double friction)
    {
        foreach (IMovement movement in partialMovements.Values)
        {
            movement.Frame(friction);
        }
        return thisMovement.Frame(friction);
    }

    public override (float, float) Move()
    {

        float x = 0;
        float y = 0;
        (float, float) xy;
        foreach (var movement in partialMovements.Values)
        {
            xy = movement.Move();
            x += xy.Item1;
            y -= xy.Item2;
        }
        float yout = (float)(thisMovement.MovementSpeed / Math.Sqrt((x * x / y * y) + 1));
        return (yout * x / y, yout);
    }

    public override void ResetMovementAngle(double angle)
    {
    }
    public override void ResetMovementSpeed(double speed)
    {
    }
    public override void UpdateMovement()
    {
        thisMovement.UpdateMovement();
    }
    public void UpdateCompositeMovement(string name)
    {
        if (partialMovements.ContainsKey(name))
        {
            partialMovements[name].UpdateMovement();
        }
    }
}

