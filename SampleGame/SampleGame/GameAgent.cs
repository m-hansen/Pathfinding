using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame
{
    public class GameAgent : MovingEntity
    {
        //public float Rotation = 0.0f;                   // how far the object has rotated
        //public float RotationSpeed = 1.0f;              // how fast the object should rotate
        //public float ZLayer;                            // depth of object
        //public Color Color = Color.White;               // max RGB of the image to draw
        public int Type;                                // type of agent (wall, etc)

        public int TotalFrames { get; private set; }    // the total frames in the image
        public TimeSpan AnimationInterval;              // how often the frames are changed

        private Rectangle[] rects;                      // rectangle array of each sub image to draw within the sprite sheet
        private int currentFrame;                       // which frame of the image we're currently on
        private TimeSpan animElapsed;                   // how long it's been since we last moved frames

        // helper property for getting the width and height of the object.
        public int FrameWidth { get { return rects == null ? Texture.Width : rects[0].Width; } }
        public int FrameHeight { get { return rects == null ? Texture.Height : rects[0].Height; } }

        // the agent's current bounding rectangle, used for collision detection
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle
                (
                    (int)(Position.X - Origin.X * Scale),
                    (int)(Position.Y - Origin.Y * Scale),
                    (int)(rects == null ? Texture.Width * Scale  : rects[0].Width * Scale),
                    (int)(rects == null ? Texture.Height * Scale : rects[0].Height * Scale)
                );
            }
        }
        
        // Load the texture for the agent from the content pipeline
        public virtual void LoadContent(ContentManager contentManager, string assetName, Rectangle? firstRect = null, int frames = 1, bool horizontal = true, int space = 0)
        {
            // loading the image for the object
            Texture = contentManager.Load<Texture2D>(assetName);

            // setting the total number of frames within the image
            TotalFrames = frames;

            // setting the origin to the center of the object
            Origin = new Vector2(Texture.Width / (2 * (horizontal ? frames : 1)), Texture.Height / (2 * (horizontal ? 1 : frames)));

            // if the image is a sprite sheet, set each rectangle of the object
            if (firstRect.HasValue)
            {
                rects = new Rectangle[frames];

                for (int i = 0; i < frames; i++)
                {
                    rects[i] = new Rectangle
                    (
                        firstRect.Value.Left + (horizontal ? (firstRect.Value.Width + space) * i : 0),
                        firstRect.Value.Top + (horizontal ? 0 : (firstRect.Value.Height + space) * i),
                        firstRect.Value.Width,
                        firstRect.Value.Height
                    );
                }
            }
        }

        public virtual void Update(GameTime gametime) // TODO make override instead of virtual?
        {
            // if the object is active on the screen
            if (Active)
            {
                // if the image is a sprite sheet 
                // and if enough time has passed to where we need to move to the next frame
                if (TotalFrames > 1 && (animElapsed += gametime.ElapsedGameTime) > AnimationInterval)
                {
                    if (++currentFrame == TotalFrames)
                        currentFrame = 0;               

                    // move back by the animation interval (in miliseconds)
                    animElapsed -= AnimationInterval;
                }

                // move the object by the velocity
                Position += Velocity;
            }
        }

        // Render the sprite to the screen
        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            // whether the object is currently being drawn on the screen
            if (Active)
            {
                sprites.Draw(Texture, Position, rects == null ? null : (Rectangle?)rects[currentFrame], Color, Rotation, Origin, Scale, SpriteEffects.None, ZLayer);
            }
        }
    }
}
