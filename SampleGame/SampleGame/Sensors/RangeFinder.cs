using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Drawing;

namespace SampleGame
{
    public class RangeFinder : Sensor
    {
        public float Rotation;                  // the angle of the range finder
        public int MaxDistance;                 // how far the range finder will reach
        public int Index;                       // index of range finder (used to determine where to display the distance on the screen)
        public string DirectionText;            // display which rangefinder if being referenced
        public Vector2 intersectingPoint;       // stores the point of intersection between the sensor and object

        private bool isIntersecting;            // if the rangefinder is currently intersecting with an object
        private Vector2 endPoint;               // the point to where the rangefinder stops
        private double intersectionDistance;    // if the range finder is currently intersecting an agent

        public override void Update(KeyboardState keyboard, List<Wall> wallList, Vector2 playerPos, float playerRot)
        {
            // reinitializing no intersection and the intersection distance 
            intersectionDistance = 0.0f;
            isIntersecting = false;

            // the sensor is active if the key is currently being pushed down
            Active = keyboard.IsKeyDown(Key);

            // end point of the range finder (beginning point is the player position)
            endPoint = CalculateRotatedMovement(new Vector2(0, -1), playerRot + Rotation) * MaxDistance + playerPos;

            // if the rangefinder's line is absolute vertical (slope would be infinite)
            bool isVerticalLine = endPoint.X == playerPos.X;

            // finding the slope of the rangefinder
            float slope = !isVerticalLine ? (endPoint.Y - playerPos.Y) / (endPoint.X - playerPos.X) : 0;

            // y = mx+b, finding b (b = y - mx)
            float offset = !isVerticalLine ? endPoint.Y - endPoint.X * slope : 0;

            //List<Wall> walls = wallList.Where(a => a.Type == (int)Enums.AgentType.Wall).ToList();

            // checking to see if the rangefinder is currently intersecting with a wall

            foreach (Wall wall in wallList)
            {
                // NOTE: We want to check all the intersections and not break out of the loop early 
                // incase the rangefinder is intersecting multiple agents
                CheckIntersection(wall, isVerticalLine, slope, offset, playerPos, endPoint);
            }
        }

        private void CheckIntersection(Wall wall, bool isVerticalLine, float slope, float offset, Vector2 rfStartPoint, Vector2 rfEndPoint)
        {
            // NOTE: drawing lines backwards so that higher values are always on the top and right
            // making it easier for calculation.
            Rectangle wallBounds = wall.Bounds;

            Vector2 bottomLeft = new Vector2(wallBounds.Left, wallBounds.Top);
            Vector2 bottomRight = new Vector2(wallBounds.Left + wallBounds.Width, wallBounds.Top);
            Vector2 topLeft = new Vector2(wallBounds.Left, wallBounds.Top + wallBounds.Height);
            Vector2 topRight = new Vector2(bottomRight.X, topLeft.Y);

            if (IsLineIntersection(topLeft, topRight, isVerticalLine, slope, offset, rfStartPoint, rfEndPoint))
                isIntersecting = true;
            if (IsLineIntersection(bottomLeft, bottomRight, isVerticalLine, slope, offset, rfStartPoint, rfEndPoint))
                isIntersecting = true;
            if (IsLineIntersection(bottomLeft, topLeft, isVerticalLine, slope, offset, rfStartPoint, rfEndPoint))
                isIntersecting = true;
            if (IsLineIntersection(bottomRight, topRight, isVerticalLine, slope, offset, rfStartPoint, rfEndPoint))
                isIntersecting = true;
        }

