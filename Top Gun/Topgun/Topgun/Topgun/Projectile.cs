// Projectile.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Content;
using Engine.Extensions;

namespace Topgun
{
    enum ProjectileType
    {
        Standard,
        Rapid,
        Spread,
        Heavy,
        Orb
    }
    class Projectile
    {
        #region Fields
        // Image representing the Projectile
        public Texture2D Texture;

        // Position of the Projectile relative to the upper left side of the screen
        public Vector2 Position;

        public Vector2 firePosition;

        // State of the Projectile
        public bool Active;

        // The amount of damage the projectile can inflict to an enemy
        public int Damage;

        private float rotationAngle;

        public RotatedRectangle rotatedCollisionBox;

        private ProjectileType projectileType;

        // Determines how fast the projectile moves
        public float projectileMoveSpeed;

        private const int MAX_PROJECTILE_SPEED_MULTIPLYER = 4;

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

        public ProjectileType ProjectileType
        {
            get { return projectileType; }
        }

        public Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            }
        }
#endregion
        public Projectile(float angle, ProjectileType projectile)
        {
            
          
            rotationAngle = angle;
            firePosition.X = (float)Math.Cos(rotationAngle);
            firePosition.Y = (float)Math.Sin(rotationAngle);

            Initialize(firePosition, projectile);

        }
        

        public void Initialize(Vector2 position, ProjectileType projectile_Type)
        {
            projectileType = projectile_Type;
            switch (projectile_Type)
            {
                case ProjectileType.Standard:
                    Texture = ScreenManager.Instance.Game.Content.Load<Texture2D>(@"Projectile\standard");
                    Damage = 5;
                    projectileMoveSpeed = 8.0f * MAX_PROJECTILE_SPEED_MULTIPLYER;
                    break;
                case ProjectileType.Rapid:
                    Texture = ScreenManager.Instance.Game.Content.Load<Texture2D>(@"Projectile\rapid");
                    Damage = 2;
                    projectileMoveSpeed = 9.0f * MAX_PROJECTILE_SPEED_MULTIPLYER;
                    break;
                case ProjectileType.Spread:
                    Texture = ScreenManager.Instance.Game.Content.Load<Texture2D>(@"Projectile\spread");
                    Damage = 5;
                    projectileMoveSpeed = 5.0f * MAX_PROJECTILE_SPEED_MULTIPLYER;
                    break;
                case ProjectileType.Heavy:
                    Texture = ScreenManager.Instance.Game.Content.Load<Texture2D>(@"Projectile\heavy");
                    Damage = 10;
                    projectileMoveSpeed = 4.0f * MAX_PROJECTILE_SPEED_MULTIPLYER;
                    break;
                case ProjectileType.Orb:
                    Texture = ScreenManager.Instance.Game.Content.Load<Texture2D>(@"Projectile\orb");
                    Damage = 5;
                    projectileMoveSpeed = 2.0f * MAX_PROJECTILE_SPEED_MULTIPLYER;
                    break;
                default:
                    break;
              
           
            }

            rotatedCollisionBox = new RotatedRectangle(CollisionBounds, rotationAngle);
   
            Position = position;

            Active = true;

            
        }

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
        #region Update and Draw
        public void Update()
        {
            if (Active)
            {
                Position.X += (projectileMoveSpeed * firePosition.X);
                Position.Y += (projectileMoveSpeed * firePosition.Y);

                rotatedCollisionBox.ChangePosition(new Vector2(Position.X - rotatedCollisionBox.Width / 2, Position.Y));
            }
            
        }

        public void Draw(DrawContext ctx)
        {
            if (Active)
            {
                ctx.Batch.Draw(Texture, Position + ctx.Offset, null, new Color(0.0f, 0.0f, 0.0f, 0.85f), rotationAngle + (float)(Math.PI * 0.5f),
               new Vector2(Width / 2, Height / 2), 1.1f, SpriteEffects.None, 0f);
                ctx.Batch.Draw(Texture, Position + ctx.Offset, null, Color.White, rotationAngle + (float)( Math.PI * 0.5f),
                new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
               
            }
        }
        #endregion
    }
}
