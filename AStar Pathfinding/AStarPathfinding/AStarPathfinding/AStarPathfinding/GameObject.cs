using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStarPathfinding
{
    class GameObject
    {
        // Position of the GameObject relative to the upper left side of the screen
        public Vector2 position;

        //Origin of the GameObject
        public Vector2 origin;

        //Draw Origin of the GameObject
        private Vector2 drawOrigin;

        public Vector2 DrawOrigin
        {
            get
            {
                return drawOrigin;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                drawOrigin = position + origin;
            }
        }
    }
}
