using System;

namespace AStarPathfinding
{
    class Position
    {
        int x;
        int y;

        public Position(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
