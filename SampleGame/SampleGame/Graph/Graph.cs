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
        bool nodeNeedsUpdate = false;                            // used to determine if we need to update the node or not

        bool isOnOpenList = false;                              // used to determine if the node in on the open list
        bool isOnClosedList = false;                            // used to determine if the node in on the open list
        bool found = false;

        public Node StartNode;                                  // the node we begin our search at
        public Node CurrentNode;                                // the current node to evaluate
        public Node TargetNode;                                 // the target we wish to reach

        public Graph()
        {
        }

        public virtual void Update(GameTime gameTime, MouseState mouseStateCurrent, MouseState mouseStatePrevious, Player player, BaseGameEntity crosshair, List<Wall> wallList)
        {           
            // pressing the left mouse button sets the start node
            /*if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed)
            {
                foreach (Node node in NodeList)
                {
                    if (node.Active && node.Cell.Contains(new Point((int)mouseStateCurrent.X, (int)mouseStateCurrent.Y)))
                    {
                        if (StartNode != null) 
                            StartNode.Color = Color.LightGray;
                        StartNode = node;
                        CurrentNode = StartNode;            // set the current node tot he start node
                        StartNode.MovementCost = 0;         // since this is the starting node, the movement cost must be zero
                        nodeNeedsUpdate = true;
                    }
                }
            }*/

            // pressing the right mouse button sets the target node
            if (mouseStateCurrent.RightButton == ButtonState.Pressed && mouseStatePrevious.RightButton != ButtonState.Pressed)
            {
                foreach (Node node in NodeList)
                {
                    if (node.Active && node.Cell.Contains(new Point((int)player.Position.X, (int)player.Position.Y)))
                    {
                        StartNode = node;
                        CurrentNode = StartNode;
                    }

                    if (node.Active && node.Cell.Contains(new Point((int)mouseStateCurrent.X, (int)mouseStateCurrent.Y)))
                    {
                        if (TargetNode != null)
                            TargetNode.Color = Color.LightGray;
                        TargetNode = node;
                    }
                }
                nodeNeedsUpdate = true;
            }

            // check to make sure the nodes exists before setting the color
            if (StartNode != null) 
                StartNode.Color = Color.Green;       
            if (TargetNode != null)
                TargetNode.Color = Color.DarkOrange;

            // update the nodes and their neighbors
            updateNodeList(gameTime, player, crosshair, wallList);
            addAdjacentNodes();

            // run the A* algorithm
            if (nodeNeedsUpdate && TargetNode != null && StartNode != null && StartNode != TargetNode)
                calculateAStar();
        }

        // clear lists, reset node costs, and calculate heuristic values
        private void initializeAStar()
        {
            // reset flags
            found = false;
            nodeNeedsUpdate = false;

            // clear the lists
            OpenList.Clear();
            ClosedList.Clear();

            // initialize F and G
            foreach (Node node in NodeList)
            {
                node.MovementCost = 0;
                node.TotalCost = 0;
                node.Color = Color.LightGray;
            }

            // get the Heuristic value for each node
            foreach (Node node in NodeList)
            {
                node.Heuristic = calculateHeuristic(node, TargetNode);
            }
        }

        private void calculateAStar()
        {
            // clear lists, reset node costs, and calculate heuristic values
            initializeAStar();

            // continue to calculate a path until the target is reached
            while (!found)
                calculatePath();
        }

        private void calculatePath()
        {
            // check all the adjacent nodes
            foreach (Node neighbor in CurrentNode.AdjacentNodes)
                setNodeValues(neighbor);

            // move from open list to closed list
            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            // set the current node to the node with the smallest total (F) value
            CurrentNode = getSmallestFNode();
        }

        private void setNodeValues(Node neighbor)
        {
            // check for null case
            if (neighbor == null)
                return;

            // check for end case
            if (neighbor == TargetNode)
            {
                TargetNode.ParentNode = CurrentNode;        // set the parent node
                found = true;                               // we have found our target node!
                return;
            }

            // check to make sure the node is not on our closed list
            if (!ClosedList.Contains(neighbor))
            {
                // check to see if it is on the open list
                if (OpenList.Contains(neighbor))
                {
                    // calculate a new movement cost (G)
                    int gcost = CurrentNode.MovementCost + calculateMovementCost(CurrentNode, neighbor);

                    // check for a lower movement cost
                    if (gcost < neighbor.MovementCost)
                    {
                        neighbor.ParentNode = CurrentNode;                                      // set the new parent node
                        neighbor.MovementCost = gcost;                                          // set the new movement cost (G)
                        neighbor.TotalCost = calculateTotalCost(neighbor);                      // set the new total cost (F)
                    }
                }
                else        
                {
                    // not on open or closed list
                    neighbor.ParentNode = CurrentNode;                                                                  // set parent
                    neighbor.MovementCost = CurrentNode.MovementCost + calculateMovementCost(CurrentNode, neighbor);    // set movement cost (G)
                    neighbor.TotalCost = calculateTotalCost(neighbor);                                                  // set total cost (F)
                    OpenList.Add(neighbor);                                                                             // add the node to the open list
                }
            }
        }

        //private void checkLists()
        //{
        //    if (CurrentNode == TargetNode)
        //        return;

        //    // reset the flags before evaluating lists
        //    isOnOpenList = false;
        //    isOnClosedList = false;

        //    // check to see if the current node is on the open list
        //    foreach (Node openNode in OpenList)
        //    {
        //        if (CurrentNode == openNode)
        //            isOnOpenList = true;
        //    }

        //    // check to see if the current node is on the closed list
        //    foreach (Node closedNode in ClosedList)
        //    {
        //        if (CurrentNode == closedNode)
        //            isOnClosedList = true;
        //    }

        //    // if the current node is not on either list
        //    // then the current node's neighbors must have their parent node set to the current node
        //    // in other words, current node's neighbors are the children nodes
        //    if (!isOnClosedList && !isOnOpenList)
        //    {
        //        // first we must add the current node to the closed list
        //        ClosedList.Add(CurrentNode);

        //        // next we need to add nodes to the open list
        //        foreach (Node node in CurrentNode.AdjacentNodes)
        //        {
        //            OpenList.Add(node);                                                 // add the node to the open list
        //            node.ParentNode = CurrentNode;                                      // set the node's parent to the current node
        //            node.MovementCost = calculateMovementCost(CurrentNode, node);       // calculate the movement cost from one node to another
        //            node.TotalCost = node.MovementCost + node.Heuristic;                // calculate the F value (F = G + H)
        //        }
        //    }

        //    // check open list for reparenting
        //    foreach (Node node in OpenList)
        //    {
        //        int potentialGScore = CurrentNode.MovementCost + node.MovementCost;
        //        if (potentialGScore < node.MovementCost)
        //        {
        //            // reparent
        //            node.ParentNode = CurrentNode;
        //        }
        //    }

        //    // search for the lowest F value
        //    CurrentNode = getSmallestFNode();

        //    // move the smallest F value node from the open list to the closed list
        //    OpenList.Remove(CurrentNode);
        //    ClosedList.Add(CurrentNode);

        //    //checkLists();
        //}

        // iterates through each node in the open list and returns the one with the smallest total cost (the F value) //todo

        private Node getSmallestFNode()
        {
            int lowestVal = int.MaxValue;           // this variable should be initialized to the largest value possible
            Node smallestFNode = null;              // we will use this to return the smallest node

            foreach (Node node in OpenList)
            {
                if (node.TotalCost < lowestVal)
                {
                    lowestVal = node.TotalCost;     // set the lowest value to the nodes F value (total cost)
                    smallestFNode = node;           // set the smallest node to potentially be returned
                }
            }

            return smallestFNode;
        }

        // update each node
        private void updateNodeList(GameTime gameTime, Player player, BaseGameEntity crosshair, List<Wall> wallList)
        {
            foreach (Node node in NodeList)
            {
                node.Update(gameTime, player, crosshair, wallList);
            }
        }

        //// update the target node
        //private void updateTargetNode(BaseGameEntity crosshair)
        //{
        //    //nodeNeedsUpdate = false;

        //    foreach (Node node in NodeList)
        //    {
        //        // check for target node
        //        if (node != TargetNode && node.Cell.Contains(new Point((int)crosshair.Position.X, (int)crosshair.Position.Y)))
        //        {
        //            TargetNode = node;
        //            //nodeNeedsUpdate = true;
        //        }
        //    }
        //}

        //// update the current node
        //private void updateCurrentNode(Player player)
        //{
        //    CurrentNode = NodeList[0];
        //    //foreach (Node node in NodeList)
        //    //{
        //    //    // check for current node
        //    //    if (node.Cell.Contains(new Point((int)player.Position.X, (int)player.Position.Y)))
        //    //    {
        //    //        CurrentNode = node;
        //    //    }
        //    //}
        //}

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
            if (currentNode.id == (targetNode.id + 1) || currentNode.id == (targetNode.id - 1) ||   // left/right
                currentNode.id == (targetNode.id + 16) || currentNode.id == (targetNode.id - 16))   // top/bottom
                return 10;
            
            // the node is at a diagonal and should have a movement cost of 14
            if (currentNode.id == (targetNode.id - 17) || currentNode.id == (targetNode.id - 15) || // bottom right/left
                currentNode.id == (targetNode.id + 17) || currentNode.id == (targetNode.id + 15))   // top right/left
                return 14;

            // no movement cost because the nodes are the same (or an error occured)
            return 0;
        }

        // calculate the F (total) cost for A*
        private int calculateTotalCost(Node node)
        {
            return (node.MovementCost + node.Heuristic);    // F = G + H
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
                foreach (Node neighbor in NodeList)
                {
                    // check left, right, top, and bottom nodes
                    if ((node != neighbor) && (!node.Equals(neighbor)) && (neighbor.id == (node.id + 1) || neighbor.id == (node.id - 1) ||  // left/right
                        neighbor.id == (node.id + numHorizNodes) || neighbor.id == (node.id - numHorizNodes) ||                             // top/bottom
                        neighbor.id == (node.id - (numHorizNodes + 1)) || neighbor.id == (node.id - (numHorizNodes - 1)) ||                 // bottom right/left
                        neighbor.id == (node.id + (numHorizNodes + 1)) || neighbor.id == (node.id + (numHorizNodes - 1))))                  // top right/left
                    {
                        bool found = false;     // does the adjacent node already exist?

                        // check the list of adjacent nodes
                        foreach (Node adjacentNode in node.AdjacentNodes)
                        {
                            if (neighbor.id == adjacentNode.id)
                            {
                                found = true;
                            }
                        }

                        // add an adjacent node if it wasn't already in the list
                        if (!found && node.Active && neighbor.Active)
                        {
                            if (!(node.id % numHorizNodes == 0 && ((neighbor.id == (node.id - 1) || neighbor.id == (node.id + (numHorizNodes - 1)) || neighbor.id == (node.id - (numHorizNodes + 1)))) || // special left case
                                (node.id % numHorizNodes == (numHorizNodes - 1) && ((neighbor.id == (node.id + 1)) || neighbor.id == (node.id + (numHorizNodes + 1)) || neighbor.id == (node.id - (numHorizNodes - 1)))) || // special right case
                                (node.id < numVertNodes && ((neighbor.id == (node.id - numHorizNodes)) || neighbor.id == (node.id - numHorizNodes - 1) || neighbor.id == (node.id - numHorizNodes + 1))) || // top of grid
                                (node.id >= ((numVertNodes * numHorizNodes) - numHorizNodes) && (neighbor.id == (node.id + numHorizNodes) && neighbor.id == (node.id + numHorizNodes - 1) || neighbor.id == (node.id + numHorizNodes + 1))))) // bottom of grid
                            node.AdjacentNodes.Add(neighbor);
                        }

                             //&& 
                        //   (!(node.id % numHorizNodes == 0 && neighbor.id == (node.id - 1) && neighbor.id == (node.id + (numHorizNodes - 1)) && neighbor.id == (node.id - (numHorizNodes + 1))) || // special left case
                        //   !(node.id % numHorizNodes == numHorizNodes - 1 && neighbor.id == (node.id + 1)) && neighbor.id == (node.id + (numHorizNodes + 1)) && neighbor.id == (node.id - (numHorizNodes - 1)) || // special right case
                        //   !(node.id < numVertNodes && neighbor.id == (node.id - numHorizNodes) && neighbor.id == (node.id - numHorizNodes - 1) && neighbor.id == (node.id - numHorizNodes + 1)) || // top of grid
                        //   !(node.id >= ((numVertNodes * numHorizNodes) - numHorizNodes) && neighbor.id == (node.id + numHorizNodes) && neighbor.id == (node.id + numHorizNodes - 1) && neighbor.id == (node.id + numHorizNodes + 1)))) // bottom of grid

                        // remove an inactive node
                        else if (found && !node.Active || !neighbor.Active)
                            node.AdjacentNodes.Remove(neighbor);
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            if (found && TargetNode != StartNode)
            {
                Node node = TargetNode; // start at the final node and work back to the start
                do
                {
                    DrawingHelper.DrawFastLine(node.Position, node.ParentNode.Position, Color.White);
                    node.Color = Color.Purple;
                    node = node.ParentNode;
                } while (node.ParentNode != null && node != null);
            }
        }
    }
}
