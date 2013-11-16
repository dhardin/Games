using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Sprites;

namespace PolarRecall
{
    class Explosion : GameObject
    {
        #region Fields
        AnimatedSpriteFromSpriteSheet _sprite;
        #endregion

        #region Properties
        public bool IsDead
        {
            get;
            set;
        }
        #endregion

        public void LoadContent(ContentManager content)
        {
            SpriteSheet explosions = content.Load<SpriteSheet>("Explosion");
            _sprite = new AnimatedSpriteFromSpriteSheet(explosions, "OutExplodeP1", 18, 8);
            _sprite.IsLooping = false;
            IsDead = false;
        }

        public override void Update(GameTime gameTime)
        {
            IsDead = _sprite.IsDone;
            if (!IsDead)
            {
                _sprite.Update(gameTime);
            }

        }

        public override void Draw(DrawContext ctx)
        {
            _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color, Engine.Extensions.AnchorType.Center);
        }

        public void Reset()
        {
            _sprite.Reset();
        }

        public override Rectangle CollsionBounds
        {
            get { return Rectangle.Empty; }
        }
    }
}
