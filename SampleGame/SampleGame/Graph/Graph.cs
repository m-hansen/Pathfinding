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
    public class Graph
    {
        public List<Node> NodeList = new List<Node>();          // the list of all nodes in the graph

        // used for A*
        public List<Node> OpenList = new List<Node>();          // contains a list of nodes that need to be checked
        public List<Node> ClosedList = new List<Node>();        // contains a list of nodes that have been checked already
        List<Node> path = new List<Node>();

        int numHorizNodes = 16;                                 // number of horizontal nodes
        int numVertNodes = 12;                                  // number of vertical nodes

        /* TODO should be false */
        bool nodeNeedsUpdate = true;                           // used to determine if we need to update the node or not



        bool isOnOpenList = false;                              // used to determine if the node in on the open list
        bool isOnClosedList = false;                            // used to determine if the node in on the open list

        public Node StartNode;                                  // the node we begin our search at
        public Node CurrentNode;                                // the current node to evaluate
        public Node TargetNode;                                 // the target we wish to reach

        public Graph()
        {
        }

        public virtual void Update(GameTime gameTime, MouseState mouseStateCurrent, MouseState mouseStatePrevious, Player player, BaseGameEntity crosshair, List<Wall> wallList)
        {
            // pressing the left mouse button sets the start node
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed)
            {
                foreach (Node node in NodeList)
                {
                    if (node.Cell.Contains(new Point((int)mouseStateCurrent.X, (int)mouseStateCurrent.Y)))
                    {
                        if (StartNode != null) 
                            StartNode.Color = Color.LightGray;
                        StartNode = node;
                    }
                }
            }

            // pressing the right mouse button sets the target node
            if (mouseStateCurrent.RightButton == ButtonState.Pressed && mouseStatePrevious.RightButton != ButtonState.Pressed)
            {
                foreach (Node node in NodeList)
                {
                    if (node.Cell.Contains(new Point((int)mouseStateCurrent.X, (int)mouseStateCurrent.Y)))
                    {
                        if (TargetNode != null)
                            TargetNode.Color = Color.LightGray;
                        TargetNode = node;
                    }
                }
            }

            // check to make sure the nodes exists before setting the color
            if (StartNode != null) 
                StartNode.Color = Color.Green;       
            if (TargetNode != null)
                TargetNode.Color = Color.DarkOrange;

            updateNodeList(gameTime, player, crosshair, wallList);
            updateTargetNode(crosshair);
            updateCurrentNode(player);
            //if (nodeNeedsUpdate)
                //calculateAStar(StartNode, TargetNode);

            // get the Heuristic value for each node
            if (nodeNeedsUpdate)
            {
                // calculate the H value
                foreach (Node node in NodeList)
                {
                    node.Heuristic = calculateHeuristic(node, TargetNode);
                }
            }


            // ************************ BEGIN CHECKING LISTS ************************ //

            // reset the flags before evaluating lists
            isOnOpenList = false;
            isOnClosedList = false;

            // check to see if the current node is on the open list
            foreach (Node openNode in OpenList)
            {
                if (CurrentNode == openNode)
                    isOnOpenList = true;
            }

            // check to see if the current node is on the closed list
            foreach (Node closedNode in ClosedList)
            {
                if (CurrentNode == closedNode)
                    isOnClosedList = true;
            }

            // if the current node is not on either list
            // then the current node's neighbors must have their parent node set to the current node
            // in other words, current node's neighbors are the children nodes
            if (!isOnClosedList && !isOnOpenList)
            {
                // first we must add the current node to the closed list
                ClosedList.Add(CurrentNode);

                // next we need to add nodes to the open list
                foreach (Node node in CurrentNode.AdjacentNodes)
                    OpenList.Add(node);

                // TODO: should this iterate through each node in the closed list to update it?
                // or should it just take the current node?

                // TODO: should this iterate through the entire open list or use CurrentNode.adjacentnodes?

                // now we set the parent node for each adjacent node
                foreach (Node node in OpenList)
                    node.ParentNode = CurrentNode;
            }

            // ************************ END CHECKING LISTS ************************ //


            // calculate the movement cost of each node
            foreach (Node potentialNode in CurrentNode.AdjacentNodes)
            {
                potentialNode.MovementCost = calculateMovementCost(CurrentNode, potentialNode);
            }

            // update the adjacent nodes
            addAdjacentNodes();
        }

        //// TODO
        //private Node calculateAStar(Node start, Node target)
        //{
        //    nodeNeedsUpdate = false;
        //    // make sure the lists are empty before beginning the algorithm
        //    ClosedList.Clear();
        //    OpenList.Clear();
        //    ///////////////////////////////

        //    OpenList.Add(start);
        //    path.Add(start);

        //    start.MovementCost = 0;                                     // initialize the movement cost (G)
        //    int gScore = start.MovementCost;                            // g = MovementCost           
        //    int fScore = start.MovementCost + start.Heuristic;          // F = G + H

        //    while (OpenList != null)
        //    {
        //        int lowestCost = OpenList[0].TotalCost;  // TODO - should this be outside of while?
        //        // get the node that has the lowest F cost (total cost)
        //        foreach (Node node in OpenList)
        //        {
        //            if (node.TotalCost < lowestCost)
        //                CurrentNode = node;
        //        }

        //        // move the current node from the open list to the closed list
        //        OpenList.Remove(CurrentNode);
        //        ClosedList.Add(CurrentNode);

        //        foreach (Node neighbor in CurrentNode.AdjacentNodes)
        //        {
        //            bool found = false;
        //            int potentialMovementCost = gScore + calculateMovementCost(CurrentNode, neighbor);

        //            foreach (Node closedNode in ClosedList)
        //            {
        //                if (neighbor.id == closedNode.id)
        //                {
        //                    if (potentialMovementCost >= gScore)
        //                        continue;
        //                    found = true;
        //                }
        //            }

        //            if (!found || potentialMovementCost < gScore)
        //            {
        //                path.Add(neighbor);     // TODO - should be currentnode?? // TODO - foreach??
        //                gScore = potentialMovementCost;
        //                fScore = gScore + calculateHeuristic(neighbor, target);
        //            }
        //        }

        //        if (CurrentNode == target)
        //                return (modifyPath(path, target));
        //    }
        //    return null;
        //    // TODO - return failure condition
        //    // on this line
        //}

        //// recursively make modifications to the path
        //private Node modifyPath(List<Node> previousNode, Node currentNode)
        //{
        //    foreach (Node node in previousNode)
        //    {
        //        if (node.id == currentNode.id)
        //        {
        //            Node temp = modifyPath(previousNode, node);
        //            //return (temp + currentNode);
        //        }
        //    }
        //    return currentNode;
        //}

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

        // calculate the h (Heuristic) value for A*
        private int calculateHeuristic(Node currentNode, Node targetNode)
        {
            // calculate based on the X/Y coordinates of the current node and the target node
            if (currentNode.Position.X >= targetNode.Position.X && currentNode.Position.Y >= targetNode.Position.Y)
                return (int)(Math.Abs((currentNode.Position.X / Node.CELL_SIZE - targetNode.Position.X / Node.CELL_SIZE) + (currentNode.Position.Y / Node.CELL_SIZE - targetNode.Position.Y / Node.CELL_SIZE)));
            else if (currentNode.Position.X < targetNode.Position.X && currentNode.Position.Y >= targetNode.Position.Y)
                return (int)(Math.Abs((targetNode.Position.X / Node.CELL_SIZE - currentNode.Position.X / Node.CELL_SIZE) + (currentNode.Position.Y / Node.CELL_SIZE - targetNode.Position.Y / Node.CELL_SIZE)));
            else if (currentNode.Position.X >= targetNode.Position.X && currentNode.Position.Y < targetNode.Position.Y)
                return (int)(Math.Abs((currentNode.Position.X / Node.CELL_SIZE - targetNode.Position.X / Node.CELL_SIZE) + (targetNode.Position.Y / Node.CELL_SIZE - currentNode.Position.Y / Node.CELL_SIZE)));
            else
                return (int)(Math.Abs((targetNode.Position.X / Node.CELL_SIZE - currentNode.Position.X / Node.CELL_SIZE) + (targetNode.Position.Y / Node.CELL_SIZE - currentNode.Position.Y / Node.CELL_SIZE)));
        }

        // calculate the g (movement) cost for A*
        private int calculateMovementCost(Node currentNode, Node targetNode)
        {
            // the node is directly horizontal or vertical and should have a movement cost of 10
            if (currentNode.id == (targetNode.id + 1) || currentNode.id == (targetNode.id - 1) ||  // left/right
                currentNode.id == (targetNode.id + 16) || currentNode.id == (targetNode.id - 16))   // top/bottom
                return 10;
            
            // the node is at a diagonal and should have a movement cost of 14
            if (currentNode.id == (targetNode.id - 17) || currentNode.id == (targetNode.id - 15) || // bottom right/left
                currentNode.id == (targetNode.id + 17) || currentNode.id == (targetNode.id + 15))   // top right/left
                return 14;

            // no movement cost because the nodes are the same (or an error occured)
            return 0;
        }

        // add a node to the list
        public void addNode(Vector2 nodePosition)
        {
            NodeList.Add(new Node(nodePosition));
        }

        // An adjacent node will take into account all of the nodes around it
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
            if (StartNode != null)
                DrawingHelper.DrawFastLine(StartNode.Position, TargetNode.Position, Color.White);
        }
    }
}
