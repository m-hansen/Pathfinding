using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame
{
    public class BaseGameEntity
    {
        public enum EntityType
        {
            Player = 0,
            Enemy = 1,
            Crosshair = 2,
            Waypoint = 3
        };

        public EntityType Type;
        public Texture2D Texture { get; protected set; }// the image set for the object
        public Vector2 Position;                        // the current position of the object
        public Vector2 Origin;                          // origin of object: currently set to the middle of the image
        public bool Active = true;                      // whether the object is active on the screen
        public float Rotation = 0.0f;                   // how far the object has rotated
        public float RotationSpeed = 1.0f;              // how fast the object should rotate
        public float Scale = 1;                         // how much to scale the image
        public float ZLayer;                            // depth of object
        public Color Color = Color.White;               // max RGB of the image to draw
        public Rectangle Bounds;

        // Load the texture for the agent from the content pipeline
        public virtual void LoadContent(ContentManager contentManager, string assetName)
        {
            // loading the image for the object
            Texture = contentManager.Load<Texture2D>(assetName);

            // setting the origin to the center of the object
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            
            Bounds = new Rectangle((int)(Position.X - Origin.X * Scale), (int)(Position.Y - Origin.Y * Scale), (int)(Texture.Width * Scale), (int)(Texture.Height * Scale));
        }

        public virtual void Update(GameTime gametime)
        {
        }

        // Render the sprite to the screen
        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            // whether the object is currently being drawn on the screen
            if (Active)
            {
                sprites.Draw(Texture, Position - Origin, Color);
            }
        }
    }
}
