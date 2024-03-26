using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;
using Newtonsoft.Json;

public abstract class Movable : Item
{// Angle where the character is "looking" (for picture, shooting and stuff)
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
    public Movable((float, float) pos, float baseSpeed, Tilemap map, double acceleration, double friction, bool setAngle) : base(pos, map, new CircleCollider2D(), "")
    {
        this.BaseSpeed = baseSpeed;
        this.acceleration = acceleration;
        this.friction = friction;

        Rigidbody2D rb = Prefab.AddComponent<Rigidbody2D>();
        rb.simulated = true;

    }
    /// <summary>
    /// Move the current object based on it's movements
    /// </summary>
    /// <param name="percentage">Percentage of current frame</param>
    /// <param name="setAngle">Whether to set angle of object to match the movement angle</param>
    public void Move(double percentage, bool setAngle)
    {
        List<IMovement> allMovements = new List<IMovement>(MovementsAutomated);
        allMovements.AddRange(MovementsControlled.Values);
        (double, double) xy;
        double x = 0;
        double y = 0;
        foreach (var movement in allMovements)
        {
            if (movement.Frame(friction, percentage) && MovementsAutomated.Contains(movement))
            {
                MovementsAutomated.Remove(movement);
            }
            xy = movement.Move(percentage); //ToolsMath.PolarToCartesian(movement.Angle, movement.MovementSpeed);
            x += xy.Item1;
            y -= xy.Item2;
        }
        this.X += x;
        this.Y += y;
        if (setAngle)
        {
            Angle = ToolsMath.GetAngleFromLengts(x, -y);
        }
    }
}
