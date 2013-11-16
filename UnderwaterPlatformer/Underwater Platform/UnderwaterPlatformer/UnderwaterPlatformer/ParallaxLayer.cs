using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UnderwaterPlatformer
{
    class ParallaxLayer
    {
        public Vector2 Position;
        public Vector2 MoveSpeed;
        private List<Texture2D> Textures = new List<Texture2D>();
        private List<int> TextureIndexes = new List<int>();
        public bool Alive = true;
        private int TotalWidth = 0;
        public int CurrentTextureIndex = 0;
        //Adds a specified Texture to the texture list
        public void AddTexture(Texture2D texture)
        {
            Textures.Add(texture);
        }
        //adds the texture number from the texture list for use within the level
        public void AddTextureIndex(int i)
        {
            TextureIndexes.Add(i);
        }
        //adds a random texture from the Textures list to the TexturesIndexes for use within the level
        public void AddRandomTexture()
        {
                Random random = new Random();
                TextureIndexes.Add(random.Next(Textures.Count()));
        }
        //finds whether the visible width of the level background layer is running out
        public bool IsOutOfLayers(float ScreenWidth)
        {
            TotalWidth = 0;
            foreach (int i in TextureIndexes)
            {
                TotalWidth += Textures[i].Width;
            }
            if (Math.Abs(Position.X) > TotalWidth-ScreenWidth)
                return true;
            else
                return false;
        }
        //returns the X location of each texture in the background
        private float GetX(int count)
        {
            float x = 0.0f;
            for (int i = 0; i < count; i++)
            {
                x += Textures[TextureIndexes[i]].Width;
            }
            return x;
        }
        //public void Update()
        //{
        //    // Update the positions of the background
        //    for (int i = 0; i < positions.Length; i++)
        //    {
        //        // Update the position of the screen by adding the speed
        //        positions[i].X += speed;
        //        // If the speed has the background moving to the left
        //        if (speed <= 0)
        //        {
        //            // Check the texture is out of view then put that texture at the end of the screen
        //            if (positions[i].X <= -texture.Width)
        //            {
        //                positions[i].X = texture.Width * (positions.Length - 1);
        //            }
        //        }

        //        // If the speed has the background moving to the right
        //        else
        //        {
        //            // Check if the texture is out of view then position it to the start of the screen
        //            if (positions[i].X >= texture.Width * (positions.Length - 1))
        //            {
        //                positions[i].X = -texture.Width;
        //            }
        //        }
        //    }
        //}
        //draws
        public void Draw(DrawContext ctx)
        {
            int Count = 0;
            foreach (int i in TextureIndexes)
            {
                Vector2 positions = Vector2.Zero;
                positions.Y = 0;
                positions.X = GetX(Count);
                ctx.Batch.Draw(Textures[i], Position+positions, Color.White);
                Count++;
            }
        }
    }
}
