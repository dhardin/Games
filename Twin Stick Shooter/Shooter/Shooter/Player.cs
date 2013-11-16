using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class Player : GameObject
    {
        // Animation representing the player
        public Texture2D PlayerTexture;

        //Player's orignal orgin
        private Vector2 origin;

        //Current rotation of player
        private float rotationAngle;

        //full speed of the ship
        float fullSpeed = 320f;

        //maximum length of velocity of ship
        float velocityMaximum;

        float dragPerSecond = 0.9f;

        //number of radians the ship can turn when left stick is pressed all the way
        float rotationRadiansPerSecond;

        // State of the player
        public bool Active;

        // Amount of hit points that player has
        public int Health;
        Vector2 velocity;
        
        // Get the width of the player ship
        public int Width
        {
            get { return PlayerTexture.Width; }
        }
  
        public Vector2 Origin
        {
            get { return origin;}
            set { origin = value;}
        }
        public float RotationAngle
        {
            get { return rotationAngle; }
            set { rotationAngle = value; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return PlayerTexture.Height; }
        }

        public float RotationRadiansPerSecond
        {
            get { return rotationRadiansPerSecond; }
            set { rotationRadiansPerSecond = value; }
        }

        public float FullSpeed
        {
            get { return fullSpeed; }
            set { fullSpeed = value; }
        }

        public float VelocityMaximum
        {
            get { return velocityMaximum; }
            set { velocityMaximum = value; }
        }

        public float DragPerSecond
        {
            get { return dragPerSecond; }
            set { dragPerSecond = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                if ((value.X == Single.NaN) || (value.Y == Single.NaN))
                {
                    throw new ArgumentException("Velocity was NaN");
                }
                velocity = value;
            }
        }
        public void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;

            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            //set starting rotation angle of player
            rotationAngle = 0f;

            //max rotation speed of ship with left joystick
            rotationRadiansPerSecond = 15f;

            //full speed of ship
            fullSpeed = 320f;

            //sourceRectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);

            velocityMaximum = 320f;

            dragPerSecond = 0.9f;

            velocity = Vector2.Zero;

            //set orgin position of player
            origin.X = texture.Width / 2;
            origin.Y = texture.Height / 2;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
        }

        // Draw the player
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerTexture, Position, null, Color.White, RotationAngle,
                Origin, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
