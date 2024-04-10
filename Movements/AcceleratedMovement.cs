
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
    /// <summary>
    /// Movement constantly slowing by friction but accelerates when updated
    /// </summary>
    public AcceleratedMovement(float initialSpeed, float angle, float acceleration, float maxSpeed) : base(initialSpeed, angle)
    {
        this.Acceleration = acceleration;
        this.baseSpeed = maxSpeed;
    }
    /// <summary>
    /// Only slowing movement
    /// </summary>
    public AcceleratedMovement(float initialSpeed, float angle) : base(initialSpeed, angle)
    {
        this.Acceleration = 0;
        this.baseSpeed = initialSpeed;
    }
    public AcceleratedMovement() : base() { }

    public override bool Frame(float friction)
    {
        bool stopped;
        if (this.MovementSpeed > 0.0005)
        {
            MovementSpeed -= friction * GCon.percentageOfFrame;
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
        if (MovementSpeed < baseSpeed)
        {
            MovementSpeed += Acceleration * GCon.percentageOfFrame;
        }
        else
        {
            MovementSpeed = baseSpeed;
        }
    }
}

