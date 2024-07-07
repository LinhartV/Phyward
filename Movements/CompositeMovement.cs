
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Movement composed of multiple movements with preset Speed. Speeds of partial movements will determine the direction
/// </summary>
public class CompositeMovement : IMovement
{
    [JsonProperty]
    private IMovement thisMovement;
    [JsonProperty]
    public Dictionary<string, IMovement> partialMovements = new Dictionary<string, IMovement>();
    /// <summary>
    /// Movement composed of multiple movements with preset Speed. Speeds of partial movements will determine the direction
    /// </summary>
    /// <param name="thisMovement">Movement, that will determine the Speed (partial movements are just for direction)</param>
    /// <param name="movements">Partial movements which will set the direction of the final movement</param>
    public CompositeMovement(IMovement thisMovement, Dictionary<string, IMovement> movements) : base(0, 0, false)
    {
        partialMovements = movements;
        this.thisMovement = thisMovement;
    }
    public CompositeMovement() : base() { }

    public override bool Frame(float friction)
    {
        base.Frame(friction);
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
            y += xy.Item2;
        }
        if (y != 0 && x != 0)
        {
            var del = Math.Sqrt(((x * x) / (y * y)) + 1);
            float yout = (float)(thisMovement.MovementSpeed / del);
            return (yout * x / Math.Abs(y), yout * Math.Abs(y) / y);
        }
        else if (x == 0 && y != 0)
        {
            return (0, thisMovement.MovementSpeed * Math.Abs(y) / y);
        }
        else if (x != 0 && y == 0)
        {
            return (thisMovement.MovementSpeed * Math.Abs(x) / x, 0);
        }
        else
        {
            return (0, 0);
        }
    }

    public override void ResetMovementAngle(float angle)
    {
    }
    public override void UpdateMovement()
    {
        if (!isUpdated)
        {
            thisMovement.UpdateMovement();
            isUpdated = true;
        }
    }
    public void UpdateCompositeMovement(string name)
    {
        if (partialMovements.ContainsKey(name))
        {
            partialMovements[name].UpdateMovement();
        }
    }
    public override void ResetMovementSpeed(float speed)
    {
        base.ResetMovementSpeed(speed);
        thisMovement.ResetMovementSpeed(speed);
        foreach (var movement in partialMovements.Values)
        {
            movement.ResetMovementSpeed(speed);
        }
    }
}

