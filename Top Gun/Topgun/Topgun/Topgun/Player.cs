using Engine.Audio;
using Engine.Extensions;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using Engine.TaskManagement;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;
using System.IO;
using Engine.ParticleSystem;
using Engine.ParticleEmitter;

namespace Topgun
{
    class Player : GameObject
    {
        #region Fields
        private const float SPEED = 10.0f;
        private const float RotationRadiansPerSecond = 15.0f;
        private const float FullSpeed = 10.0f;
        private const float VelocityMaximum = 10.0f;
        private const float DragPerSecond = 0.9f;
        private float Rotation;

        private int health;
        public const int MAX_HEALTH = 10;
        private const float MAX_FIRE_RATE_TIME = 0.2f;

        private int shieldCount;
        public const int MAX_SHIELD_COUNT = 10;
        private const float MAX_SHIELD_TIME = 15.0f;

        private int nukeCount;
        public const int MAX_NUKE_COUNT = 2;
        private const float MAX_NUKE_COOLDOWN = 2;

        private int score;
        private const int MAX_SCORE = int.MaxValue;

        ParticleSystem smoke;
        ParticleSystem explosion;
        ParticleEmitter shieldP;
        ParticleSystem emitterSystem;
        ParticleSystem blood;

        public PlayerIndex playerIndex;
        AnimatedSpriteFromSpriteSheet _sprite;
        AnimatedSpriteFromSpriteSheet _spriteDeath;
        public List<Projectile> projectiles = new List<Projectile>();
        public RotatedRectangle rotatedCollisionBox;

        public List<PowerUp> powerUps = new List<PowerUp>();

        public Nuke nuke;

        public Shield shield;
        
        public TimeSpan CollideWaitTime = TimeSpan.FromSeconds(2.0);
        private TimeSpan FlickerTime = TimeSpan.FromSeconds(0.01);
        private TimeSpan FireCooldownTime = TimeSpan.FromSeconds(MAX_FIRE_RATE_TIME);
        private TimeSpan previousFireTime;
        private TimeSpan previousNukeTime;
        private TimeSpan ShieldTime = TimeSpan.FromSeconds(15.0f);
        private TimeSpan NukeTime = TimeSpan.FromSeconds(MAX_NUKE_COOLDOWN);

        public bool invincibilityFlicker = false;
        public bool HasCollided = false;
        private bool collideReset = true;
        private bool hasFired = false;
        private bool nukeFired = false;
        private bool shieldUsed = false;

        SoundEffect powerUpRapidFireSoundEffect;
        SoundEffect powerUpShieldSoundEffect;
        SoundEffect powerUpSonicSoundEffect;
        SoundEffect powerUpHeavySoundEffect;
        SoundEffect hitSoundEffect;
        SoundEffect standardFireSoundEffect;
        SoundEffect nukeSoundEffect;


        #endregion

        #region Properties

        public Game _game;

