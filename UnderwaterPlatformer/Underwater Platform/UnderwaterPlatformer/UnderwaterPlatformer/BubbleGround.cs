        using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Sprites;
using Engine.Extensions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace UnderwaterPlatformer
{
    class BubbleGround
    {
        Vector2 Position;
        public AnimatedSpriteFromSpriteSheet _sprite;

        public BubbleGround(Vector2 pos)
        {
            Position = pos;
        }

        public void LoadContent(ContentManager _content)
        {
            var spriteSheet = _content.Load<SpriteSheet>("Ground_Bubble");
            _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, "BG1", 2, 3);
            
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
                    _sprite.Update(gameTime);
        }
        public void Draw(DrawContext ctx)
        {
            _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White);
        }
    }

}