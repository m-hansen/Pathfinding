﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Drawing;

namespace SampleGame
{
    public class Player : GameAgent
    {
        public float Speed;  // forward - backward speed
        private List<Sensor> sensorList = new List<Sensor>();
        private SteeringBehaviors steering = new SteeringBehaviors();
        private Vector2 previousPosition;

        public Player()
        {
            MaxSpeed = 150.0f;
            MaxForce = 2.0f;
            MaxTurnRate = 5.0f;
            Mass = 2.0f;
            Scale = 1.0f;
            Heading = new Vector2(0, -1);
            Rotation = 0;
        }

        public void InitializeSensors()
        {
            //sensorList.Add(new RangeFinder()
            //{
            //    Type = (int)Enums.SensorType.RangeFinder,
            //    Rotation = (float)Math.PI / 6,
            //    Key = Keys.P,
            //    MaxDistance = 150,
            //    Index = 0,
            //    DirectionText = "Right"
            //});

            //sensorList.Add(new RangeFinder()
            //{
            //    Type = (int)Enums.SensorType.RangeFinder,
            //    Rotation = 0,
            //    Key = Keys.P,
            //    MaxDistance = 150,
            //    Index = 1,
            //    DirectionText = "Middle"
            //});

            //sensorList.Add(new RangeFinder()
            //{
            //    Type = (int)Enums.SensorType.RangeFinder,
            //    Rotation = -1 * (float)Math.PI / 6,
            //    Key = Keys.P,
            //    MaxDistance = 150,
            //    Index = 2,
            //    DirectionText = "Left"
            //});

            //sensorList.Add(new AdjacentAgentSensor()
            //{
            //    Type = (int)Enums.SensorType.AgentSensor,
            //    Radius = 150,
            //    Key = Keys.O
            //});

            //sensorList.Add(new PieSliceSensor() // - 60 to 60 degrees
            //{
            //    Type = (int)Enums.SensorType.PieSliceSensor,
            //    Key = Keys.I,
            //    Rotation1 = -1 * (float)Math.PI / 3,
            //    Rotation2 = (float)Math.PI / 3,
            //    MaxDistance = 150,
            //    DisplayText = "(1,0) - Straight Ahead",
            //    Index = 0
            //});

            //sensorList.Add(new PieSliceSensor() // 60 to 120 degrees
            //{
            //    Type = (int)Enums.SensorType.PieSliceSensor,
            //    Key = Keys.I,
            //    Rotation1 = (float)Math.PI / 3,
            //    Rotation2 = 2 * (float)Math.PI / 3,
            //    MaxDistance = 150,
            //    DisplayText = "(0,1) - Right",
            //    Index = 1
            //});

            //sensorList.Add(new PieSliceSensor() // 120 to 240 degrees
            //{
            //    Type = (int)Enums.SensorType.PieSliceSensor,
            //    Key = Keys.I,
            //    Rotation1 = 2 * (float)Math.PI / 3,
            //    Rotation2 = 4 * (float)Math.PI / 3,
            //    MaxDistance = 150,
            //    DisplayText = "(-1,0) - Backwards",
            //    Index = 2
            //});

            //sensorList.Add(new PieSliceSensor() // 240 to 300 degrees
            //{
            //    Type = (int)Enums.SensorType.PieSliceSensor,
            //    Key = Keys.I,
            //    Rotation1 = 4 * (float)Math.PI / 3,
            //    Rotation2 = 5 * (float)Math.PI / 3,
            //    MaxDistance = 150,
            //    DisplayText = "(0,-1) - Left",
            //    Index = 3
            //});

            sensorList.Add(new PathNodeSensor()
            {
                Type = (int)Enums.SensorType.NodeSensor,
                Radius = 150,
                Key = Keys.U
            });
        }

        public Vector2 CalculateRotatedMovement(Vector2 point, float rotation)
        {
            return Vector2.Transform(point, Matrix.CreateRotationZ(rotation));
        }

        public void Update(GameTime gameTime, KeyboardState keyboardStateCurrent, KeyboardState keyboardStatePrevious, 
            MouseState mouseStateCurrent, MouseState mouseStatePrevious, List<Wall> wallList, Graph navagationGraph, List<GameAgent> agentAIList, BaseGameEntity crosshair,
            int windowWidth, int windowHeight)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // rotation
            if (keyboardStateCurrent.IsKeyDown(Keys.Left) || keyboardStateCurrent.IsKeyDown(Keys.A)){
                Rotation -= (elapsedTime * RotationSpeed) % MathHelper.TwoPi;}
            if (keyboardStateCurrent.IsKeyDown(Keys.Right) || keyboardStateCurrent.IsKeyDown(Keys.D))
                Rotation += (elapsedTime * RotationSpeed) % MathHelper.TwoPi;

            // check for wall collision
            Color = Color.White;
            foreach (Wall wall in wallList)
            {
                if (Bounds.Intersects(wall.Bounds))
                {
                    Position = previousPosition;
                    if (Math.Abs(wall.Position.Y - Position.Y) < 25 && wall.Position.X > Position.X)         // approaching from left
                        Velocity = Vector2.Reflect(Velocity, new Vector2(-1, 0));
                    else if (Math.Abs(wall.Position.Y - Position.Y) < 25 && wall.Position.X < Position.X)     // approaching from right
                        Velocity = Vector2.Reflect(Velocity, new Vector2(1, 0));
                    else if (Math.Abs(wall.Position.X - Position.X) < 25 && wall.Position.Y < Position.Y)    // approaching from bottom
                        Velocity = Vector2.Reflect(Velocity, new Vector2(0, -1));
                    else if (Math.Abs(wall.Position.X - Position.X) < 25 && wall.Position.Y > Position.Y)     // approaching from top
                        Velocity = Vector2.Reflect(Velocity, new Vector2(0, -1));
                    Position += Velocity;
                    Color = Color.Red;  // a quick flash to let the player know they hit a wall
                }
            }

