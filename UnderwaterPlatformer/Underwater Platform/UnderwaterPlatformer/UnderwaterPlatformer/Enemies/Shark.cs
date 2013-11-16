using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Sprites;
using Engine.Extensions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UnderwaterPlatformer.Enemies
{
    class Shark : GameObject
    {
        int MoveDirection = 0;
        int MoveTimer = 100;
        public AnimatedSpriteFromSpriteSheet _sprite;

        public void LoadContent(ContentManager _content, int _moveDirection)
        {
            var spriteSheet = _content.Load<SpriteSheet>("BigShark");
            _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, "BFS1L", 2, 4);
            MoveDirection = _moveDirection;
            
        }

        public override Microsoft.Xna.Framework.Rectangle CollsionBounds
        {
            get
            {
                return new Rectangle((int)(Position.X - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5), (int)Position.Y - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height);
            }
        }
        private void MoveRight()
        {
            Vector2 newVelocity = Vector2.Zero;
            newVelocity.X += 1;
            Position += newVelocity;
        }
        private void MoveLeft()
        {
            Vector2 newVelocity = Vector2.Zero;
            newVelocity.X -= 1;
            Position += newVelocity;
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (MoveTimer != 0)
            {
                if (MoveTimer > 50)
                {
                   if (MoveDirection == 0)
                    {
                        MoveLeft();
                    }
                    else MoveRight();
                    MoveTimer--;
                    if (MoveDirection == 0)
                        _sprite.FirstFrame = "BFS1L";
                    else
                        _sprite.FirstFrame = "BFS1R";
                    _sprite.Update(gameTime);
                }
                if (MoveTimer <= 50)
                {
                      if (MoveDirection == 0)
                    {
                        MoveRight();
                    }
                    else MoveLeft();
                    MoveTimer--;
                    if (MoveDirection == 0)
                        _sprite.FirstFrame = "BFS1R";
                    else
                        _sprite.FirstFrame = "BFS1L";
                    _sprite.Update(gameTime);
                }
            }
            else MoveTimer = 100;
        }
        public override void Draw(DrawContext ctx)
        {
            _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.BottomCenter);
        }
    }

}
