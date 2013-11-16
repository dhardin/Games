using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStarPathfinding
{
    /// <summary>
    /// A physical representation of the map.
    /// </summary>
    public class Map
    {
        private int[,] layout = new int[,]
        {
            { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0}, 
            { 1, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 1},
            { 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0}, 
            { 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1}, 
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1}, 
            { 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 2, 2, 2, 2, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0}, 
            { 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 2, 2, 2, 3, 3, 3, 2, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0}, 
            { 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 0, 2, 3, 3, 3, 3, 3, 2, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 0}, 
            { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 2, 2, 3, 3, 2, 3, 3, 2, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1}, 
            { 0, 1, 0, 4, 4, 0, 0, 0, 1, 2, 3, 3, 3, 3, 2, 2, 2, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0}, 
            { 0, 1, 0, 4, 0, 0, 1, 0, 1, 2, 3, 2, 2, 2, 0, 0, 1, 0, 1, 4, 0, 1, 0, 1, 1, 0, 1, 0, 1}, 
            { 0, 1, 0, 4, 4, 0, 1, 0, 1, 2, 2, 1, 0, 1, 1, 0, 1, 0, 1, 4, 0, 1, 1, 0, 1, 0, 1, 1, 0}, 
            { 0, 0, 0, 0, 4, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 0, 4, 4, 4, 0, 1, 1, 0, 0, 0, 0, 1}, 
            { 0, 1, 1, 4, 4, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 4, 4, 4, 4, 1, 1, 1, 1, 0, 1, 1, 1, 1}, 
            { 0, 1, 0, 1, 4, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1}, 
            { 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 4, 0, 1, 0, 0, 1, 0, 1, 1, 0}, 
            { 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1}, 
            { 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0}, 
            { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1}, 
        };

        public Map(int[,] map)
        {
            layout = map;
        }

        private List<Texture2D> textures;

        /// <summary>
        /// The width of the map.
        /// </summary>
        public int Width
        {
            get { return layout.GetLength(1); }
        }
        /// <summary>
        /// The height of the map.
        /// </summary>
        public int Height
        {
            get { return layout.GetLength(0); }
        }

        public int GetTextureWidth()
        {
            return textures[0].Width;
        }

        public int GetTextureHeight()
        {
            return textures[0].Height;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Map()
        {

        }

        /// <summary>
        /// Sets the textures for the map to draw.
        /// </summary>
        public void SetTextures(List<Texture2D> textures)
        {
            this.textures = textures;
        }

        /// <summary>
        /// Returns the tile index for the given cell.
        /// </summary>
        public int GetIndex(int cellX, int cellY)
        {
            if (cellX < 0 || cellX > Width - 1 || cellY < 0 || cellY > Height - 1)
                return 0;

            return layout[cellY, cellX];
        }

        public int GetLandType(int x, int y)
        {
            return layout[y, x];
        }



        /// <summary>
        /// Draws the map.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (textures == null)
            {
                return;
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = layout[y, x];

                    spriteBatch.Draw(textures[index], new Vector2(x, y) 
                        * Global.TileSize, Color.White);
                }
            }
        }
    }
}
