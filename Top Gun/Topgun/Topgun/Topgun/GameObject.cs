using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;
using Engine.Sprites;

namespace Topgun
{
    abstract class GameObject
    {
        public bool isDead { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public Texture2D Texture { get; set; }

        public abstract Rectangle CollisionBounds { get; }
        public Vector2 Velocity { get; set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(DrawContext ctx);

        public bool CollidesWith(Rectangle rectangle, ref Vector2 newPosition)
        {
            Vector2 collisionDepth = RectangleExtensions.GetIntersectionDepth(CollisionBounds, rectangle);

            if (collisionDepth != Vector2.Zero)
            {
                if (Math.Abs(collisionDepth.Y) < Math.Abs(collisionDepth.X))
                    newPosition.Y += collisionDepth.Y;
                else
                    newPosition.X += collisionDepth.X;
            }

            return collisionDepth != Vector2.Zero;

        }

    }
}
