#region File Description
//-----------------------------------------------------------------------------
// Player.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Engine.Sprites;



namespace UnderwaterPlatformer
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    class Player : GameObject
    {
        GameTime g;

        // Animations
        AnimatedSpriteFromSpriteSheet _sprite;
        AnimatedSpriteFromSpriteSheet _spriteJump;
        AnimatedSpriteFromSpriteSheet _spriteDead;


        // The sound used when the player or an enemy dies
        public SoundEffect jumpSound;
        public SoundEffect landSound;
        public SoundEffect hitSound;

        bool jumpLeft;
        bool jumpRight;

        private bool isMovableX;
        private bool isMovableY;
        public bool collisionX;
        private bool isOnPlatform;

        public TimeSpan CollideWaitTime = TimeSpan.FromSeconds(2.0);
        private TimeSpan FlickerTime = TimeSpan.FromSeconds(0.01);

        public const int MAX_OXYGEN_TIME_PER_BUBBLE = 10;
        public TimeSpan Oxygen_Time = TimeSpan.FromSeconds(MAX_OXYGEN_TIME_PER_BUBBLE);

        public bool movingLeft, movingRight, stationary;

        

        public int MAX_OXYGEN_TIME
        {
            get { return MAX_OXYGEN_TIME_PER_BUBBLE; }
        }

        public bool invincibilityFlicker = false;
        public bool HasCollided = false;

        private const int MAX_OXYGEN = 5;
        public int MAX_OXYGEN_
        {
            get { return MAX_OXYGEN; }
        }
        public int Oxygen { get; set; }

        public Vector2 offset;

        private const int MaxNumJumps = 2;
        private int currentJumps;

        public bool jumpSoundPlayed;
        public bool landSoundPlayed;

        public int CurrentJumps
        {
            get { return currentJumps; }
            set 
            {
                if (value < MaxNumJumps && value >= 0)
                    currentJumps = value;
                else
                    throw new Exception("Not an acceptable input for variable");
            }
        }

        public bool IsMovableX
        {
            get { return isMovableX; }
            set { isMovableX = value; }
        }

        public bool IsMovableY
        {
            get { return isMovableY; }
            set { isMovableY = value; }
        }

        private GameLevel level;

        public override Rectangle CollsionBounds
        {
            get
            {
                return new Rectangle((int)(Position.X - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width * 0.5), (int)(Position.Y - _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height), _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Width, _sprite.SpriteSheet.SourceRectangle(_sprite.CurrentFrame).Height);
            }
        }

        public BoundingBox CollisionBox
        {
            get
            {
                return new BoundingBox(new Vector3(this.CollsionBounds.X, this.CollsionBounds.Y, 0.0f), new Vector3(this.CollsionBounds.Right, this.CollsionBounds.Bottom, 0.0f));
            }
        }

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        private float previousBottom;

        public GameLevel Level
        {
            get { return level; }
            set { level = value; }
        }

        public bool IsJumping
        {
            get { return isJumping; }
            set { isJumping = value; }
        }
        Vector2 velocity;

        private const float moveSpeed = 5.0f;
        // Constants for controling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.70f;
        private const float JumpLaunchVelocity = -2000.0f;
        private const float GravityAcceleration = 100.0f;
        private const float MaxFallSpeed = 100.0f;
        private const float JumpControlPower = 0.14f; 

        // Input configuration
        private const float MoveStickScale = 1.0f;
        private const float AccelerometerScale = 1.5f;
        private const Buttons JumpButton = Buttons.A;
        bool isOnGround;
        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = value; }
        }

        public bool IsOnPlatform
        {
            get { return isOnPlatform; }
            set { isOnPlatform = value; }
        }
       

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;

        // Jumping state
        private bool isJumping;
        public bool wasJumping;
        private float jumpTime;

        private Rectangle localBounds;    

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player()
        {
            collisionX = false;
            Oxygen = 5;
            Reset(Position);
            isAlive = true;
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent(ContentManager _content)
        {
            var spriteSheet = _content.Load<SpriteSheet>("Player");
            var spriteJumpSheet = _content.Load<SpriteSheet>("PlayerJump");
            var spriteDeadSheet = _content.Load<SpriteSheet>("PlayerDead");
            _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, "PR1", 18, 8);
            _spriteJump = new AnimatedSpriteFromSpriteSheet(spriteJumpSheet, "PJR1", 18, 1);
            _spriteDead = new AnimatedSpriteFromSpriteSheet(spriteDeadSheet, "Player_Dead", 1, 1);
            jumpSound = _content.Load<SoundEffect>(@"Player\Sounds\Jump_Sound_Mike_Koenig");
            landSound = _content.Load<SoundEffect>(@"Player\Sounds\clothing_coat_lands_on_floor_005");
            hitSound = _content.Load<SoundEffect>(@"Player\Sounds\impact_snowball_hits_car_window_001");
            jumpSoundPlayed = false;
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            currentJumps = 0;
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        public void Update(
            GameTime gameTime, 
            KeyboardState keyboardState, 
            GamePadState gamePadState, 
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            GetInput(keyboardState, gamePadState, accelState, orientation);

            ApplyPhysics(gameTime);

            _sprite.Update(gameTime);
            _spriteJump.Update(gameTime);
            _spriteDead.Update(gameTime);
            // Clear input.
            movement = 0.0f;

            if (HasCollided && CollideWaitTime.TotalSeconds > 0)
            {
                FlickerTime -= gameTime.ElapsedGameTime;
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
            }
            if (Oxygen.Equals(0))
                isAlive = false;
        }
        public void Update(
            GameTime gameTime,
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            ApplyPhysics(gameTime);
            _sprite.Reset();
            isOnGround = true;
            // Clear input.
            movement = 0.0f;
        }

        public override void Update(GameTime gameTime)
        {
             throw new NotImplementedException();
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(
            KeyboardState keyboardState, 
            GamePadState gamePadState, 
            AccelerometerState accelState, 
            DisplayOrientation orientation)
        {
            
                Vector2 NewPos = Vector2.Zero;

                if ((gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                         keyboardState.IsKeyDown(Keys.Left) ||
                         keyboardState.IsKeyDown(Keys.A)))
                {
                    NewPos.X -= moveSpeed;
                    _sprite.FirstFrame = "PL1";
                    if (!isOnGround)
                        _spriteJump.FirstFrame = "PJL1";
                    movingLeft = true;
                    movingRight = false;
                    stationary = false;
                }
                else if ((gamePadState.IsButtonDown(Buttons.DPadRight) ||
                         keyboardState.IsKeyDown(Keys.Right) ||
                         keyboardState.IsKeyDown(Keys.D)))
                {
                    NewPos.X += moveSpeed;
                    _sprite.FirstFrame = "PR1";
                    if (!isOnGround)
                        _spriteJump.FirstFrame = "PJR1";
                    movingRight = true;
                    movingLeft = false;
                    stationary = false;
                }
                else
                {
                    _sprite.Reset();
                    movingLeft = false;
                    movingRight = false;
                    stationary = true;
                }
                
            if (!NewPos.Equals(Vector2.Zero))
                Position += NewPos;
            

            // Check if the player wants to jump.

            isJumping =
                (gamePadState.IsButtonDown(JumpButton) ||
                keyboardState.IsKeyDown(Keys.Space) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W));
            if (isJumping && (gamePadState.IsButtonUp(JumpButton) &&
                keyboardState.IsKeyUp(Keys.Space) &&
                keyboardState.IsKeyUp(Keys.Up) &&
                keyboardState.IsKeyUp(Keys.W)))
                     currentJumps++;
            //if ((isOnGround && isOnPlatform) && (keyboardState.IsKeyDown(Keys.Down) ||
            //    keyboardState.IsKeyDown(Keys.S)))
            //{
            //    isOnPlatform = false;
            //    isOnGround = false;
                
            //}
                
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                    velocity.Y = DoJump(velocity.Y, gameTime);
                // Apply velocity.
                Position += velocity * elapsed;
                Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));                
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround /*&& currentJumps < MaxNumJumps*/) || jumpTime > 0.0f)
                {
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    isOnGround = false;
                }
                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }


        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public override void Draw(DrawContext ctx)
        {
            // Draw that sprite.
            if (!invincibilityFlicker)
            {
                if (isAlive)
                {
                    if (isOnGround)
                        _sprite.Draw(ctx.Batch, Position + ctx.Offset + offset, Color.White, Engine.Extensions.AnchorType.BottomCenter);
                    else
                        _spriteJump.Draw(ctx.Batch, Position + ctx.Offset + offset, Color.White, Engine.Extensions.AnchorType.BottomCenter);
                }
                else
                    _spriteDead.Draw(ctx.Batch, Position + ctx.Offset, Color.White, Engine.Extensions.AnchorType.BottomCenter);
            }
            
            
        }
    }
}
