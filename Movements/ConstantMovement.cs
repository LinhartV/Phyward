
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

    public class ConstantMovement : IMovement
    {
        //speed the item can be moving by
        [JsonProperty]
        private double baseSpeed = 0;
        [JsonProperty]
        private int stopMovement = 0;
        public ConstantMovement(double movementSpeed, double angle) : base(movementSpeed, angle)
        {
            baseSpeed = movementSpeed;
        }
        public ConstantMovement() : base(){}

        public override bool Frame(double friction)
        {
            stopMovement++;
            if (stopMovement > 1)
            {
                MovementSpeed = 0;
                return true;
            }
            else
            {
                MovementSpeed = baseSpeed;
                return false;
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
            this.baseSpeed = speed;
        }
        public override void UpdateMovement()
        {
            stopMovement = 0;
        }
    }

