
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RealAcceleratedMovement : IMovement
{
    [JsonProperty]
    public float Acceleration { get; set; }
    [JsonProperty]
    private float frictionCoef;
    /// <summary>
    /// which way I'm accelerating right now (angle tells real angle of movement)
    /// </summary>
    [JsonProperty]
    private float wantedAngle;
    [JsonProperty]
    private float xMovement;
    [JsonProperty]
    private float yMovement;
    /// <summary>
    /// Movement constantly slowing by friction but accelerates when updated - rotation means acceleration in new direction (not simply rewriting angle of this movement)
    /// </summary>
    public RealAcceleratedMovement(float initialSpeed, float angle, float acceleration, float maxSpeed, float frictionCoef = 1, bool keepUpdated = false) : base(initialSpeed, angle, keepUpdated)
    {
        this.Acceleration = acceleration;
        this.baseSpeed = maxSpeed;
        this.frictionCoef = frictionCoef;
        (xMovement, yMovement) = ToolsMath.PolarToCartesian(angle, initialSpeed);
    }
    /// <summary>
    /// Only slowing movement
    /// </summary>
    public RealAcceleratedMovement(float initialSpeed, float angle, float frictionCoef = 1, bool keepUpdated = false) : base(initialSpeed, angle, keepUpdated)
    {
        this.Acceleration = 0;
        this.baseSpeed = initialSpeed;
        this.frictionCoef = frictionCoef;
        (xMovement, yMovement) = ToolsMath.PolarToCartesian(angle, initialSpeed);
    }
    public RealAcceleratedMovement() : base() { }

    public override bool Frame(float friction)
    {
        base.Frame(friction);
        bool stopped;
        if (this.MovementSpeed > 0.0005)
        {
            MovementSpeed = (float)Math.Sqrt(xMovement * xMovement + yMovement * yMovement);
            MovementSpeed -= friction * frictionCoef * GCon.percentageOfFrame;
            (xMovement, yMovement) = ToolsMath.PolarToCartesian(Angle, MovementSpeed);

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
        }/*
        float deltax, prex;
        float deltay, prey;
        (prex, prey) = ToolsMath.PolarToCartesian(Angle, MovementSpeed);
        (deltax, deltay) = ToolsMath.PolarToCartesian(preangle, Acceleration * GCon.percentageOfFrame);
        prex += deltax;
        prey += deltay;
        Angle = ToolsMath.CartesianToPolar(prex, prey).Item1;*/
        return stopped;
    }

    public override (float, float) Move()
    {
        return ToolsMath.PolarToCartesian(Angle, MovementSpeed);
    }

    public override void ResetMovementAngle(float angle)
    {
        wantedAngle = angle;
    }

    public override void UpdateMovement()
    {
        if (!isUpdated)
        {
            float deltax, deltay;
            isUpdated = true;
            (xMovement, yMovement) = ToolsMath.PolarToCartesian(Angle, MovementSpeed);
            (deltax, deltay) = ToolsMath.PolarToCartesian(wantedAngle, Acceleration * GCon.percentageOfFrame);
            xMovement += deltax;
            yMovement += deltay;
            MovementSpeed = (float)Math.Sqrt(xMovement * xMovement + yMovement * yMovement);
            if (MovementSpeed > baseSpeed)
            {
                MovementSpeed = baseSpeed;
            }
            Angle = ToolsMath.CartesianToPolar(xMovement,yMovement).Item1;
        }
    }
    /// <summary>
    /// Sets current movement speed and if it's higher than max speed, it updates max speed as well
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

