using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PolarRecall
{
    class DrawContext
    {
        public SpriteBatch Batch { get; set; }
        public GraphicsDevice Device { get; set; }
        public Texture2D Blank { get; set; }
        public GameTime Time { get; set; }
        public Vector2 Offset { get; set; }
    }
}
