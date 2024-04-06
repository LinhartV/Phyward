
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class AcceleratedMovement : IMovement
    {
        [JsonProperty]
        private double maxSpeed;
        [JsonProperty]
        public double Acceleration { get; set; }
        /// <summary>
        /// Movement constantly slowing by friction but accelerates when updated
        /// </summary>
        public AcceleratedMovement(double initialSpeed, double angle, double acceleration, double maxSpeed) : base(initialSpeed, angle)
        {
            this.Acceleration = acceleration;
            this.maxSpeed = maxSpeed;
        }
        /// <summary>
        /// Only slowing movement
        /// </summary>
        public AcceleratedMovement(double initialSpeed, double angle) : base(initialSpeed, angle)
        {
            this.Acceleration = 0;
            this.maxSpeed = initialSpeed;
        }
        public AcceleratedMovement():base(){}

        public override bool Frame(double friction)
        {
            if (this.MovementSpeed > 0.0005)
            {
                MovementSpeed -= friction * GCon.percentageOfFrame;
                return false;
            }
            else
            {
                MovementSpeed = 0;
                return true;
            }
        }

        public override (float, float) Move()
        {
            return ToolsMath.PolarToCartesian(Angle, MovementSpeed * GCon.percentageOfFrame);
        }

        public override void ResetMovementAngle(double angle)
        {
            this.Angle = angle;
        }
        public override void ResetMovementSpeed(double speed)
        {
            maxSpeed = speed;
        }

        public override void UpdateMovement()
        {
            if (MovementSpeed < maxSpeed)
            {
                MovementSpeed += Acceleration * GCon.percentageOfFrame;
            }
            else
            {
                MovementSpeed = maxSpeed;
            }
        }
    }