            // seek waypoint
            if (navagationGraph.Path.Count != 0)
            {
                Vector2 steeringForce = steering.seek(this, navagationGraph.Path[0].Position);    // calculate the player's steering force
                Vector2 acceleration = Vector2.Divide(steeringForce, Mass);         // calculate the acceleration (A = F/M)
                Velocity += (acceleration * elapsedTime);                           // update the velocity (V = DT) 
                previousPosition = Position;
                Position += Velocity * elapsedTime;
            }
            // arrive at crosshair
            else if (Position.X - crosshair.Position.X < 25 && Position.Y - crosshair.Position.Y < 25)
            {
                Vector2 steeringForce = steering.arrive(this, crosshair.Position);    // calculate the player's steering force
                Vector2 acceleration = Vector2.Divide(steeringForce, Mass / 3);         // calculate the acceleration (A = F/M)
                Velocity += (acceleration * elapsedTime);                           // update the velocity (V = DT) 
                previousPosition = Position;
                Position += Velocity * elapsedTime;
            }
            // seek crosshair
            else
            {
                Vector2 steeringForce = steering.seek(this, crosshair.Position);    // calculate the player's steering force
                Vector2 acceleration = Vector2.Divide(steeringForce, Mass);         // calculate the acceleration (A = F/M)
                Velocity += (acceleration * elapsedTime);                           // update the velocity (V = DT) 
                previousPosition = Position;
                Position += Velocity * elapsedTime;
            }

            // update heading
            Vector2 v2 = Vector2.Multiply(Velocity, Velocity);  // Velocity squared
            if ((float)v2.Length() > 0.000001)
            {
                Heading = Vector2.Normalize(Velocity) * 40 + Position;
                // TODO update side
            }

            //float size = (float)Math.Atan2(crosshair.Position.X * Heading.Y - Heading.X * crosshair.Position.Y, Heading.X * crosshair.Position.X + Heading.Y * crosshair.Position.Y);
            //if (size < MathHelper.Pi)
            //    Rotation -= (size * elapsedTime) % MathHelper.TwoPi;
            //else
            //    Rotation += (size * elapsedTime) % MathHelper.TwoPi;
            //if (size <= MathHelper.PiOver2)
            //    Rotation += (size * elapsedTime * RotationSpeed) % MathHelper.TwoPi;    // sign +
            //else if (size > MathHelper.PiOver2)
            //    Rotation += (size * elapsedTime * RotationSpeed) % MathHelper.TwoPi;    // sign +
            //else if (size > MathHelper.PiOver2)
            //    Rotation -= (size * elapsedTime * RotationSpeed) % MathHelper.TwoPi;    // sign -
            //else if (size <= MathHelper.PiOver2)
            //    Rotation -= (size * elapsedTime * RotationSpeed) % MathHelper.TwoPi;    // sign -

            // movement
            if (keyboardStateCurrent.IsKeyDown(Keys.Up) || keyboardStateCurrent.IsKeyDown(Keys.W))
            {
                Vector2 nextPos = CalculateRotatedMovement(new Vector2(0, -1), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, wallList, agentAIList, windowWidth, windowHeight))
                    Position = nextPos;
            }
            if (keyboardStateCurrent.IsKeyDown(Keys.Down) || keyboardStateCurrent.IsKeyDown(Keys.S))
            {
                Vector2 nextPos = CalculateRotatedMovement(new Vector2(0, 1), Rotation) * Speed + Position;

                if (IsValidMove(nextPos, wallList, agentAIList, windowWidth, windowHeight))
                    Position = nextPos;
            }

            // update each sensor
            foreach (Sensor sensor in sensorList)
            {
                if (sensor.Type == (int)Enums.SensorType.NodeSensor)
                    sensor.Update(keyboardStateCurrent, navagationGraph, this.Position, this.Rotation);
                else if (sensor.Type == (int)Enums.SensorType.RangeFinder)
                    sensor.Update(keyboardStateCurrent, wallList, this.Position, this.Rotation);
                else
                    sensor.Update(keyboardStateCurrent, agentAIList, this.Position, this.Rotation);
            }


            base.Update(gameTime);
        }

        private bool IsValidMove(Vector2 nextPos, List<Wall> wallList, List<GameAgent> agentAIList, int windowWidth, int windowHeight)
        {
            Rectangle rect = new Rectangle
            (
                (int)(nextPos.X - Origin.X * Scale),
                (int)(nextPos.Y - Origin.Y * Scale),
                FrameWidth,
                FrameHeight
            );

            bool collision = false;

            foreach (Wall wall in wallList)
            {
                if (collision = wall.Bounds.Intersects(rect))
                    break;
            }

            foreach (GameAgent agent in agentAIList)
            {
                if (collision = agent.Bounds.Intersects(rect))
                    break;
            }

            return (!collision && rect.Left > 0 && rect.Left + rect.Width < windowWidth && rect.Top > 0 && rect.Top + rect.Height < windowHeight);
        }

        public override void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            foreach (Sensor sensor in sensorList)
            {
                sensor.Draw(sprites, this.Position, font1);
            }

            //DrawingHelper.DrawRectangle(Bounds, Color.Red, false); // debug - bounding rectangle
            DrawingHelper.DrawFastLine(Position, Heading, Color.Yellow); // debug - heading
            //sprites.Draw(Texture, Position, Bounds, Color, Rotation, Origin, Scale, SpriteEffects.None, 0.0f);

            base.Draw(sprites, font1);
        }
    }
}
