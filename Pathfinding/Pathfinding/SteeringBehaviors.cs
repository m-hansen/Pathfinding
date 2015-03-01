using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SampleGame
{
    public class SteeringBehaviors
    {
        // seek to a target lolcation
        public Vector2 seek(Player player, Vector2 targetPos)
        {
            Vector2 desiredVel = Vector2.Normalize(targetPos - player.Position);
            Vector2.Multiply(desiredVel, player.MaxSpeed);
            return (desiredVel - player.Velocity);
        }

        // flee from a target location
        public Vector2 flee(Player player, Vector2 targetPos)
        {
            Vector2 desiredVel = Vector2.Normalize(player.Position - targetPos);
            Vector2.Multiply(desiredVel, player.MaxSpeed);
            return (desiredVel - player.Velocity);
        }

        // arrive at a target location
        public Vector2 arrive(Player player, Vector2 targetPos)
        {
            const float DEACCELERATION = 20.0f;
            Vector2 target = targetPos - player.Position;
            float distance = target.Length();

            if (distance > 0)
            {
                float speed = (distance / DEACCELERATION);  // calculate the speed of the player
                speed = Math.Min(speed, player.MaxSpeed);   // return the smaller value to prevent exceeding the max speed
                return (Vector2.Divide((Vector2.Multiply(target, speed)), distance) - player.Velocity);
            }

            return (Vector2.Zero);
        }
    }
}
