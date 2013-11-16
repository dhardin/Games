using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Extensions;

namespace UnderwaterPlatformer
{
    class WorldObject : GameObject
    {
        private Rectangle sourceRect;
        private Vector2 spriteAnchor; 

        public WorldObject(Texture2D someTexture, Vector2 pos)
        {
            base.Texture = someTexture;
            base.Position = pos;
            sourceRect = new Rectangle((int)pos.X, (int)pos.Y, someTexture.Width, someTexture.Height);
            spriteAnchor = sourceRect.GetAnchor(AnchorType.BottomCenter);
        }
        

        public override void Update(GameTime gameTime)
        {
            return;
        }
        public override Rectangle CollsionBounds
        {
            get { return new Rectangle((int)(Position.X - Texture.Width * 0.5f), (int)(Position.Y - Texture.Height + 24), Texture.Width, Texture.Height); }
            
        }

        public override void Draw(DrawContext ctx)
        {
           ctx.Batch.Draw(Texture, Position + ctx.Offset, sourceRect, Color.White, 0.0f, spriteAnchor, 1.0f, SpriteEffects.None, 0.0f);
        }

    }
}
