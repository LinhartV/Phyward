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
    // Angle where the character is "looking" (for picture, shooting and stuff)
    private double angle;
    public double Angle
    {
        get => angle;
        set
        {
            angle = value % (Math.PI * 2);
        }
    }
    [JsonProperty]
    private double baseSpeed;
    /// <summary>
    /// Overall speed of an item
    /// </summary>
    public double BaseSpeed
    {
        get => baseSpeed; set
        {
            baseSpeed = value;
            foreach (var movement in MovementsControlled.Values)
            {
                movement.ResetMovementSpeed(value);
            }
        }
    }
    [JsonProperty]
    private double friction;
    public double Friction { get => friction; set => friction = Math.Abs(value); }
    private double acceleration;
    public double Acceleration
    {
        get => acceleration;
        set
        {
            acceleration = value;
            foreach (var movement in MovementsControlled.Values)
            {
                if (movement is AcceleratedMovement am)
                    am.Acceleration = value;
            }
        }
    }
    //Movements once set and controlled just by the movement itself. It deletes itself when speed is 0. (e.g. explosion)
    [JsonProperty]
    public List<IMovement> MovementsAutomated { get; set; } = new List<IMovement>();
    //Movements controlled by other actions, such as player control. Accesed by id and not deleted even when speed is 0.
    [JsonProperty]
    public Dictionary<string, IMovement> MovementsControlled { get; set; } = new Dictionary<string, IMovement>();
    public Movable() { }
    public Movable((float, float) pos, float baseSpeed, double acceleration, double friction, GameObject prefab, Tilemap map = null) : base(pos, prefab, map)
    {
        this.BaseSpeed = baseSpeed;
        this.acceleration = acceleration;
        this.friction = friction;
        this.AddAction(new ItemAction("move", 1));
        GCon.game.ItemsStep.Add(this.Id, this);
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
            y -= xy.Item2;
        }
        Rigidbody2D rb = this.Prefab.GetComponent<Rigidbody2D>();
        rb.MovePosition(new Vector2(rb.position.x + x, rb.position.y + y));
        //this.X += x;
        //this.Y += y;
        if (SetAngle)
        {
            Angle = ToolsMath.GetAngleFromLengts(x, -y);
        }
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
    public void RotateControlledMovement(string movementName, double angleRotation, bool rotate = true)
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
