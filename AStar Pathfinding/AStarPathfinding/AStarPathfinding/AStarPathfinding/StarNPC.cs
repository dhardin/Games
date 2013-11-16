using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AStarPathfinding
{
    class StarNPC : GameObject
    {
        // Animation representing the npc
        public Texture2D NPCTexture;

        // State of the npc
        public bool Active;

        // Get the width of the npc ship
        public int Width
        {
            get { return NPCTexture.Width; }
        }

        // Get the height of the npc ship
        public int Height
        {
            get { return NPCTexture.Height; }
        }
        public void Initialize(Texture2D texture, Vector2 position)
        {
            NPCTexture = texture;

            // Set the starting position of the npc around the middle of the screen and to the back
            Position = position;

            origin.X = NPCTexture.Width / 2;
            origin.Y = NPCTexture.Height / 2;

            // Set the npc to be active
            Active = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(NPCTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }

}
