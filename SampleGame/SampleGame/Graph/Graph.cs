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
        bool displayGraph;
        public List<Node> nodeList = new List<Node>();

        const int GRID_SIZE = 50;

        public Graph()
        {

        }

        public virtual void Update(GameTime gametime)
        {
            
        }

        public void addNode(Vector2 nodePosition)
        {
            nodeList.Add(new Node(nodePosition));
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
