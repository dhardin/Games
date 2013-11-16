using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace PolarRecall
{
    abstract class GameObject
    {
        public Vector2 Position;
        public Texture2D Texture { get; set; }

        public abstract Rectangle CollsionBounds { get; }
        public Vector2 Velocity { get; set; }

        public Color Color { get; set; }

        /// <summary>
        /// Maximum turn angle in radians per second
        /// </summary>
        public float MaxTurnSpeed { get; set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(DrawContext ctx);

        public bool CollidesWith(Rectangle rectangle, out Vector2 newPosition)
        {
            Vector2 collisionDepth = CollsionBounds.GetIntersectionDepth(rectangle);
            newPosition = Position;

            if (Math.Abs(collisionDepth.X) < Math.Abs(collisionDepth.Y))
                newPosition.X += collisionDepth.X;
            else
            {
                newPosition.Y += collisionDepth.Y;
            }

            return collisionDepth != Vector2.Zero;

        }

    }
}
