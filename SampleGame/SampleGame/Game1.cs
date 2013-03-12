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
        bool displayDebugInfo = true;                          // flag for debug information
        bool displayGrid = true;                                // flag for grid
        List<Node> nodeList = new List<Node>();
        List<BaseGameEntity> waypoints = new List<BaseGameEntity>();

        int windowWidth = 0;                                    // width of the window
        int windowHeight = 0;                                   // height of the window
        Graph g = new Graph();
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

            // ************ CREATING THE WALLS FOR THE ASSIGNMENT ********* //

            int defaultWalls = 3;

            for (int i = 0; i < defaultWalls; i++)
            {
                agentAIList.Add(new GameAgent()
                {
                    Type = (int)Enums.AgentType.Wall
                });
            }

            // ********** END CREATING THE WALLS FOR THE ASSIGNMENT ******* //

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

            // ************ WAYPOINT LIST ************ //

            int hWaypoints = 18;
            int vWaypoints = 14;

            for (int i = 0; i < hWaypoints; i++)
            {
                for (int j = 0; j < vWaypoints; j++)
                {
                    waypoints.Add(new BaseGameEntity()
                    {
                        Position = new Vector2(i * 50 + 25, j * 50 + 25),
                    });
                }
            }

            foreach (BaseGameEntity waypoint in waypoints)
            {
                waypoint.LoadContent(this.Content, "Images\\waypoint");
            }

            // ************ END WAYPOINT LIST ************ //

            // ************ LOADING THE WALLS FOR THE ASSIGNMENT ********* //

            agentAIList[0].LoadContent(this.Content, "Images\\wall");
            agentAIList[1].LoadContent(this.Content, "Images\\wall");
            agentAIList[2].LoadContent(this.Content, "Images\\wall");

            agentAIList[0].Rotation = MathHelper.PiOver2;

            agentAIList[0].Scale = 1;
            agentAIList[1].Scale = 1;
            agentAIList[2].Scale = 1;

            agentAIList[0].Position = new Vector2(325, 350);
            agentAIList[1].Position = new Vector2(650, 325);
            agentAIList[2].Position = new Vector2(350, 525);

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

            player.Update(gameTime, keyboardStateCurrent, keyboardStatePrevious, mouseStateCurrent, mouseStatePrevious, agentAIList, crosshair, windowWidth, windowHeight);

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

                // Create a new node
                Node node = new Node(new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y));
                node.LoadContent(this.Content, "Images\\waypoint");
                nodeList.Add(node);
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
            Color alphaBlack = Color.Black;
            alphaBlack.A = 240;

            spriteBatch.Begin();                        // begin drawing sprites


            foreach (Node node in g.nodeList)
            {
                node.Draw(this.spriteBatch, font1);
            }

            // draw the grid
            if (displayGrid)
            {
                int gridSpacing = 50;                       // used for size of grid (X by X)

                // draw the vertical lines
                for (int i = 0; i < windowWidth / gridSpacing; i++)
                {
                    DrawingHelper.DrawFastLine(new Vector2(i * gridSpacing, i), new Vector2(i * gridSpacing, i * gridSpacing + windowHeight), Color.Gray);
                }

                // draw the horizontal lines
                for (int i = 0; i < windowHeight / gridSpacing; i++)
                {
                    DrawingHelper.DrawFastLine(new Vector2(i, i * gridSpacing), new Vector2(i * gridSpacing + windowWidth, i * gridSpacing), Color.Gray);
                }

                // draw each waypoint
                foreach (BaseGameEntity waypoint in waypoints)
                {
                    // TODO - waypoint.Draw(this.spriteBatch, font1);
                }
            }

            // Draw each agent
            foreach (GameAgent agent in agentAIList)
            {
                agent.Draw(this.spriteBatch, font1);
            }

            foreach (Node node in nodeList)
            {
                node.Draw(this.spriteBatch, font1);
            }

            crosshair.Draw(this.spriteBatch, font1);    // draw the crosshair

            // *********************** DRAWING TEXT ON THE SCREEN FOR ASSIGNMENT ******************** //

            spriteBatch.DrawString(font1, "Sensor Keys", new Vector2(700, 500), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Rangefinders: P", new Vector2(680, 520), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Agent Sensors: O", new Vector2(670, 540), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Pie-Slice Sensors: I", new Vector2(655, 560), Color.Red, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
           
            // *********************** END DRAWING TEXT ON THE SCREEN FOR ASSIGNMENT ***************** //

            player.Draw(this.spriteBatch, font1);   // draws the player object on the screen

            spriteBatch.End();                          // stop drawing sprites

            spriteBatch.Begin();

            DrawingHelper.DrawRectangle(new Rectangle(windowWidth / 2 - 105, windowHeight - 45, 232, 45), alphaBlack, true);
            spriteBatch.DrawString(font1, "Press F2 to display the grid", new Vector2(windowWidth / 2 - 100, windowHeight - 40), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font1, "Press F3 for debugging information", new Vector2(windowWidth / 2 - 100, windowHeight - 20), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0);   

            if (displayDebugInfo)
            {
                alphaBlack.A = 200;
                DrawingHelper.DrawRectangle(new Rectangle(15, 15, 275, 50), alphaBlack, true);

                spriteBatch.DrawString(font1, "Player Pos: " + player.Position.X + ", " + player.Position.Y, new Vector2(20, 20), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);
                spriteBatch.DrawString(font1, "Player Heading: " + player.Heading.X + ", " + player.Heading.Y, new Vector2(20, 40), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}