using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SampleGame
{
    class SteeringBehaviors
    {
        public Vector2 seek(Player player, Vector2 targetPos)
        {
            Vector2 desiredVel = Vector2.Normalize(targetPos - player.Position);
            Vector2.Multiply(desiredVel, player.MaxSpeed);
            return (desiredVel - player.Velocity);
        }

        public Vector2 flee(Player player, Vector2 targetPos)
        {
            Vector2 desiredVel = Vector2.Normalize(player.Position - targetPos);
            Vector2.Multiply(desiredVel, player.MaxSpeed);
            return (desiredVel - player.Velocity);
        }
    }
}
