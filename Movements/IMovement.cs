﻿
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public abstract class IMovement
    {
        public IMovement(){}
        public IMovement(double movementSpeed, double angle)
        {
            this.Angle = angle;
            this.MovementSpeed = movementSpeed;
        }
        public double MovementSpeed { get; set; }
        [JsonProperty]
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
            }
        }
        public abstract (double, double) Move(double percentage);
        /// <summary>
        /// what will happen every frame
        /// </summary>
        /// <returns>If the movement is 0</returns>
        public abstract bool Frame(double friction, double percentage);
        //change properties of this movement
        public abstract void ResetMovementAngle(double angle);
        public abstract void ResetMovementSpeed(double speed);
        /// <summary>
        /// proceed action of this movement on call (eg. player keeps on pressing arrow up)
        /// </summary>
        public abstract void UpdateMovement(double percentage);
        public virtual void SuddenStop()
        {
            this.MovementSpeed = 0;
        }
    }

