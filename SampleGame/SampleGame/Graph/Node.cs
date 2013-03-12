using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame
{
    public class Node : BaseGameEntity
    {
        public int id = -1;                // the id for each node
        static int nextID = 0;             // keeps track of the next avaliable id
        static Texture texture;

        public Node(Vector2 pos)
        {
            id = nextID;
            Position = pos;
        }

        public virtual void LoadContent(ContentManager contentManager, string assetName)
        {
            // loading the image for the object
            texture = contentManager.Load<Texture2D>("Images\\waypoint");

            // setting the origin to the center of the object
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            //sprites.Draw(Texture, Position - Origin, Color.White);
        }
    }
}
