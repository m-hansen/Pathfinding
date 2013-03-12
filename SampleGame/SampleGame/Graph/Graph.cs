using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame
{
    class Graph
    {
        public List<Node> nodeList = new List<Node>();

        public Graph()
        {
            nodeList.Add(new Node(new Vector2(300,300)));
        }

        public virtual void Update(GameTime gametime)
        {
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            foreach (Node node in nodeList)
            {
                sprites.Draw(node.Texture, node.Position - node.Origin, Color.White);
                sprites.DrawString(font1, "Node ID: " + node.id, new Vector2(200, 200), Color.White);
            }
        }
    }
}
