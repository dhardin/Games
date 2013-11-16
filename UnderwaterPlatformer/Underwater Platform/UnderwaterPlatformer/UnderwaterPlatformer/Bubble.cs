using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Engine.Sprites;


namespace UnderwaterPlatformer
{
    enum BubbleType
    {
        SmallBubble,
        MediumBubble,
        LargeBubbe
    }
    class Bubble : GameObject
    {
        Texture2D bubbleTexture;
        private int bubbleType;
        public Vector2 bubbleSpawn;
        string bubbleName;

        public Bubble(BubbleType bubble, Vector2 pos)
        {
            switch (bubble)
            {
                case BubbleType.MediumBubble:
                    bubbleName = "bubble_medium";
                    break;
                case BubbleType.LargeBubbe:
                    bubbleName = "bubble_large";
                    break;
                default:
                    break;
            }
            Position = pos;
            bubbleSpawn = pos;
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent(ContentManager _content)
        {

            bubbleTexture = _content.Load<Texture2D>(@"Bubbles\" + bubbleName);
        }

        public override Microsoft.Xna.Framework.Rectangle CollsionBounds
        {
            get
            {
                return new Rectangle((int)(Position.X), (int)(Position.Y), bubbleTexture.Width, bubbleTexture.Height);
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Vector2 tempPos = Position;

            tempPos.Y -= 0.35f;

            Position = tempPos;
        }

        public override void Draw(DrawContext ctx)
        {

            ctx.Batch.Draw(bubbleTexture, Position + ctx.Offset, Color.FromNonPremultiplied(255, 255, 255, (int)(150)));

        }
    }
}
