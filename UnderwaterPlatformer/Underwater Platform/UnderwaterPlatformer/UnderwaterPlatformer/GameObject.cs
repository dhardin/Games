using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace UnderwaterPlatformer
{
    abstract class GameObject
    {
        public Vector2 Position { get; set; }
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

            if (!collisionDepth.X.Equals(0) || !collisionDepth.Y.Equals(0))
            {
                Console.Write(true);
            }
            //solve the collision along the shallow axis
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
