// Nuke.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Content;
using Engine.Extensions;

namespace Topgun
{
    class Shield
    {
        #region Fields
        // Image representing the Projectile
        public Texture2D Texture;

        // Position of the Nuke relative to the upper left side of the screen
        public Vector2 Position;

        // State of the Shield
        public bool Active;

        // The amount of damage the projectile can inflict to an enemy
        public int Damage;

        private const float scale = 0.15f;

        private Rectangle collisionBounds;

        #endregion

        #region Properties

        // Get the width of the projectile ship
        public int Width
        {
            get { return Texture.Width; }
        }

        // Get the height of the projectile ship
        public int Height
        {
            get { return Texture.Height; }
        }

        public Rectangle CollisionBounds
        {
            get
            {
                return collisionBounds; 
            }
            set
            {
                collisionBounds = value;
            }
        }
        #endregion
        public Shield()
        {
            Initialize(Position);
        }


        public void Initialize(Vector2 position)
        {
            Texture = ScreenManager.Instance.Game.Content.Load<Texture2D>(@"Projectile\NukeWave");

            collisionBounds = new Rectangle((int)(Position.X - Width * 0.5 * scale), (int)(Position.Y - Width * 0.5 * scale), (int)(Texture.Width * scale), (int)(Texture.Height * scale));
           
            Position = position;

            Active = true;
        }

        #region Update and Draw
        public void Update(Vector2 pos)
        {
            Position = pos;
            if (Active)
            {

                CollisionBounds = new Rectangle((int)(Position.X - Width * 0.5 * scale), (int)(Position.Y - Width * 0.5 * scale), (int)(Texture.Width * scale), (int)(Texture.Height * scale));
            }

        }

        public void Draw(DrawContext ctx)
        {
            if (Active)
            {
                ctx.Batch.Draw(Texture, Position + ctx.Offset, null, Color.LightBlue, 0,
                new Vector2(Width * 0.5f, Height * 0.5f), scale, SpriteEffects.None, 0f);
            }
        }
        #endregion
    }
}
