using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AStarPathfinding
{
    class NPC : GameObject
    {
        // Animation(s) representing the NPC
        public List<Animation> NPC_Animation;

        //Current direction - 0 = up, 1 = down, 2 = right, 3 = left
        private int npc_direction;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position;

        // The Origin of the object
        public Vector2 Origin;

        // The state of the Enemy Ship
        public bool Active;

        //The state of units movement - i.e., whether or not they reached their destination
        bool destinationXReached;
        bool destinationYReached;

        public Vector2 endPosition;

         // The speed at which the npc moves
        float npcMoveSpeed;

        float movePenalty;

        // Get the width of the npc
        public int Width
        {
            get { return NPC_Animation[npc_direction].FrameWidth; }
        }

        // Get the height of the npc
        public int Height
        {
            get { return NPC_Animation[npc_direction].FrameHeight; }
        }
        
        //Get the move speed of the NPC
        public float NPC_MoveSpeed
        {
            get { return npcMoveSpeed; }
            set { npcMoveSpeed = value; }
        }
        //Get and Set the direction the NPC is facing (used for animations
        public int NPC_Direction
        {
            get { return npc_direction; }
            set { npc_direction = value; }
        }

        public float NPC_MovePenalty
        {
            get { return movePenalty; }
        }
        public void Move(Vector2 endPoint, int terrainMovePenalty)
        {
            if (terrainMovePenalty > 1)
                movePenalty = terrainMovePenalty * 0.12f;
            else
                movePenalty = -1f;
            if(endPosition != endPoint)
                endPosition = endPoint;
            if (Position.X != endPosition.X)
            {
                destinationXReached = false;
            }
            if (Position.Y != endPosition.Y)
            {
                destinationYReached = false;
            }
        }

       

        public void Initialize(List<Animation> animation, Vector2 position)
        {
            destinationXReached = false;
            destinationYReached = false;

            // Load the npc texture
            NPC_Animation = animation;

            // Set the position of the npc
            Position = position;

            // We initialize the npc to be active so it will be update in the game
            Active = true;

            // Set how fast the npc moves
            npcMoveSpeed = 1f;

            //set how much terrain affects movement
            movePenalty = 0f;
        }


        public void Update(GameTime gameTime)
        {
            //adjust x coordinates
            
            if (!destinationXReached || !destinationYReached)
            {
                if (Position.X > endPosition.X)
                {
                    Position.X = Position.X - npcMoveSpeed + movePenalty;
                    npc_direction = 3;//npc face left
                    destinationXReached = false;
                    if (Position.X <= endPosition.X)
                    {
                        destinationXReached = true;
                        Position.X = endPosition.X;
                    }
                }
                else if (Position.X < endPosition.X)
                {
                    Position.X = Position.X + npcMoveSpeed - movePenalty;
                    npc_direction = 2;//npc face right
                    destinationXReached = false;
                    if (Position.X >= endPosition.X)
                    {
                        destinationXReached = true;
                        Position.X = endPosition.X;
                    }
                }

                //adjust y coordinates
                if (Position.Y > endPosition.Y)
                {
                    Position.Y = Position.Y - npcMoveSpeed + movePenalty;
                    npc_direction = 1;//npc face down
                    destinationYReached = false;
                    if (Position.Y <= endPosition.Y)
                    {
                        destinationYReached = true;
                        Position.Y = endPosition.Y;
                    }
                }
                else if (Position.Y < endPosition.Y)
                {
                    Position.Y = Position.Y + npcMoveSpeed - movePenalty;
                    npc_direction = 0;//npc face up
                    destinationYReached = false;
                    if (Position.Y >= endPosition.Y)
                    {
                        destinationYReached = true;
                        Position.Y = endPosition.Y;
                    }
                }
            }
            
            for (int i = 0; i < NPC_Animation.Count(); i++)
            {
                if (i != npc_direction)
                    NPC_Animation[i].Active = false;
                else
                    NPC_Animation[i].Active = true;
                // Update the position of the Animations
                NPC_Animation[i].Position = Position;

                //update frame time in regards to land type
                //convert move penalty to int
                NPC_Animation[i].AdjustFrameTime = NPC_Animation[i].FrameTime;
                if (movePenalty != -1.0f)
                {
                    long tempPenalty = (long)(movePenalty * 3 * 100);
                    NPC_Animation[i].AdjustFrameTime = (int)tempPenalty;
                }
                // Update Animation
                NPC_Animation[i].Update(gameTime);
            }

            
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            NPC_Animation[npc_direction].Draw(spriteBatch);
        }
    }
}
