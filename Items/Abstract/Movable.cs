using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;
using Newtonsoft.Json;

public abstract class Movable : Item
{
    /// <summary>
    /// Whether to set the angle of the item in the direction of a travel
    /// </summary>
    public bool SetAngle { get; set; } = true;
    [JsonProperty]
    private float baseSpeed;
    /// <summary>
    /// Overall speed of an item
    /// </summary>
    public float BaseSpeed
    {
        get => baseSpeed;
        set
        {
            baseSpeed = value;
            foreach (var movement in MovementsControlled.Values)
            {
                movement.ResetMovementSpeed(value);
            }
        }
    }
    [JsonProperty]
    private float friction;
    public float Friction
    {
        get => friction;
        set
        {
            friction = Math.Abs(value);
        }
    }
    private float acceleration;
    public float Acceleration
    {
        get => acceleration;
        set
        {
            acceleration = value;
            foreach (var movement in MovementsControlled.Values)
            {
                if (movement is AcceleratedMovement am)
                    am.Acceleration = value;
                if (movement is CompositeMovement cm)
                {
                    foreach (var pm in cm.partialMovements.Values)
                    {
                        if (pm is AcceleratedMovement a)
                        {
                            a.Acceleration = value;
                        }
                    }
                }
            }
        }
    }
    //Movements once set and controlled just by the movement itself. It deletes itself when speed is 0. (e.g. shot speed)
    [JsonProperty]
    public List<IMovement> MovementsAutomated { get; set; } = new List<IMovement>();
    //Movements controlled by other actions, such as player control. Accesed by id and not deleted even when speed is 0.
    [JsonProperty]
    public Dictionary<string, IMovement> MovementsControlled { get; set; } = new Dictionary<string, IMovement>();
    [JsonProperty]
    private float xVelocity;
    [JsonProperty]
    private float yVelocity;
    [JsonIgnore]
    private Vector2 prevVelocity;
    public Movable() { }
    public Movable((float, float) pos, float baseSpeed, float acceleration, float friction, GameObject prefab, bool isSolid = true, Tilemap map = null) : base(pos, prefab, isSolid, map)
    {
        this.BaseSpeed = baseSpeed;
        this.acceleration = acceleration;
        this.friction = friction;
    }
    public override void SaveItem()
    {
        xVelocity = rb.velocity.x;
        yVelocity = rb.velocity.y;
        base.SaveItem();
    }
    protected override void SetupItem()
    {
        base.SetupItem();
        this.rb.velocity = new Vector2(xVelocity, yVelocity);
    }
    public void CorrectSpeed()
    {
        List<IMovement> allMovements = new List<IMovement>(MovementsAutomated);
        allMovements.AddRange(MovementsControlled.Values);
        if (prevVelocity != rb.velocity)
        {
            foreach (var movement in allMovements)
            {
                movement.SetSpeedAccordingToPrefabVelocity(rb);
            }
        }
    }
    /// <summary>
    /// Move the current object based on it's movements
    /// </summary>
    public void Move()
    {
        List<IMovement> allMovements = new List<IMovement>(MovementsAutomated);
        allMovements.AddRange(MovementsControlled.Values);
        
        (float, float) xy;
        float x = 0;
        float y = 0;
        foreach (var movement in allMovements)
        {
            if (movement.Frame(friction) && MovementsAutomated.Contains(movement))
            {
                MovementsAutomated.Remove(movement);
            }
            xy = movement.Move(); //ToolsMath.PolarToCartesian(movement.Angle, movement.MovementSpeed);
            x += xy.Item1;
            y += xy.Item2;
        }
        rb.velocity = (new Vector2(x, y));
        //this.X += x;
        //this.Y += y;
        if (SetAngle)
        {
            Angle = ToolsMath.GetAngleFromLengts(x, y);
        }
        prevVelocity = rb.velocity;
    }
    /// <summary>
    /// Creates new movement which is controlled only by movement itself (deletes itself when 0)
    /// </summary>
    public void AddAutomatedMovement(IMovement movement)
    {
        MovementsAutomated.Add(movement);
    }
    /// <summary>
    /// Cretes new movement which can be controlled by other events (key press etc.)
    /// </summary>
    public void AddControlledMovement(IMovement movement, string movementName)
    {
        if (!MovementsControlled.ContainsKey(movementName))
        {
            MovementsControlled.Add(movementName, movement);
        }
    }
    /// <summary>
    /// Calls UpdateMovement of particular movement
    /// </summary>
    public void UpdateControlledMovement(string movementName)
    {
        if (MovementsControlled.ContainsKey(movementName))
        {
            MovementsControlled[movementName].UpdateMovement();
        }
    }
    /// <summary>
    /// Calls UpdateMovement of particular movement
    /// </summary>
    public void UpdateCompositeMovement(string movementName, string partialMovementName)
    {
        if (MovementsControlled.ContainsKey(movementName) && MovementsControlled[movementName] is CompositeMovement)
        {
            (MovementsControlled[movementName] as CompositeMovement).UpdateCompositeMovement(partialMovementName);
        }
        MovementsControlled[movementName].UpdateMovement();
    }
    /// <summary>
    /// Rotates particular movement or sets its angle
    /// </summary>
    /// <param name="movementName">Name of the movement to rotate</param>
    /// <param name="angleRotation">Angle to set or rotate by</param>
    /// <param name="rotate">Set true for relative rotation, set false to set the angle</param>
    public void RotateControlledMovement(string movementName, float angleRotation, bool rotate = true)
    {
        if (MovementsControlled.ContainsKey(movementName))
        {
            if (rotate)
                MovementsControlled[movementName].Angle -= angleRotation;
            else
                MovementsControlled[movementName].ResetMovementAngle(angleRotation);
        }
    }
}
