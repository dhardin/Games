using Engine.Audio;
using Engine.Extensions;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace PolarRecall
{
    class Shayne : GameObject
    {
        #region Fields
        AnimatedSpriteFromSpriteSheet _sprite;
        AnimatedSpriteFromSpriteSheet _shirt;
        private const float Speed = 100.0f;
        private SoundEffectInstance _walkingSound;

        #endregion

        #region Properties
        public Vector2 Direction { get; set; }
        public override Rectangle CollsionBounds
        {
            get
            {
                return new Rectangle((int)Position.X - 5, (int)Position.Y - 5, 10, 10);
            }
        }
        #endregion

        public Shayne()
        {
            Position = new Vector2(50, 50);
            Color = Color.Transparent;
        }

        public void LoadContent(ContentManager content)
        {
            var spriteSheet = content.Load<SpriteSheet>("Shayne");
            _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, "PF1", 18, 8);
            _shirt = new AnimatedSpriteFromSpriteSheet(spriteSheet, "PF1s", 18, 8);
            _walkingSound = AudioEngine.Instance.GetSoundEffect(@"Sounds\mario_footstep1");
        }

        public override void Update(GameTime gameTime)
        {
            if (Direction.Length() > 0.1f)
            {
                SetDirection();
                _sprite.Update(gameTime);
                _shirt.Update(gameTime);

                Vector2 delta = Direction * (Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                delta.Y = -delta.Y; // Different coordinate systems
                Position += delta;
                Velocity = delta;

                _walkingSound.Play();
            }
        }

        private void SetDirection()
        {
            CardinalDirection direction = Direction.ToCardinalDirection();
            switch (direction)
            {
                case CardinalDirection.North:
                    _sprite.FirstFrame = "PB1";
                    break;
                case CardinalDirection.NorthEast:
                    _sprite.FirstFrame = "PNE1";
                    break;
                case CardinalDirection.East:
                    _sprite.FirstFrame = "PR1";
                    break;
                case CardinalDirection.SouthEast:
                    _sprite.FirstFrame = "PSE1";
                    break;
                case CardinalDirection.South:
                    _sprite.FirstFrame = "PF1";
                    break;
                case CardinalDirection.SouthWest:
                    _sprite.FirstFrame = "PSW1";
                    break;
                case CardinalDirection.West:
                    _sprite.FirstFrame = "PL1";
                    break;
                case CardinalDirection.NorthWest:
                    _sprite.FirstFrame = "PNW1";
                    break;
                default:
                    break;
            }

            // Shirts follow a similar naming convention.
            _shirt.FirstFrame = _sprite.FirstFrame + "s";
        }

        public override void Draw(DrawContext ctx)
        {
            _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.BottomCenter);
            _shirt.Draw(ctx.Batch, Position + ctx.Offset, Color, AnchorType.BottomCenter);
        }
    }
}
