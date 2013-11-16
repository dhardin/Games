using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Engine.Sprites;
using Engine.Extensions;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using Engine.GameStateManagerment;

namespace Topgun
{
    enum PowerUpType
    {
        None,
        Fast,
        Heavy,
        Sonic,
        Shield,
        Nuke
    }
    class PowerUp : GameObject
    {
        #region Fields
        PowerUpType type;

        AnimatedSpriteFromSpriteSheet _sprite;

        const string PATH = "PowerUps\\";

        bool isActive = false;

        string firstFrame, powerUpName;

        const int NUM_FRAMES = 1;

        private float rateOfFire;



        #endregion

        #region Properties
        public PowerUpType Type
        {
            get { return type; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public float RateOfFire
        {
            get { return rateOfFire; }
        }
        #endregion

        public PowerUp(PowerUpType pUp, Vector2 pos)
        {
            type = pUp;
            Position = pos;
            ActivatePowerUp();
        }

        public void ActivatePowerUp(/*ref Player player*/)
        {
            switch (type)
            {
                case PowerUpType.None:
                     rateOfFire = 0.4f;
                    break;  
                case PowerUpType.Fast:
                    firstFrame = "FastFire";
                    rateOfFire = 0.1f;
                    break;
                case PowerUpType.Heavy:
                    firstFrame = "Heavy";
                    rateOfFire = 0.3f;
                    break;
                case PowerUpType.Sonic:
                    firstFrame = "Sonic";
                    rateOfFire = .2f;
                    break;
                case PowerUpType.Shield:
                    firstFrame = "Shield";
                    break;
                case PowerUpType.Nuke:
                    firstFrame = "Nuke";
                    break;
            }
        }

        public override Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle((int)(Position.X - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5), (int)(Position.Y - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height), _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height);
            }
        }

        public void LoadContent(ContentManager _content)
        {
            if (!type.Equals(PowerUpType.None))
            {
                string temp = PATH;

                var spriteSheet = _content.Load<SpriteSheet>(temp + "\\PowerUp");

                _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, firstFrame, 2, NUM_FRAMES);

                isActive = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);
        }

        public override void Draw(DrawContext ctx)
        {
            if (isActive)
            {
                _sprite.Draw(ctx.Batch, Position + ctx.Offset, new Color(0.0f, 0.0f, 0.0f, 0.85f), AnchorType.Center, SpriteEffects.None, 0.0f, 1.1f);
                _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.Center, SpriteEffects.None);
            }
        }
    }
}
