
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ConstantMovement : IMovement
{
    [JsonProperty]
    private int stopMovement = 0;
    public ConstantMovement(float movementSpeed, float angle) : base(movementSpeed, angle)
    {
        baseSpeed = movementSpeed;
    }
    public ConstantMovement() : base() { }

    public override bool Frame(float friction)
    {
        bool stopped;
        stopMovement++;
        if (stopMovement > 1)
        {
            MovementSpeed = 0;
            stopped = true;
        }
        else
        {
            MovementSpeed = baseSpeed;
            stopped = false;
        }
        return stopped;
    }


    public override (float, float) Move()
    {
        return ToolsMath.PolarToCartesian(Angle, MovementSpeed);

    }

    public override void ResetMovementAngle(float angle)
    {
        this.Angle = angle;
    }
    public override void UpdateMovement()
    {
        stopMovement = 0;
    }
}

