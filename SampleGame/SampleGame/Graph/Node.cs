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
        public int id = -1;                 // the id for each node
        static int nextID = 0;              // keeps track of the next avaliable id
        //public static Texture2D NodeTexture;             // todo?
        List<Node> adjacentNodes = new List<Node>();
        public Rectangle Cell;                     // the bounding rectangle
        Vector2 waypoint;                   // the origin/waypoint in the cell/node
        public bool Active = true;          // flag to determine whether the node is reachable or not (ie: is it a waypoint or a wall?)

        public Node(Vector2 pos)
        {
            id = getNextID();
            Position = pos;
            Cell = new Rectangle((int)Position.X-25, (int)Position.Y-25, 50, 50);
            waypoint = Position;
        }

        private int getNextID()
        {
            return nextID++;
        }

        //public virtual void LoadContent(ContentManager contentManager, string assetName)
        //{
        //    // loading the image for the object
        //    NodeTexture = contentManager.Load<Texture2D>("Images\\waypoint");

        //    // setting the origin to the center of the object
        //    Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        //}

        public virtual void Update(GameTime gametime, Player player)
        {
            Color = Cell.Contains(new Point((int)player.Position.X, (int)player.Position.Y)) ? Color.Green : Color.LightGray;

            // if active add waypoint else add wall - set states?
            //Texture = Active ? LoadContent(contentManager, "Images\\waypoint") : LoadContent(contentManager, "Images\\wall");
        }

        public bool doesCellContainPlayer(Player player)
        {
            if (Cell.Contains(new Point((int)player.Position.X, (int)player.Position.Y)))
                return true;
            return false;
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            DrawingHelper.DrawRectangle(Cell, Color.Gray, false);
            sprites.Draw(Texture, waypoint - Origin, Color);
            sprites.DrawString(font1, id.ToString(), Position - new Vector2(15, 15), Color.White, Rotation, Origin, 0.5f, SpriteEffects.None, 1.0f);
        }
    }
}
