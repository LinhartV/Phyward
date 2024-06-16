using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;
using Newtonsoft.Json;

public abstract class Movable : Item, IPausable
{
    /// <summary>
    /// Whether to set the angle of the item in the direction of a travel
    /// </summary>
    public bool SetAngle { get; set; } = true;
    [JsonProperty]
    private float baseSpeed;
    /// <summary>
    /// Overall Speed of an item
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
    //Movements once set and controlled just by the movement itself. It deletes itself when Speed is 0. (e.g. shot Speed).
    [JsonProperty]
    public List<IMovement> MovementsAutomated { get; set; } = new List<IMovement>();
    //Movements controlled by other actions, such as player control. Accesed by id and not deleted even when Speed is 0.
    [JsonProperty]
    public Dictionary<string, IMovement> MovementsControlled { get; set; } = new Dictionary<string, IMovement>();
    [JsonProperty]
    private float xVelocity;
    [JsonProperty]
    private float yVelocity;
    [JsonIgnore]
    private Vector2 prevVelocity;
    public Movable() { }
    public Movable(Vector2 pos, float baseSpeed, float acceleration, float friction, GameObject prefab, bool isSolid = true, Tilemap map = null) : base(pos, prefab, isSolid, map)
    {
        this.BaseSpeed = baseSpeed;
        this.acceleration = acceleration;
        this.friction = friction;
        this.AddAction(new ItemAction("faceInDirection", 1, ItemAction.ExecutionType.EveryTime));
    }

    protected Movable(float baseSpeed, float acceleration, float friction, GameObject prefab, bool isSolid = false) : base(prefab, isSolid)
    {
        this.BaseSpeed = baseSpeed;
        this.acceleration = acceleration;
        this.friction = friction;
        this.AddAction(new ItemAction("faceInDirection", 1, ItemAction.ExecutionType.EveryTime));
    }

    public void TriggerPause(ToolsSystem.PauseType type)
    {
        if (pauseTypes.Contains(type))
        {
            rb.velocity = prevVelocity;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
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
    /// <summary>
    /// To communicate with unity rigidbody Speed system (don't use elsewhere)
    /// </summary>
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
            if (MovementsAutomated.Contains(movement) && movement.KeepUpdated)
            {
                movement.UpdateMovement();
            }
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
    /// Gets an angle where the item is moving
    /// </summary>
    public float GetMovementAngle()
    {
        return ToolsMath.GetAngleFromLengts(prevVelocity.x, prevVelocity.y);
    }
    /// <summary>
    /// Gets a magnitude of movement vector
    /// </summary>
    public float GetMovementSpeed()
    {
        return prevVelocity.magnitude;
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
                MovementsControlled[movementName].ResetMovementAngle(MovementsControlled[movementName].Angle - angleRotation);
            else
                MovementsControlled[movementName].ResetMovementAngle(angleRotation);
        }
    }

    public void StopAll()
    {
        foreach (var movement in MovementsAutomated)
        {
            movement.SuddenStop();
        }
        foreach (var movement in MovementsAutomated)
        {
            movement.SuddenStop();
        }
    }
}
