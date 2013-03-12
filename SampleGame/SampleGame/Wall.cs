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
    class Wall : BaseGameEntity
    {
        public Wall()
        {
            Bounds = new Rectangle();
        }

        public virtual void Update(GameTime gametime)
        {
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width / 2, Texture.Height / 2);
        }
    }
}
