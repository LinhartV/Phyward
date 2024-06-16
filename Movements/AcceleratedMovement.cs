
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AcceleratedMovement : IMovement
{
    [JsonProperty]
    public float Acceleration { get; set; }
    [JsonProperty]
    private float frictionCoef;
    /// <summary>
    /// Movement constantly slowing by friction but accelerates when updated - rotation sets new angle (ignoring the fact that it should accelerate to that angle - it rotates instantly)
    /// </summary>
    public AcceleratedMovement(float initialSpeed, float angle, float acceleration, float maxSpeed, float frictionCoef = 1, bool keepUpdated = false) : base(initialSpeed, angle, keepUpdated)
    {
        this.Acceleration = acceleration;
        this.baseSpeed = maxSpeed;
        this.frictionCoef = frictionCoef;
    }
    /// <summary>
    /// Only slowing movement
    /// </summary>
    public AcceleratedMovement(float initialSpeed, float angle, float frictionCoef = 1, bool keepUpdated = false) : base(initialSpeed, angle, keepUpdated)
    {
        this.Acceleration = 0;
        this.baseSpeed = initialSpeed;
        this.frictionCoef = frictionCoef;
    }
    public AcceleratedMovement() : base() { }

    public override bool Frame(float friction)
    {
        base.Frame(friction);
        bool stopped;
        if (this.MovementSpeed > Constants.MIN_VALUE)
        {
            MovementSpeed -= friction * frictionCoef * GCon.frameTime;
            if (MovementSpeed < 0)
            {
                MovementSpeed = 0;
            }
            stopped = false;
        }
        else
        {
            MovementSpeed = 0;
            stopped = true;
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
        if (!isUpdated)
        {
            isUpdated = true;
            if (MovementSpeed < baseSpeed)
            {
                MovementSpeed += Acceleration * GCon.frameTime;
            }
            else
            {
                MovementSpeed = baseSpeed;
            }
        }
    }
    /// <summary>
    /// Sets current movement Speed and if it's higher than max Speed, it updates max Speed as well
    /// </summary>
    public override void ResetMovementSpeed(float speed)
    {
        MovementSpeed = speed;
        if (baseSpeed < speed)
        {
            baseSpeed = speed;
        }
    }

}

