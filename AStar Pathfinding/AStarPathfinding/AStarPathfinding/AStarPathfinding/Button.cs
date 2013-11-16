using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AStarPathfinding
{
    class Button : GameObject
    {
        // Animation representing the npc
        public Texture2D buttonTexture;

        // Get the width of the npc ship
        public int Width
        {
            get { return buttonTexture.Width; }
        }

        // Get the height of the npc ship
        public int Height
        {
            get { return buttonTexture.Height; }
        }
        public void Initialize(Texture2D texture, Vector2 position)
        {
            buttonTexture = texture;

            // Set the starting position of the npc around the middle of the screen and to the back
            Position = position;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(buttonTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }

}
