using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Drawing;

namespace SampleGame
{
    public class Node : BaseGameEntity
    {
        public int id = -1;                 // the id for each node
        static int nextID = 0;              // keeps track of the next avaliable id
        //public static Texture2D NodeTexture;             // todo?
        List<Node> adjacentNodes = new List<Node>();
        Rectangle cell;                     // the bounding rectangle
        Vector2 waypoint;                   // the origin/waypoint in the cell/node

        public Node(Vector2 pos)
        {
            id = nextID;
            Position = pos;
            cell = new Rectangle((int)Position.X-25, (int)Position.Y-25, 50, 50);
            waypoint = Position;
        }

        //public virtual void LoadContent(ContentManager contentManager, string assetName)
        //{
        //    // loading the image for the object
        //    NodeTexture = contentManager.Load<Texture2D>("Images\\waypoint");

        //    // setting the origin to the center of the object
        //    Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        //}

        public virtual void Update(GameTime gametime)
        {
            //waypoint = new Vector2(Position.X + Texture.Width, Position.Y + Texture.Height);
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            DrawingHelper.DrawRectangle(cell, Color.Gray, false);
            sprites.Draw(Texture, waypoint - Origin, Color.LightGray);
        }
    }
}
