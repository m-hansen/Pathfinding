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
        public List<Node> NodeList = new List<Node>();

        Node currentNode;
        Node targetNode;

        public Graph()
        {

        }

        public virtual void Update(GameTime gametime, Player player, BaseGameEntity crosshair)
        {

            foreach (Node node in NodeList)
            {
                // check for current node
                if (node.Cell.Contains(new Point((int)player.Position.X, (int)player.Position.Y)))
                {
                    currentNode = node;
                }

                // check for target node
                if (node.Cell.Contains(new Point((int)crosshair.Position.X, (int)crosshair.Position.Y)))
                {
                    targetNode = node;
                }
            }

            // update the adjacent nodes
            addAdjacentNodes();
        }

        public void addNode(Vector2 nodePosition)
        {
            NodeList.Add(new Node(nodePosition));
        }

        // An adjacent node will take into account the top, bottom, left and right nodes
        // it will NOT consider diagonals
        private void addAdjacentNodes()
        {
            // check for adjacent nodes
            foreach (Node node in NodeList)
            {
                foreach (Node node2 in NodeList)
                {
                    // check left, right, top, and bottom nodes
                    if ((!node.Equals(node2)) && (node2.id == (node.id + 1) || node2.id == (node.id - 1) ||
                        node2.id == (node.id + 16) || node2.id == (node.id - 16)))
                    {
                        bool found = false;     // does the adjacent node already exist?

                        // check the list of adjacent nodes
                        foreach (Node adjacentNode in node.AdjacentNodes)
                        {
                            if (node2.id == adjacentNode.id)
                                found = true;
                        }

                        // add an adjacent node if it wasn't already in the list
                        if (!found && node.Active && node2.Active)
                            node.AdjacentNodes.Add(node2);
                        // remove an inactive node
                        else if (found && !node.Active || !node2.Active)
                            node.AdjacentNodes.Remove(node2);
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            foreach (Node node in NodeList)
            {
                if (node.id == 86)
                {
                    int i = 200;
                    //sprites.DrawString(font1, "Adjacent Nodes: " + node.id, new Vector2(i, 200), Color.White);
                    foreach (Node anode in node.AdjacentNodes)
                    {
                        i += 20;
                        sprites.DrawString(font1, "Adjacent Nodes: " + anode.id, new Vector2(200, i), Color.White);
                    }
                }
            }

            sprites.DrawString(font1, "Current Node ID: " + currentNode.id, new Vector2(20, 230), Color.White);
            sprites.DrawString(font1, "Target Node ID: " + targetNode.id, new Vector2(20, 260), Color.White);
        }
    }
}
