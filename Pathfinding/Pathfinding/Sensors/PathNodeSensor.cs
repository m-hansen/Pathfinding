using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Drawing;

namespace SampleGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PathNodeSensor : Sensor
    {
        private bool isInRange;                     // if an agent is 
        private Vector2 distance = new Vector2();   // distance between agent and player

        private List<InRangeInfo> inRangeInfoList = new List<InRangeInfo>();

        public override void Update(KeyboardState keyboard, Graph navagationGraph, Vector2 playerPos, float playerRot)
        {
            inRangeInfoList.Clear();
            isInRange = false;

            // the sensor is active if the key is currently being pushed down
            Active = keyboard.IsKeyDown(Key);

            //List<Node> nodeList = navagationGraph.NodeList.Where(a => (int)a.Type == (int)Enums.AgentType.NPC).ToList();

            // check if agent is within the radius
            foreach (Node node in navagationGraph.NodeList)
            {
                if (node.Active) // make sure the node isn't a wall
                {
                    // get the X/Y distance between player and agent
                    distance.X = playerPos.X - node.Position.X;
                    distance.Y = playerPos.Y - node.Position.Y;

                    float dist = (float)Math.Sqrt(Math.Pow(distance.X, 2) + Math.Pow(distance.Y, 2));

                    // check if an agent is within range
                    if (dist <= Radius)
                    {
                        isInRange = true;

                        inRangeInfoList.Add(new InRangeInfo()
                        {
                            Distance = dist,
                            Rotation = CalculateRotation(playerPos, playerRot, node.Position),
                            Position = node.Position
                        });
                    }
                }
            }
        }

        private float CalculateRotation(Vector2 playerPos, float playerRot, Vector2 targetPos)
        {
            Vector2 dist = playerPos - targetPos;

            playerRot = playerRot % (float)(MathHelper.TwoPi);

            double temp1 = Math.Atan2(dist.X, -dist.Y);
            double temp2 = temp1 - playerRot - MathHelper.Pi;
            double temp3 = temp2 % (MathHelper.TwoPi);

            if (temp3 < 0)
                temp3 += MathHelper.TwoPi;

            return (float)(temp3);
        }

        private float GetRotationInDegrees(float Rotation)
        {
            return (float)Math.Round(Rotation * 180 / MathHelper.Pi, 2);
        }

        public override void Draw(SpriteBatch sprites, Vector2 center, SpriteFont font1)
        {
            if (Active)
            {
                DrawingHelper.DrawCircle(new Vector2(center.X, center.Y), Radius, (isInRange ? Color.Red : Color.Green), false);

                if (isInRange)
                {
                    string text = "Path Node Sensor: [";

                    foreach (InRangeInfo inRangeInfo in inRangeInfoList)
                    {
                        float targetAngle = GetRotationInDegrees(inRangeInfo.Rotation);     // calculate the angle of the target in relation to the player
                        float targetDistance = (float)Math.Round(inRangeInfo.Distance, 2);  // calculate the distance between the target and player

                        // draw a line from the player each node for debugging purposes
                        DrawingHelper.DrawFastLine(new Vector2(center.X, center.Y), 
                            new Vector2(inRangeInfo.Position.X, inRangeInfo.Position.Y), 
                            Color.MediumPurple);

                        text += "(" + targetAngle + ", " + targetDistance + ")";
                    }

                    text += "]";

                    sprites.DrawString(font1, text, new Vector2(20, 560), Color.LightGreen, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                    sprites.DrawString(font1, "     (Angle, Distance)", new Vector2(20, 580), Color.LightGreen, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                }
            }
        }

        public class InRangeInfo
        {
            public float Distance;
            public float Rotation;
            public Vector2 Position;
        }
    }
}
