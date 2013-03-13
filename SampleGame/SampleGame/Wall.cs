using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Drawing;

namespace SampleGame
{
    public class Wall : BaseGameEntity
    {
        public Wall()
        {
            //Bounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width / 2, Texture.Height / 2);
        }

        public virtual void Update(GameTime gametime)
        {
            
        }

        public virtual void Draw(SpriteBatch sprites, SpriteFont font1)
        {
            DrawingHelper.DrawRectangle(Bounds, Color.Purple, true);
            //sprites.Draw(Texture, Position - Origin, Color);
        }
    }
}
