using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Drawing;

namespace SampleGame
{
    public class Node : BaseGameEntity
    {
        public int id = -1;                                     // the id for each node
        public List<Node> AdjacentNodes = new List<Node>();     // list of adjacent nodes
        public Rectangle Cell;                                  // the bounding rectangle
        public bool Active = true;                              // flag to determine whether the node is reachable or not (ie: is it a waypoint or a wall?)

        static int nextID = 0;                                  // keeps track of the next avaliable id
        public const int CELL_SIZE = 50;                        // the size of each cell
        public int Heuristic;                                   // H - Heuristic
        public int MovementCost;                                // G - movement cost
        public int TotalCost;                                   // F = G + H
        public Node ParentNode;                                 // nodes in the A* algorithm will have a parent node

        public bool IsStart = false;

        public Node(Vector2 pos)
        {
            id = getNextID();
            Position = pos;
            Cell = new Rectangle((int)(Position.X - CELL_SIZE / 2), (int)(Position.Y - CELL_SIZE / 2), CELL_SIZE, CELL_SIZE);
        }

        // return the next avaliable ID for a node
        private int getNextID()
        {
            return nextID++;
        }

        public virtual void Update(GameTime gametime, Player player, BaseGameEntity crosshair, List<Wall> wallList)
        {
            // Change the color of a node based on what entity is near
            //if (Cell.Contains(new Point((int)player.Position.X, (int)player.Position.Y)))
            //    Color = Color.Green;
            //else if (Cell.Contains(new Point((int)crosshair.Position.X, (int)crosshair.Position.Y)))
            //    Color = Color.DarkOrange;
            //else
            //    Color = Color.LightGray;


            // check if the cell contains a wall
            foreach (Wall wall in wallList)
            {
                // if it does contain a wall, set to inactive
                if (Cell.Contains(new Point((int)wall.Position.X, (int)wall.Position.Y)))
                    Active = false;
            }

            // calculate the f (total cost) value
            TotalCost = MovementCost + Heuristic;   // F = G + H
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            // draw the grid
            DrawingHelper.DrawRectangle(Cell, Color.Gray, false);

            if (Active)
                sprites.Draw(Texture, Position - Origin, Color);

            // display debug information in each cell
            sprites.DrawString(font1, id.ToString(), Position + new Vector2(-15, -15), Color.White, Rotation, Origin, 0.5f, SpriteEffects.None, 1.0f);              // display the node id          (top left)
            sprites.DrawString(font1, Heuristic.ToString(), Position + new Vector2(15, -15), Color.Yellow, Rotation, Origin, 0.5f, SpriteEffects.None, 1.0f);       // display the Heuristic        (top right)
            sprites.DrawString(font1, MovementCost.ToString(), Position + new Vector2(15, 15), Color.Yellow, Rotation, Origin, 0.5f, SpriteEffects.None, 1.0f);     // display the movement cost    (bottom right)
            sprites.DrawString(font1, TotalCost.ToString(), Position + new Vector2(-15, 15), Color.Yellow, Rotation, Origin, 0.5f, SpriteEffects.None, 1.0f);       // display the total cost       (bottom left)
        }
    }
}
