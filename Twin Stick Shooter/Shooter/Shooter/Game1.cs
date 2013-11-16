using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch; //USED FOR TOUCHSCREEN PHONE

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // Represents the player 
        Player player;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // A movement speed for the player
        float playerMoveSpeed;

        // Image used to display the static background
        Texture2D mainBackground;

        Texture2D starTexture;
        List<Animation> stars;

        // Enemies
        Texture2D enemyTexture;
        Texture2D triangleEnemyTexture;
        Texture2D circleEnemyTexture;
        Texture2D diamondEnemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        // A random number generator
        Random random;

        Texture2D projectileTexture;
        List<Projectile> projectiles;

        // The rate of fire of the player laser
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        //Explosions
        Texture2D explosionTexture;
        List<Animation> explosions;

        // The sound that is played when a laser is fired
        SoundEffect laserSound;

        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;

        // The music played during gameplay
        Song gameplayMusic;

        //Number that holds the player score
        int score;
        // The font used to display UI elements
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Initialize the player class
            player = new Player();

            // Set a constant player move speed
            playerMoveSpeed = 8.0f;

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);

            // Initialize our random number generator
            random = new Random();

            stars = new List<Animation>();

            projectiles = new List<Projectile>();

            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(.09f);

            //Initialize explosions
            explosions = new List<Animation>();

            //Set player's score to zero
            score = 0;

            //Enable the FreeDrag gesture.
           TouchPanel.EnabledGestures = GestureType.FreeDrag; //USED FOR TOUCHSCREEN PHONE

           graphics.PreferredBackBufferWidth = 800;
           graphics.PreferredBackBufferHeight = 600;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the player resources
            //Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("ship");
            //playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            enemyTexture = Content.Load<Texture2D>("mineAnimation");
            triangleEnemyTexture = Content.Load<Texture2D>("Enemies/triangle_enemy");
            circleEnemyTexture = Content.Load<Texture2D>("Enemies/circle_enemy");
            diamondEnemyTexture = Content.Load<Texture2D>("Enemies/diamond_enemy");

            starTexture = Content.Load<Texture2D>("background/star_animation");

            projectileTexture = Content.Load<Texture2D>("laser");

            explosionTexture = Content.Load<Texture2D>("explosion");

            // Load the music
            gameplayMusic = Content.Load<Song>("sound/gameMusic");

            // Load the laser and explosion sound effect
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");

            // Load the score font
            font = Content.Load<SpriteFont>("gameFont");

            // Start the music right away
            PlayMusic(gameplayMusic);

            mainBackground = Content.Load<Texture2D>("bg_stars");

            //AddStars(20);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerTexture, playerPosition);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// Add animated stars to your background image.
        /// </summary>
        /// <param name="numStars"></param>
        public void AddStars(int numStars)
        {
            //randomly load stars
            //Random randomStarX = new Random();
           // Random randomStarY = new Random();
           
            for (int x = 0; x < numStars; x++)
            {
                for (int y = 0; y < numStars; y++)
                {
                    Vector2 starPosition = new Vector2();
                    Animation starAnimation = new Animation();
                    starPosition.X = x * 25;
                    starPosition.Y = y * 25;
                    //starPosition.X = randomStarX.Next(0, graphics.PreferredBackBufferWidth);
                    //starPosition.Y = randomStarY.Next(0, graphics.PreferredBackBufferHeight);
                    starAnimation.Initialize(starTexture, starPosition, 3, starTexture.Height, 3, 600, Color.White, 1f, true);
                    stars.Add(starAnimation);
                }
            }
           
        }

        /// <summary>
        /// Used to play our game music.
        /// </summary>
        /// <param name="song"></param>
        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);


            //Update the player
            UpdatePlayer(gameTime);

            // Update the enemies
            UpdateEnemies(gameTime);

            // Update the collision
            UpdateCollision();

            // Update the projectiles
            UpdateProjectiles();

            // Update the explosions
            UpdateExplosions(gameTime);

            UpdateStars(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Update the collisons between objects
        /// </summary>
        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X,
            (int)player.Position.Y,
            player.Width,
            player.Height);

            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X,
                (int)enemies[i].Position.Y,
                enemies[i].Width,
                enemies[i].Height);

                // Determine if the two objects collided with each
                // other
                if (rectangle1.Intersects(rectangle2))
                {
                    // Subtract the health from the player based on
                    // the enemy damage
                    player.Health -= enemies[i].Damage;

                    // Since the enemy collided with the player
                    // destroy it
                    enemies[i].Health = 0;

                    // If the player health is less than zero we died
                    if (player.Health <= 0)
                        player.Active = false;
                }

            }

            // Projectile vs Enemy Collision
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // Create the rectangles we need to determine if we collided with each other
                    rectangle1 = new Rectangle((int)projectiles[i].Position.X -
                    projectiles[i].Width / 2, (int)projectiles[i].Position.Y -
                    projectiles[i].Height / 2, projectiles[i].Width, projectiles[i].Height);

                    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                    enemies[j].Width, enemies[j].Height);

                    // Determine if the two objects collided with each other
                    if (rectangle1.Intersects(rectangle2))
                    {
                        enemies[j].Health -= projectiles[i].Damage;
                        projectiles[i].Active = false;
                    }
                }
            }
        }

        /// <summary>
        /// Add enemies to our game.
        /// </summary>
        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();
            Animation circleAnimation = new Animation();
            Animation triangleAnimation = new Animation();
            Animation diamondAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            circleAnimation.Initialize(circleEnemyTexture, Vector2.Zero, circleEnemyTexture.Width, circleEnemyTexture.Height, 1, 1, Color.White, 1f, true);
            triangleAnimation.Initialize(triangleEnemyTexture, Vector2.Zero, triangleEnemyTexture.Width, triangleEnemyTexture.Height, 1, 1, Color.White, 1f, true);
            diamondAnimation.Initialize(diamondEnemyTexture, Vector2.Zero, diamondEnemyTexture.Width, diamondEnemyTexture.Height, 1, 1, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));
            Vector2 position2 = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));
            Vector2 position3 = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));
            Vector2 position4 = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy(graphics);
            Enemy circleEnemy = new Enemy(graphics);
            Enemy diamondEnemy = new Enemy(graphics);
            Enemy triangleEnemy = new Enemy(graphics);

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);
            circleEnemy.Initialize(circleAnimation, position2);
            diamondEnemy.Initialize(diamondAnimation, position3);
            triangleEnemy.Initialize(triangleAnimation, position4);


            // Add the enemy to the active enemies list
            int enemyType = random.Next(0, 3);

            switch (enemyType)
            {
                case 0:
                    enemies.Add(enemy);
                    break;
                case 1:
                    enemies.Add(circleEnemy);
                    break;
                case 2:
                    enemies.Add(diamondEnemy);
                    break;
                case 3:
                    enemies.Add(triangleEnemy);
                    break;
            }
        }

        /// <summary>
        /// Add projectile fired by the player to the game
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fireDirection"></param>
        private void AddProjectile(Vector2 position, float fireDirection)
        {
            Projectile projectile = new Projectile(fireDirection);
            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
            projectiles.Add(projectile);
        }

        /// <summary>
        /// Add an explosion to the game.
        /// </summary>
        /// <param name="position"></param>
        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }


        /// <summary>
        /// Update star animations
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateStars(GameTime gameTime)
        {
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                stars[i].Update(gameTime);
            }
        }


        /// <summary>
        /// update explosion animations and remove explosions
        /// that have reached their lifetime.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Update enemy spawn time and enemy animations
        /// add explosions for dead enemies, update score, and remove destroyed
        /// enemies.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime, player.Position);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies[i].Position);

                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += enemies[i].Value;
                    }
                    enemies.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Update projectile animations
        /// </summary>
        private void UpdateProjectiles()
        {
            // Update the Projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Updates the Player object with respect to recieved input
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdatePlayer(GameTime gameTime)
        {

            //reset rotation angle if it is greater than 2PI radians or less than 0 radians
            player.RotationAngle = MathHelper.WrapAngle(player.RotationAngle);
            // Get Thumbstick Controls
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // calculate the current forward vector (make it negative so we turn in the correct direction)
            Vector2 forward = new Vector2((float)Math.Sin(player.RotationAngle),
                (float)Math.Cos(player.RotationAngle));
            Vector2 right = new Vector2(-1 * forward.Y,forward.X);//vector that is at a right angle (+PI/2) to the vector that you're moving in

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // calculate the new forward vector with the left stick
            if (currentGamePadState.ThumbSticks.Left.LengthSquared() > 0f)
            {
                // change the direction 
                Vector2 wantedForward = Vector2.Normalize(currentGamePadState.ThumbSticks.Left);
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
                if (angleDiff > (Math.PI/20))
                {
                    player.RotationAngle += facing * Math.Min(angleDiff, elapsed *
                        player.RotationRadiansPerSecond);
                }

                // add velocity
                player.Velocity += currentGamePadState.ThumbSticks.Left * (elapsed * player.FullSpeed);
                if (player.Velocity.Length() > player.VelocityMaximum)
                {
                    player.Velocity = Vector2.Normalize(player.Velocity) *
                        player.VelocityMaximum;
                }
            }
            //currentGamePadState.ThumbSticks.Left = Vector2.Zero;

            // apply drag to the velocity
            player.Velocity -= player.Velocity * (elapsed * player.DragPerSecond);
            if (player.Velocity.LengthSquared() <= 0f)
            {
                player.Velocity = Vector2.Zero;
            }
            // Windows Phone Controls
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.FreeDrag)
                {
                    player.Position += gesture.Delta;
                }
            }


            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
            currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
            currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }

            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

            // Fire in indicated right thumbstick direction
            if (currentGamePadState.ThumbSticks.Right.LengthSquared() > 0.0f && (gameTime.TotalGameTime - previousFireTime > fireTime))
            {
                // Reset our current time
                previousFireTime = gameTime.TotalGameTime;

                // change the fire direction 
                Vector2 wantedForward = Vector2.Normalize(currentGamePadState.ThumbSticks.Right);
                Vector2 forwardVector = new Vector2(1, 0);
                //check the angle between the right angle from the current vector and wanted.  Adjust rotation direction accordingly
              
                float angleDiff = (float)Math.Acos(Vector2.Dot(wantedForward, forwardVector));
                //quadrant II
                if (wantedForward.Y > 0)
                {
                    angleDiff *= -1;
                }
            
                // Add the projectile but rotate it and fire in the direction indicated by the right analog stick
                AddProjectile(player.Position + new Vector2(player.Width / 2, 0), angleDiff);
                // Play the laser sound
                laserSound.Play();
            }

            // reset score if player health goes to zero
            if (player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Start drawing
            spriteBatch.Begin();

            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
            for (int i = 0; i < stars.Count; i++)
            {
                stars[i].Draw(spriteBatch);
            }

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            // Draw the Projectiles
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }

            // Draw the explosions
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);

            }

            // Draw the score
            spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
            // Draw the Player
            player.Draw(spriteBatch);

            // Stop drawing
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
