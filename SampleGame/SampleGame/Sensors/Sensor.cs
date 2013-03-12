using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame
{
    public class Sensor
    {
        public int Type;            // which type of sensor (see Enums class)
        public bool Active;         // whether the sensor is active
        public float Radius;        // used for Agent Sensors
        public Keys Key;            // key to be pushed to activate sensor

        public virtual void Update(KeyboardState keyboard, List<GameAgent> agentAIList, Vector2 playerPos, float playerRot)
        {
            
        }

        public Vector2 CalculateRotatedMovement(Vector2 point, float rotation)
        {
            return Vector2.Transform(point, Matrix.CreateRotationZ(rotation));
        }

        public virtual void Draw(SpriteBatch sprites, Vector2 startPoint, SpriteFont font1)
        {

        }
    }
}