        private bool IsLineIntersection(Vector2 targetStartPoint, Vector2 targetEndPoint, bool isVerticalLine, float slope, float offset, Vector2 rfStartPoint, Vector2 rfEndPoint)
        {
            // if our range finder is a vertical line
            if (isVerticalLine)
            {
                // if the agent's wall is a vertical line
                if (targetStartPoint.X == targetEndPoint.X)
                    return false;

                // the rangefinders endpoint's y coord maybe lower than it's startpoint's y coord
                float rfMinY = rfEndPoint.Y > rfStartPoint.Y ? rfStartPoint.Y : rfEndPoint.Y;
                float rfMaxY = rfEndPoint.Y > rfStartPoint.Y ? rfEndPoint.Y : rfStartPoint.Y;

                // if the rangefinder's X position is within the target's X range
                // and the target's Y position is within the the rangefinder's Y range
                // then there is an intersection, otherwise, no intersection
                if (targetStartPoint.X <= rfStartPoint.X && targetEndPoint.X >= rfStartPoint.X
                    && rfMinY <= targetStartPoint.Y && rfMaxY >= targetStartPoint.Y)
                {
                    CalculateDistanceToIntersection(rfStartPoint, new Vector2(rfStartPoint.X, targetStartPoint.Y));
                    return true;
                }

                // no intersection at this point
                return false;
            }

            // if the agent's wall is a vertical line
            if (targetStartPoint.X == targetEndPoint.X)
            {
                // finding the intersection point's y coordinate: y = mx + b
                float intersectingYPos = targetStartPoint.X * slope + offset;

                // the rangefinders endpoint's y coord maybe lower than it's startpoint's y coord
                float rfMinY = rfEndPoint.Y > rfStartPoint.Y ? rfStartPoint.Y : rfEndPoint.Y;
                float rfMaxY = rfEndPoint.Y > rfStartPoint.Y ? rfEndPoint.Y : rfStartPoint.Y;

                // if both lines contain this y coordinate
                if (targetStartPoint.Y <= intersectingYPos && targetEndPoint.Y >= intersectingYPos
                        && rfMinY <= intersectingYPos && rfMaxY >= intersectingYPos)
                {
                    CalculateDistanceToIntersection(rfStartPoint, new Vector2(targetStartPoint.X, intersectingYPos));
                    return true;
                }

                // no intersection at this point
                return false;
            }

            // finding the intersection point's x coordinate: x = (y - b) / m
            float intersectingXPos = (targetStartPoint.Y - offset) / slope;

            // the rangefinders endpoint's x coord maybe lower than it's startpoint's x coord
            float rfMinX = rfEndPoint.X > rfStartPoint.X ? rfStartPoint.X : rfEndPoint.X;
            float rfMaxX = rfEndPoint.X > rfStartPoint.X ? rfEndPoint.X : rfStartPoint.X;

            // if both lines contain this x coordinate
            if (targetStartPoint.X <= intersectingXPos && targetEndPoint.X >= intersectingXPos
                && rfMinX <= intersectingXPos && rfMaxX >= intersectingXPos)
            {
                CalculateDistanceToIntersection(rfStartPoint, new Vector2(intersectingXPos, targetStartPoint.Y));
                return true;
            }

            // no intersection at this point
            return false;
        }

        private void CalculateDistanceToIntersection(Vector2 startPos, Vector2 endPos)
        {
            // calculating the distance between the two points
            double distance = Math.Sqrt(Math.Pow(endPos.Y - startPos.Y, 2) + Math.Pow(endPos.X - startPos.X, 2));

            // if the intersectionDistance has yet to be calculated (first intersection)
            if (intersectionDistance == 0.0f || distance < intersectionDistance)
            {
                intersectionDistance = distance;
                intersectingPoint = endPos;
            }
        }

        public override void Draw(SpriteBatch sprites, Vector2 startPoint, SpriteFont font1)
        { 
            if (Active)
            {
                DrawingHelper.DrawFastLine(startPoint, isIntersecting ? intersectingPoint : endPoint, isIntersecting ? Color.Red : Color.Yellow);
               
                if (isIntersecting)
                {
                    // draw only the distance
                    sprites.DrawString(font1, "Range Finder " + DirectionText + ": " + Math.Round(intersectionDistance, 2) + " pixels", 
                        new Vector2(20, 400 + 20 * Index), Color.LightGreen, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                }
            }
        }
    }
}
