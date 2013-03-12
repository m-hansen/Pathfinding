using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame
{
    public class MovingEntity : BaseGameEntity
    {
        public Vector2 Velocity;     // vector for objects velocity
        public Vector2 Heading;      // extends out from the front of the object
        public Vector2 Side;         // perpendicular to the heading vector
        public float Mass;          // mass of the object
        public float MaxSpeed;      // top speed the object can reach
        public float MaxForce;      // the maximum force of the object
        public float MaxTurnRate;   // caps off the rate an object can rotate

        public MovingEntity()
        {
            Heading = new Vector2(0, -1);   // initialize to point north
        }

        public virtual void Update(GameTime gametime, Vector2 targetPosition)
        {
            // update the heading
            RotateHeading(targetPosition);
        }

        // Used to set the heading to the front of each moving entity
        private bool RotateHeading(Vector2 target)
        {
            Vector2 targetPos = Vector2.Normalize(target - Position);       // the target based on target - pos
            double theta = Math.Acos(Vector2.Dot(Heading, targetPos));      // the angle

            // check if facing target
            if (theta < 0.00001) // NOTE: small value is used in place of zero
                return true;

            // make sure theta doesn't exceed the max turn rate
            if (theta > MaxTurnRate)
                theta = MaxTurnRate;

            

            return false;
        }

    }
}
