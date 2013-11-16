using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Engine.Sprites;
using Engine.Extensions;
using Microsoft.Xna.Framework.Content;

namespace UnderwaterPlatformer.Enemies
{
    enum FishType
    {
        SmallFish,
        MediumFish,
        BigFish
    }
    class Fish : GameObject
    {
        int MoveDirection = 0;
        int MoveTimer = 100;
        public AnimatedSpriteFromSpriteSheet _sprite;
        private string fishName, fishFirstFrameLeft, fishFirstFrameRight;
        private int numFrames;
        public FishType type;

        

        public Fish(FishType fish)
        {
            type = fish;
            switch (fish)
            {
                case FishType.SmallFish:
                    fishName = "TinyFish";
                    fishFirstFrameLeft = "TF1L";
                    fishFirstFrameRight = "TF1R";
                    numFrames = 2;
                    break;
                case FishType.MediumFish:
                    fishName = "MediumFish";
                    fishFirstFrameLeft = "MF1L";
                    fishFirstFrameRight = "MF1R";
                    numFrames = 3;
                    break;
                case FishType.BigFish:
                    fishName = "LargeFish";
                    fishFirstFrameLeft = "LF1L";
                    fishFirstFrameRight = "LF1R";
                    numFrames = 4;
                    break;
            }

        }
        public void LoadContent(ContentManager _content, int _moveDirection)
        {
            var spriteSheet = _content.Load<SpriteSheet>(fishName);
            _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, fishFirstFrameLeft, 2, numFrames);
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
                        _sprite.FirstFrame = fishFirstFrameLeft;
                    else
                        _sprite.FirstFrame = fishFirstFrameRight;
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
                        _sprite.FirstFrame = fishFirstFrameRight;
                    else
                        _sprite.FirstFrame = fishFirstFrameLeft;
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