        public override Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle((int)(Position.X - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5), (int)(Position.Y - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height), _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height);
            }
        }

        public bool SoundEffects
        {
            get;
            set;
        }

        public float Speed
        {
            get { return SPEED; }
        }

        public int Score
        {
            get { return score; }

            set 
            {
                if (value > MAX_SCORE)
                    score = MAX_SCORE;
                else
                    score = value;
            }
        }

        public int Health
        {
            get { return health;}
        }

        public int MaxHealth
        {
            get { return MAX_HEALTH; }
        }

        public int NukeCount
        {
            get { return nukeCount; }
            set { nukeCount = value; }
        }

        public int Max_Nuke_Count
        {
            get { return MAX_NUKE_COUNT; }
        }

        public bool NukeFired
        {
            get { return nukeFired; }
            set { nukeFired = value; }
        }

        public int ShieldCount
        {
            get { return shieldCount; }
            set { shieldCount = value; }
        }

        public bool ShieldUsed
        {
            get { return shieldUsed; }
            set { shieldUsed = value; }
        }

        public int Max_Sheild_Count
        {
            get { return MAX_SHIELD_COUNT; }
        }
        #endregion

        public Player(PlayerIndex pIndex, Vector2 pos)
        {
            playerIndex = pIndex;
            Position = pos;
            health = 10;
            powerUps.Add(new PowerUp(PowerUpType.Nuke, pos));
            powerUps.Add(new PowerUp(PowerUpType.Nuke, pos));
            nukeCount += 2;
            shieldCount += 2;
        }

        public void LoadContent(ContentManager content)
        {
            var spriteSheet = content.Load<SpriteSheet>("Player\\Player");
            var spriteSheetDeath = content.Load<SpriteSheet>("Player\\PlayerDeath");
            if (playerIndex.Equals(PlayerIndex.One))
                _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, "Goose", 1, 1);
            else
                _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, "Maverick", 1, 1);

            _spriteDeath = new AnimatedSpriteFromSpriteSheet(spriteSheetDeath, "Explode1", 5, 8);
            _spriteDeath.IsLooping = false;

            powerUpRapidFireSoundEffect = content.Load<SoundEffect>(@"Sounds\rapidFire");
            powerUpShieldSoundEffect = content.Load<SoundEffect>(@"Sounds\shield");
            powerUpSonicSoundEffect = content.Load<SoundEffect>(@"Sounds\spreadFire");
            hitSoundEffect = content.Load<SoundEffect>(@"Sounds\hit");
            standardFireSoundEffect = content.Load<SoundEffect>(@"Sounds\standardFire");
            powerUpHeavySoundEffect = content.Load<SoundEffect>(@"Sounds\heavyFire");
            nukeSoundEffect = content.Load<SoundEffect>(@"Sounds\Nuke");

            smoke = new ParticleSystem(_game, @"Particles\SmokeSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            explosion = new ParticleSystem(_game, @"Particles\explosionSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            emitterSystem = new ParticleSystem(_game, @"Particles\shieldSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            blood = new ParticleSystem(_game, @"Particles\bloodSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            shieldP = new ParticleEmitter(emitterSystem, 50, Position);
            _game.Components.Add(emitterSystem);
            _game.Components.Add(explosion);
            _game.Components.Add(smoke);
            _game.Components.Add(blood);

            rotatedCollisionBox = new RotatedRectangle(CollisionBounds, Rotation);
        }

        public override void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);
            for (int i = 0; i < projectiles.Count; i++)
                projectiles[i].Update();

            if (nukeFired)
                nuke.Update();

            if (health > 0)
            {
                if (HasCollided && collideReset)
                {
                    health--;
                    collideReset = false;
                    hitSoundEffect.Play();
                    if (health == 0)//PARTICLES
                        explosion.AddParticles(Position, Vector2.Zero);//PARTICLES
                    blood.AddParticles(Position, Vector2.Zero);//PARTICLES
                }
                else if (HasCollided && CollideWaitTime.TotalSeconds > 0)
                {
                    FlickerTime -= gameTime.ElapsedGameTime;
                    CollideWaitTime -= gameTime.ElapsedGameTime;
                    if (FlickerTime.TotalSeconds <= 0 && !invincibilityFlicker)
                    {
                        invincibilityFlicker = true;
                        FlickerTime = TimeSpan.FromSeconds(0.01);
                    }
                    else if (FlickerTime.TotalSeconds <= 0 && invincibilityFlicker)
                    {
                        invincibilityFlicker = false;
                        FlickerTime = TimeSpan.FromSeconds(0.01);
                    }

                }
                else
                {
                    invincibilityFlicker = false;
                    FlickerTime = TimeSpan.FromSeconds(0.01);
                    CollideWaitTime = TimeSpan.FromSeconds(2.0);
                    HasCollided = false;
                    collideReset = true;
                }

                if (shieldUsed)
                {
                    shield.Update(Position);
                    ShieldTime -= gameTime.ElapsedGameTime;
                    if (ShieldTime.TotalSeconds <= 0)
                    {
                        shield.Active = false;
                        shieldUsed = false;
                        ShieldTime = TimeSpan.FromSeconds(MAX_SHIELD_TIME);
                    }
                    shieldP.Update(gameTime, Position);//PARTICLES
                }
            }
            else
            {
                isDead = true;
                _spriteDeath.Update(gameTime);
            }
        }

        public void HandleInput(GameTime gameTime, InputState input)
        {
            GamePadState gamePadState = GamePad.GetState(playerIndex);
            KeyboardState keyboardState = input.CurrentKeyboardStates[0];
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            Vector2 newPosition = Position ;
            int inputNum = 0;

            if (health > 0)
            {

                switch (playerIndex)
                {

                    case PlayerIndex.One:
                        inputNum = 0;
                        break;
                    case PlayerIndex.Two:
                        inputNum = 1;
                        break;
                    case PlayerIndex.Three:
                        inputNum = 2;
                        break;
                    case PlayerIndex.Four:
                        inputNum = 3;
                        break;
                }
                if (input.CurrentGamePadStates[inputNum].IsConnected)
                    gamePadState = input.CurrentGamePadStates[inputNum];




                //reset rotation angle if it is greater than 2PI radians or less than 0 radians
                Rotation = MathHelper.WrapAngle(Rotation);
                // Get Thumbstick Controls
                newPosition.X += gamePadState.ThumbSticks.Left.X * Speed;
                newPosition.Y -= gamePadState.ThumbSticks.Left.Y * Speed;

                // calculate the current forward vector (make it negative so we turn in the correct direction)
                Vector2 forward = new Vector2((float)Math.Sin(Rotation),
                    (float)Math.Cos(Rotation));
                Vector2 right = new Vector2(-1 * forward.Y, forward.X);//vector that is at a right angle (+PI/2) to the vector that you're moving in

                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // calculate the new forward vector with the left stick
                if (gamePadState.ThumbSticks.Right.LengthSquared() > 0f)
                {
                    // change the direction 
                    Vector2 wantedForward = Vector2.Normalize(gamePadState.ThumbSticks.Right);
                    //find the difference between our current vector and wanted vector using the dot product
                    float angleDiff = (float)Math.Acos(
                        Vector2.Dot(wantedForward, forward));

                    //check the angle between the right angle from the current vector and wanted.  Adjust rotation direction accordingly
                    float facing;
                    if (Vector2.Dot(wantedForward, right) > 0f)
                    {
                        //rotate towards right angle first
                        facing = -1.0f;
                    }
                    else
                    {
                        //rotate towards the wanted angle since it is closer
                        facing = 1.0f;
                    }

                    //if we have an acceptable change in direction that is not too small
                    if (angleDiff > (Math.PI / 20))
                    {
                        Rotation += facing * Math.Min(angleDiff, elapsed *
                            RotationRadiansPerSecond);
                    }

                    // add velocity
                    Velocity += gamePadState.ThumbSticks.Left * (elapsed * FullSpeed);
                    if (Velocity.Length() > VelocityMaximum)
                    {
                        Velocity = Vector2.Normalize(Velocity) *
                            VelocityMaximum;
                    }
                }
                //currentGamePadState.ThumbSticks.Left = Vector2.Zero;

                // apply drag to the velocity
                Velocity -= Velocity * (elapsed * DragPerSecond);
                if (Velocity.LengthSquared() <= 0f)
                {
                    Velocity = Vector2.Zero;
                }

                // Use the Keyboard / Dpad
                if (keyboardState.IsKeyDown(Keys.Left) ||
                gamePadState.DPad.Left == ButtonState.Pressed)
                {
                    newPosition.X -= Speed;
                }
                if (keyboardState.IsKeyDown(Keys.Right) ||
                gamePadState.DPad.Right == ButtonState.Pressed)
                {
                    newPosition.X += Speed;
                }
                if (keyboardState.IsKeyDown(Keys.Up) ||
                gamePadState.DPad.Up == ButtonState.Pressed)
                {
                    newPosition.Y -= Speed;
                }
                if (keyboardState.IsKeyDown(Keys.Down) ||
                gamePadState.DPad.Down == ButtonState.Pressed)
                {
                    newPosition.Y += Speed;
                }

                // Make sure that the player does not go out of bounds
                newPosition.X = MathHelper.Clamp(newPosition.X, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5f, ScreenManager.Instance.GraphicsDevice.Viewport.Width - (_sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5f));
                newPosition.Y = MathHelper.Clamp(newPosition.Y, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height, ScreenManager.Instance.GraphicsDevice.Viewport.Height);

                Position = newPosition;
                rotatedCollisionBox.ChangePosition(new Vector2(Position.X - rotatedCollisionBox.Width / 2, Position.Y - rotatedCollisionBox.Height / 2));
                rotatedCollisionBox.Rotation = Rotation;
                //Vector2 modPos = new Vector2( _sprite.SpriteSheet.Texture.Width,_sprite.SpriteSheet.Texture.Height);

                // Fire in indicated right thumbstick direction
                if ((gamePadState.Triggers.Right >= 0.5f || keyboardState.IsKeyDown(Keys.Space)) && (gameTime.TotalGameTime - previousFireTime > FireCooldownTime))
                {
                    // Reset our current time
                    previousFireTime = gameTime.TotalGameTime;
                    if (currentProjectilePowerUp().Equals(PowerUpType.Sonic))
                    {
                        AddProjectile(Position, (float)(Rotation - Math.PI * 0.5f));
                        AddProjectile(Position, (float)(Rotation - Math.PI * 0.5f + Math.PI * 0.25));
                        AddProjectile(Position, (float)(Rotation - Math.PI * 0.5f - Math.PI * 0.25));
                    }
                    else
                        AddProjectile(Position, (float)(Rotation - Math.PI * 0.5f));

                    var pos = new Vector2(Position.X - 15, Position.Y - 5);//PARTICLES
                    smoke.AddParticles(pos, Vector2.Zero);//PARTICLES
                    pos = new Vector2(Position.X + 15, Position.Y - 5);//PARTICLES
                    smoke.AddParticles(pos, Vector2.Zero);//PARTICLES

                    hasFired = true;
                }
                if (nukeCount > 0 && (gamePadState.IsButtonDown(Buttons.RightShoulder) || keyboardState.IsKeyDown(Keys.N)) && (gameTime.TotalGameTime - previousNukeTime > NukeTime))
                {
                    previousNukeTime = gameTime.TotalGameTime;
                    nuke = new Nuke();
                    nuke.Initialize(Position);
                    nukeFired = true;
                    nukeCount--;
                    nukeSoundEffect.Play();
                }
                if (shieldCount > 0 && (gamePadState.Triggers.Left >= 0.5f || keyboardState.IsKeyDown(Keys.LeftShift)) && !shieldUsed)
                {
                    shieldUsed = true;
                    shield = new Shield();
                    shield.Initialize(Position);
                    shieldCount--;
                    powerUpShieldSoundEffect.Play();
                }
            }
                
        }

        /// <summary>
        /// Add projectile fired by the player to the game
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fireDirection"></param>
        private void AddProjectile(Vector2 position, float fireDirection)
        {
            Projectile projectile;
            switch (currentProjectilePowerUp())
            {
                case PowerUpType.Fast:
                    projectile = new Projectile(fireDirection, ProjectileType.Rapid);
                    projectile.Initialize(position, ProjectileType.Rapid);
                    powerUpRapidFireSoundEffect.Play(0.10f, 0.0f, 0.0f);
                    break;
                case PowerUpType.Heavy:
                    projectile = new Projectile(fireDirection, ProjectileType.Heavy);
                    projectile.Initialize(position, ProjectileType.Heavy);
                    powerUpHeavySoundEffect.Play(0.10f, 0.0f, 0.0f);
                    break;
                case PowerUpType.Sonic:
                    projectile = new Projectile(fireDirection, ProjectileType.Spread);
                    projectile.Initialize(position, ProjectileType.Spread);
                    powerUpSonicSoundEffect.Play(0.04f, 0.0f, 0.0f);
                    break;
                default:
                    projectile = new Projectile(fireDirection, ProjectileType.Standard);
                    projectile.Initialize(position, ProjectileType.Standard);
                    standardFireSoundEffect.Play(0.10f, 0.0f, 0.0f);
                    break;                
            }
            
            projectiles.Add(projectile);
        }

        private PowerUpType currentProjectilePowerUp()
        {
            for (int i = 0; i < powerUps.Count; i++)
            {
                if (powerUps[i].Type.Equals(PowerUpType.Fast) || powerUps[i].Type.Equals(PowerUpType.Heavy) || powerUps[i].Type.Equals(PowerUpType.Sonic))
                    return powerUps[i].Type;
            }
            return PowerUpType.None;
        }

        public void UpdateFireRate()
        {
            
            for (int i = 0; i < powerUps.Count; i++)
            {
                if (powerUps[i].Type.Equals(PowerUpType.Fast) || powerUps[i].Type.Equals(PowerUpType.Heavy) || powerUps[i].Type.Equals(PowerUpType.Sonic) || powerUps[i].Type.Equals(PowerUpType.None))
                    FireCooldownTime = TimeSpan.FromSeconds(powerUps[i].RateOfFire);
            }
        }

        public void AddScore(Enemy enemy)
        {
            score += enemy.PointValue;
        }

        public override void Draw(DrawContext ctx)
        {
            if (!invincibilityFlicker && health > 0)
            {
                if (playerIndex.Equals(PlayerIndex.One))
                    _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.Center, SpriteEffects.None, Rotation);
                else
                    _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.DarkGray, AnchorType.Center, SpriteEffects.None, Rotation);
            }
            else
                _spriteDeath.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.Center, SpriteEffects.None, 0);

            for (int i = 0; i < projectiles.Count; i++)
                projectiles[i].Draw(ctx);
            if (nukeFired)
                nuke.Draw(ctx);
            if (shieldUsed)
                shield.Draw(ctx);
        }
    }
}
