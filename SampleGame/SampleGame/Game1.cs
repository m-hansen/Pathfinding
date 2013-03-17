using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Drawing;  // DrawingHelper namespace

namespace SampleGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;                         // manage graphics
        SpriteBatch spriteBatch;                                // the sprite batch
        Player player;                                          // the human controlled player         
        List<GameAgent> agentAIList = new List<GameAgent>();    // list to hold each AI agent
        KeyboardState keyboardStateCurrent;                     // the current state of the keyboard
        KeyboardState keyboardStatePrevious;                    // the previous state of the keyboard
        MouseState mouseStateCurrent;                           // the current state of the mouse
        MouseState mouseStatePrevious;                          // the previous state of the mouse
        SpriteFont font1;                                       // default font (used for debug)
        BaseGameEntity crosshair;                               // crosshair for movement    
        bool displayDebugInfo = true;                           // flag for debug information
        bool displayGrid = true;                                // flag for grid
        List<Wall> wallList = new List<Wall>();                 // list of walls in the world
        Graph navagationGraph;                                  // the navagation graph for the world
        const int CELL_SIZE = 50;                               // the size of each cell in the grid (X by X)

        int windowWidth = 0;                                    // width of the window
        int windowHeight = 0;                                   // height of the window

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;   // height of screen
            graphics.PreferredBackBufferWidth = 800;    // width of screen
            graphics.IsFullScreen = false;              // full screen vs windowed mode 
            Content.RootDirectory = "Content";          // where the game's content is stored

            windowWidth = Window.ClientBounds.Width;
            windowHeight = Window.ClientBounds.Height;
        }

        protected override void Initialize()
        {
            // Initialize the input devices
            this.IsMouseVisible = true;
            keyboardStateCurrent = new KeyboardState();
            mouseStateCurrent = new MouseState();

            // Initialize DrawingHelper
            DrawingHelper.Initialize(GraphicsDevice);

            player = new Player();
            //player.AnimationInterval = TimeSpan.FromMilliseconds(100);          // next frame every 100 miliseconds
            player.Position = new Vector2(windowWidth / 2, windowHeight / 2);   // setting position to center of screen
            player.RotationSpeed = 5.0f;                                        // rotate somewhat quick
            player.Speed = 4.0f;                                                // setting forward - backward speed
            player.InitializeSensors();                                         // initializes all sensors for the player object

            crosshair = new BaseGameEntity();
            crosshair.Position = new Vector2(windowWidth / 2+1, windowHeight / 2+1); // TODO - slight offset to temporarly bypass a bug

            // ************ CREATING THE WALLS FOR THE ASSIGNMENT ********* //

            /*int defaultWalls = 3;

            for (int i = 0; i < defaultWalls; i++)
            {
                agentAIList.Add(new GameAgent()
                {
                    Type = (int)Enums.AgentType.Wall
                });
            }*/

            // ********** END CREATING THE WALLS FOR THE ASSIGNMENT ******* //

            // create the navagation graph
            navagationGraph = new Graph();

            for (int i = 0; i < windowHeight / CELL_SIZE; i++)
            {
                for (int j = 0; j < windowWidth / CELL_SIZE; j++)
                {
                    navagationGraph.addNode(new Vector2(j * CELL_SIZE + CELL_SIZE / 2, i * CELL_SIZE + CELL_SIZE / 2));
                }
            }

            // start at cell size to avoid overlap on first case
            for (int i = CELL_SIZE; i < windowHeight / CELL_SIZE; i++)
            {
                navagationGraph.addNode(new Vector2(i * CELL_SIZE, 0));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // loading the player's image
            player.LoadContent(this.Content, "Images\\player");//, new Rectangle(0, 0, 38, 41), 8);

            // loading the font to display text on the screen
            font1 = Content.Load<SpriteFont>("fonts/Font1");

            crosshair.LoadContent(this.Content, "Images\\crosshair");

            foreach (Node node in navagationGraph.NodeList)
            {
                node.LoadContent(this.Content, "Images\\waypoint");
            }


            // *********************** MAP GENERATION *********************** //

            // add static walls
            List<int> wallNodes = new List<int>();
            for (int i = 52; i <= 63; i++)
                wallNodes.Add(i);
            for (int i = 73; i <= 121; i += 16)
                wallNodes.Add(i);
            for (int i = 132; i <= 180; i += 16)
                wallNodes.Add(i);

            // iterate through each node and check to see if its on the static list of walls
            foreach (Node node in navagationGraph.NodeList)
            {
                foreach (int id in wallNodes)
                {
                    if (node.id == id)
                    {
                        Wall wall = new Wall();
                        wall.LoadContent(this.Content, "Images\\wall");
                        wall.Position = node.Position;
                        wall.Bounds = new Rectangle(
                            (int)(wall.Position.X - wall.Origin.X * wall.Scale), (int)(wall.Position.Y - wall.Origin.Y * wall.Scale), 
                            (int)(wall.Texture.Width), (int)(wall.Texture.Height));
                        wallList.Add(wall);
                    }
                }
            }

            // *********************** END MAP GENERATION *********************** //


            // ************ LOADING THE WALLS FOR THE ASSIGNMENT ********* //

            //agentAIList[0].LoadContent(this.Content, "Images\\wall");
            //agentAIList[1].LoadContent(this.Content, "Images\\wall");
            //agentAIList[2].LoadContent(this.Content, "Images\\wall");

            //agentAIList[0].Rotation = MathHelper.PiOver2;

            //agentAIList[0].Scale = 1;
            //agentAIList[1].Scale = 1;
            //agentAIList[2].Scale = 1;

            //agentAIList[0].Position = new Vector2(325, 350);
            //agentAIList[1].Position = new Vector2(650, 325);
            //agentAIList[2].Position = new Vector2(350, 525);

            //Random rnd = new Random();

            //for (int i = 0; i < agentAIList.Count; i++)
            //{
            //    int randNumb = rnd.Next(1);
            //    agentAIList[i].LoadContent(this.Content, randNumb > 0 ? "Images\\wall" : "Images\\wall1", null, 1, randNumb > 0);
            //    agentAIList[i].Scale = (float)rnd.Next(100) / 50 + 1;
            //    agentAIList[i].Position = new Vector2(rnd.Next(windowWidth), rnd.Next(windowHeight));

            //    int targetIndex = -1;

            //    // making sure the walls aren't out of the zone, intersecting the player, or intersecting other walls
            //    while (targetIndex < i)
            //    {
            //        Rectangle r = agentAIList[i].Bounds;
            //        if (targetIndex < 0 && (r.Left < 0 || r.Top < 0 || r.Left + r.Width > windowWidth || r.Top + r.Height > windowHeight || r.Intersects(player.Bounds)))
            //        {
            //            agentAIList[i].Position = new Vector2(rnd.Next(windowWidth), rnd.Next(windowHeight));
            //        }
            //        else if (targetIndex >= 0 && agentAIList[i].Bounds.Intersects(agentAIList[targetIndex].Bounds))
            //        {
            //            agentAIList[i].Position = new Vector2(rnd.Next(windowWidth), rnd.Next(windowHeight));

            //            targetIndex = 0;
            //        }
            //        else
            //        {
            //            targetIndex++;
            //        }
            //    }
            //}

            // ********* END LOADING THE WALLS FOR THE ASSIGNMENT ******** //
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if ((keyboardStateCurrent.IsKeyUp(Keys.Escape) && keyboardStatePrevious.IsKeyDown(Keys.Escape)) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update keyboard state
            keyboardStatePrevious = keyboardStateCurrent;
            keyboardStateCurrent = Keyboard.GetState();

            // Update mouse state
            mouseStatePrevious = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();

            // Update the player
            player.Update(gameTime, keyboardStateCurrent, keyboardStatePrevious, mouseStateCurrent, mouseStatePrevious, wallList, navagationGraph, agentAIList, crosshair, windowWidth, windowHeight);

            // Update the navagation graph and all of its nodes
            navagationGraph.Update(gameTime, player, crosshair, wallList);

            // check for debug/grid/etc
            if ((keyboardStateCurrent.IsKeyUp(Keys.F3) && keyboardStatePrevious.IsKeyDown(Keys.F3)))
                displayDebugInfo = !displayDebugInfo;
            if ((keyboardStateCurrent.IsKeyUp(Keys.F2) && keyboardStatePrevious.IsKeyDown(Keys.F2)))
                displayGrid = !displayGrid;

            // Create new agent on mouse click
            if (mouseStateCurrent.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton != ButtonState.Pressed)
            {
                // Create a new agent at mouse location
                //GameAgent agent = new GameAgent();
                //agent.LoadContent(this.Content, "Images\\agent");
                //agent.Position = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                //agent.Rotation = 0.0f;
                //agentAIList.Add(agent);
                //agent.Type = (int)Enums.AgentType.NPC;

                //// Create a new node
                //Node node = new Node(new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y));
                //node.LoadContent(this.Content, "Images\\waypoint");
                //navagationGraph.NodeList.Add(node);

                // Create a wall
                Point clickPos = new Point((int)mouseStateCurrent.X, (int)mouseStateCurrent.Y);
                foreach (Node node in navagationGraph.NodeList)
                {
                    // find the node that the mouse clicked
                    if (node.Cell.Contains(clickPos))
                    {
                        Wall wall = new Wall();
                        wall.LoadContent(this.Content, "Images\\wall");
                        wall.Position = node.Position;
                        wall.Bounds = new Rectangle(
                            (int)(wall.Position.X - wall.Origin.X * wall.Scale), (int)(wall.Position.Y - wall.Origin.Y * wall.Scale),
                            (int)(wall.Texture.Width), (int)(wall.Texture.Height));
                        wallList.Add(wall);
                    }
                }
            }

            // Place crosshair at mouse position
            if (mouseStateCurrent.RightButton == ButtonState.Pressed && mouseStatePrevious.RightButton != ButtonState.Pressed)
            {
                crosshair.Position = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);       // clears background to selected color
            Color alphaBlack = Color.Black;             // the color black
            alphaBlack.A = 200;                         // modified alpha channel

            spriteBatch.Begin();                        // begin drawing sprites

            // Draw walls, waypoints, and agents
            foreach (Wall wall in wallList)
            {
                wall.Draw(this.spriteBatch, font1);
            }

            if (displayGrid)
            {
                foreach (Node node in navagationGraph.NodeList)
                {
                    node.Draw(this.spriteBatch, font1);
                }
            }

            foreach (GameAgent agent in agentAIList)
            {
                agent.Draw(this.spriteBatch, font1);
            }

   

            crosshair.Draw(this.spriteBatch, font1);    // draw the crosshair

            // *********************** DRAWING TEXT ON THE SCREEN FOR ASSIGNMENT ******************** //

            //spriteBatch.DrawString(font1, "Sensor Keys", new Vector2(700, 500), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font1, "Rangefinders: P", new Vector2(680, 520), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font1, "Agent Sensors: O", new Vector2(670, 540), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font1, "Pie-Slice Sensors: I", new Vector2(655, 560), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
           
            // *********************** END DRAWING TEXT ON THE SCREEN FOR ASSIGNMENT ***************** //

            player.Draw(this.spriteBatch, font1);       // draws the player object on the screen
            navagationGraph.Draw(this.spriteBatch, font1);
            spriteBatch.End();                          // stop drawing sprites

            // Draw debugging and info on top of everything else
            spriteBatch.Begin();

            DrawingHelper.DrawRectangle(new Rectangle(windowWidth / 2 - 105, windowHeight - 45, 232, 45), alphaBlack, true);
            spriteBatch.DrawString(font1, "Press F2 to display the grid", new Vector2(windowWidth / 2 - 100, windowHeight - 40), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Press F3 for debugging information", new Vector2(windowWidth / 2 - 100, windowHeight - 20), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);   

            if (displayDebugInfo)
            {
                string text;

                // the background for debugging information
                DrawingHelper.DrawRectangle(new Rectangle(15, 15, windowWidth - 30, windowHeight - 30), alphaBlack, true);

                // title for the debug screen
                spriteBatch.DrawString(font1, "DEBUG", new Vector2(windowWidth / 2 - 50, 20), Color.Red, 0.0f, Vector2.Zero, 1.5f, SpriteEffects.None, 1);

                // display instructions for sensors
                spriteBatch.DrawString(font1, "Sensor Keys", new Vector2(700, 480), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font1, "Rangefinders: P", new Vector2(680, 500), Color.DarkOrange, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font1, "Agent Sensors: O", new Vector2(670, 520), Color.DarkOrange, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font1, "Pie-Slice Sensors: I", new Vector2(655, 540), Color.DarkOrange, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font1, "Path Node Sensors: U", new Vector2(645, 560), Color.DarkOrange, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

                // display information about the player's location
                spriteBatch.DrawString(font1, "Player Pos: " + player.Position.X + ", " + player.Position.Y, new Vector2(20, 20), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);
                spriteBatch.DrawString(font1, "Player Heading: " + player.Heading.X + ", " + player.Heading.Y, new Vector2(20, 40), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);

                // display info about the current node and its neighbors
                text = "Current Node ID: " + navagationGraph.CurrentNode.id + "       Neighbors: ";
                foreach (Node adjNode in navagationGraph.CurrentNode.AdjacentNodes)
                    text += adjNode.id + ", ";
                spriteBatch.DrawString(font1, text, new Vector2(20, 80), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);

                // display info about the target node and its neighbors
                text = "Target Node ID: " + navagationGraph.TargetNode.id + "       Neighbors: ";
                foreach (Node adjNode in navagationGraph.TargetNode.AdjacentNodes)
                    text += adjNode.id + ", ";
                spriteBatch.DrawString(font1, text, new Vector2(20, 100), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);

                // display information about the open list for A*
                text = "Open List: ";
                foreach (Node node in navagationGraph.OpenList)
                    text += node.id + ", ";
                spriteBatch.DrawString(font1, text, new Vector2(20, 140), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);

                // display information about the closed list for A*
                text = "Closed List: ";
                foreach (Node node in navagationGraph.ClosedList)
                    text += node.id + ", ";
                spriteBatch.DrawString(font1, text, new Vector2(20, 160), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}