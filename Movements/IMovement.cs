
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Obsolete, don't use
/// </summary>
public abstract class IMovement
{
    public IMovement() { }
    public IMovement(float movementSpeed, float angle, bool keepUpdated)
    {
        this.Angle = angle;
        this.MovementSpeed = movementSpeed;
        KeepUpdated = keepUpdated;
    }
    /// <summary>
    /// Speed the item is moving right now
    /// </summary>
    public float MovementSpeed { get; protected set; }

    /// <summary>
    /// Whether this movement - when automated - should be updated every frame.
    /// </summary>
    public bool KeepUpdated { get; protected set; }
    /// <summary>
    /// Speed property of the movement
    /// </summary>
    [JsonProperty]
    protected float baseSpeed;
    /// <summary>
    /// if it's updated this frame
    /// </summary>
    [JsonProperty]
    protected bool isUpdated;
    [JsonProperty]
    private float angle;
    public float Angle
    {
        get => angle;
        protected set
        {
            angle = value % (float)(Math.PI * 2);
        }
    }
    public abstract (float, float) Move();
    /// <summary>
    /// what will happen every frame
    /// </summary>
    /// <returns>If the movement is 0</returns>
    public virtual bool Frame(float friction)
    {
        isUpdated = false;
        return false;
    }
    public void SetSpeedAccordingToPrefabVelocity(Rigidbody2D rb)
    {
        // Set maximum speed to prefab velocity

        //my speed
        float x, y;
        (x, y) = ToolsMath.PolarToCartesian(angle, MovementSpeed);
        if (x > 0)
        {
            if (rb.velocity.x < x)
            {
                x = rb.velocity.x;
            }
        }
        else if (x < 0)
        {
            if (rb.velocity.x > x)
            {
                x = rb.velocity.x;
            }
        }
        if (y > 0)
        {
            if (rb.velocity.y < y)
            {
                y = rb.velocity.y;
            }
        }
        else if (y < 0)
        {
            if (rb.velocity.y > y)
            {
                y = rb.velocity.y;
            }
        }
        MovementSpeed = ToolsMath.CartesianToPolar(x, y).Item2;
    }
    //change properties of this movement
    public abstract void ResetMovementAngle(float angle);
    public virtual void ResetMovementSpeed(float speed)
    {
        baseSpeed = speed;
    }
    /// <summary>
    /// proceed action of this movement on call (eg. player keeps on pressing arrow up)
    /// </summary>
    public abstract void UpdateMovement();
    /// <summary>
    /// Entirely stops the movement
    /// </summary>
    public virtual void SuddenStop()
    {
        this.MovementSpeed = 0;
    }
}

