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

        int numHorizNodes = 16;
        int numVertNodes = 12;
        bool nodeNeedsUpdate = false;

        public Node CurrentNode;
        public Node TargetNode;

        public Graph()
        {
        }

        public virtual void Update(GameTime gameTime, Player player, BaseGameEntity crosshair, List<Wall> wallList)
        {
            updateNodeList(gameTime, player, crosshair, wallList);
            updateTargetNode(crosshair);
            updateCurrentNode(player);

            // get the heuristic value for each node
            if (nodeNeedsUpdate)
            {
                // calculate the H value
                foreach (Node node in NodeList)
                {
                    node.heuristic = calculateHeuristic(node, TargetNode);
                }
            }

            // update the adjacent nodes
            addAdjacentNodes();
        }

        // update each node
        private void updateNodeList(GameTime gameTime, Player player, BaseGameEntity crosshair, List<Wall> wallList)
        {
            foreach (Node node in NodeList)
            {
                node.Update(gameTime, player, crosshair, wallList);
            }
        }

        // update the target node
        private void updateTargetNode(BaseGameEntity crosshair)
        {
            nodeNeedsUpdate = false;

            foreach (Node node in NodeList)
            {
                // check for target node
                if (node != TargetNode && node.Cell.Contains(new Point((int)crosshair.Position.X, (int)crosshair.Position.Y)))
                {
                    TargetNode = node;
                    nodeNeedsUpdate = true;
                }
            }
        }

        // update the current node
        private void updateCurrentNode(Player player)
        {
            foreach (Node node in NodeList)
            {
                // check for current node
                if (node.Cell.Contains(new Point((int)player.Position.X, (int)player.Position.Y)))
                {
                    CurrentNode = node;
                }
            }
        }

        // calculate the h (heuristic) value for A*
        private int calculateHeuristic(Node currentNode, Node targetNode)
        {
            if (currentNode.Position.X >= targetNode.Position.X && currentNode.Position.Y >= targetNode.Position.Y)
                return (int)(Math.Abs((currentNode.Position.X / Node.CELL_SIZE - targetNode.Position.X / Node.CELL_SIZE) + (currentNode.Position.Y / Node.CELL_SIZE - targetNode.Position.Y / Node.CELL_SIZE)));
            else if (currentNode.Position.X < targetNode.Position.X && currentNode.Position.Y >= targetNode.Position.Y)
                return (int)(Math.Abs((targetNode.Position.X / Node.CELL_SIZE - currentNode.Position.X / Node.CELL_SIZE) + (currentNode.Position.Y / Node.CELL_SIZE - targetNode.Position.Y / Node.CELL_SIZE)));
            else if (currentNode.Position.X >= targetNode.Position.X && currentNode.Position.Y < targetNode.Position.Y)
                return (int)(Math.Abs((currentNode.Position.X / Node.CELL_SIZE - targetNode.Position.X / Node.CELL_SIZE) + (targetNode.Position.Y / Node.CELL_SIZE - currentNode.Position.Y / Node.CELL_SIZE)));
            else
                return (int)(Math.Abs((targetNode.Position.X / Node.CELL_SIZE - currentNode.Position.X / Node.CELL_SIZE) + (targetNode.Position.Y / Node.CELL_SIZE - currentNode.Position.Y / Node.CELL_SIZE)));
        }

        // add a node to the list
        public void addNode(Vector2 nodePosition)
        {
            NodeList.Add(new Node(nodePosition));
        }

        // An adjacent node will take into account all of the nodes around it that aren't walls
        private void addAdjacentNodes()
        {
            // check for adjacent nodes
            foreach (Node node in NodeList)
            {
                foreach (Node node2 in NodeList)
                {
                    // check left, right, top, and bottom nodes
                    if ((!node.Equals(node2)) && (node2.id == (node.id + 1) || node2.id == (node.id - 1) || // left/right
                        node2.id == (node.id + 16) || node2.id == (node.id - 16) ||                         // top/bottom
                        node2.id == (node.id - 17) || node2.id == (node.id - 15) ||                         // bottom right/left
                        node2.id == (node.id + 17) || node2.id == (node.id + 15)))                          // top right/left
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
        }
    }
}
