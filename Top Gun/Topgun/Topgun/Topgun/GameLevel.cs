using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Audio;
using Engine.Extensions;
using Engine.Map;
using Engine.TaskManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.GameStateManagerment;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Engine.ParticleSystem;

namespace Topgun
{
    class GameLevel
    {
        private static readonly bool debug = false;

        #region Fields
        ContentManager content;

        Level _level;
        Dictionary<string, LevelType> _levelTypes = new Dictionary<string, LevelType>();
        LevelType _levelType;
        Dictionary<int, Texture2D> _tileType = new Dictionary<int, Texture2D>();

        const int LEVEL_LENGTH = 10;
        Texture2D[] _levelTextures = new Texture2D[LEVEL_LENGTH];
        List<Texture2D> levelGenerator = new List<Texture2D>();
        const int MAX_LEVEL_SCROLL_SPEED = 2;

        const int MAX_PLAYERS = 2;
        public Player[] players = new Player[MAX_PLAYERS];

        public List<Enemy> enemies = new List<Enemy>();
        const int MAX_ENEMIES = 40;
        private const int MAX_ENEMY_SPAWN_COUNT = 10;
        private int EnemyCountModifier = 20;

        private const float ENEMY_SPAWN_TIME = 10;
        private float enemySpawnTimeModifer = 0;
        TimeSpan EnemySpawn;

        public List<PowerUp> powerUps = new List<PowerUp>();
        public List<PowerUp> activePowerUps = new List<PowerUp>();
        private const int MAX_POWER_UP_SPAWN_COUNT = 5;
        private int PowerUpCountModifier = 0;

        private int wavesCompleted = 0;
        private const int MAX_NUMBER_WAVES_TO_COMPLETE = 4;
        private int maxNumberWavesModifier = 0;
        private bool exitReached;

        bool failureSoundPlayed = false;

        //SCALING DIFF
        //public int NumOfEnemies = 30;
        public float EnemyHealthFactor = 0.0f;
        public int score;
        public int prevScore;
        public int scoreChange;
        float difficultyScale = .00005f;
        //---------------------------------------

        bool nukePlayed = false;

        Random rand = new Random();

        int scrollPos = 0;

        //level sound effects
        SoundEffect nukeSoundEffect;
        SoundEffect failureSoundEffect;
        SoundEffect victorySoundEffect;
        private const double MAX_SOUND_EFFECT_MOD = 0.5;
        private double soundEffectMod = 1;

        private bool victoryPlayed = false;
        private bool failurePlayed = false;

        // The music played during gameplay
        Song gameplayMusic;

        ParticleSystem explosion;
        ParticleSystem blood;
        ParticleSystem powerup;
        #endregion

        #region Properties
        public Game _game;
        public bool Paused { get; set; }
        public bool ExitReached
        {
            get { return exitReached; }
        }
        public bool SoundEffects { get; set; }
        public bool Music { get; set; }

        #endregion

        public void LoadContent(ContentManager c, string levelFile)
        {
            //SCALING DIFF
            prevScore = 0;
            scoreChange = 0;
            //--------------

            content = c;
            _level = new Level();

            Music = true;
            SoundEffects = true;

            _levelTypes["Temperate"] = LevelType.Temperate;
            _levelTypes["Sky"] = LevelType.Sky;
            _levelTypes["Tropical"] = LevelType.Tropical;
            _levelTypes["Arctic"] = LevelType.Arctic;
            _levelTypes["Volcanic"] = LevelType.Volcanic;
            _levelTypes["Space"] = LevelType.Space;
            _levelTypes["Ocean"] = LevelType.Ocean;
            _levelTypes["Robot"] = LevelType.Robot;

            _levelType = _levelTypes[Path.GetFileNameWithoutExtension(levelFile)];

            int random = rand.Next(1, 3);

            if (random == 1)
            {
                for (int i = 1; i <= 5; i++)
                    _tileType[i] = content.Load<Texture2D>(@"Levels\" + _levelType + @"\" + "A" + i);
            }
            else
            {
                for (int i = 1; i <= 5; i++)
                    _tileType[i] = content.Load<Texture2D>(@"Levels\" + _levelType + @"\" + "B" + i);
            }

            for (int i = 0; i < 2; i++)
            {
                levelGenerator.Add(_tileType[rand.Next(1, 6)]);
            }

            gameplayMusic = content.Load<Song>(@"Sounds\dangerZone");
            
            failureSoundEffect = content.Load<SoundEffect>(@"Sounds\failure");
            victorySoundEffect = content.Load<SoundEffect>(@"Sounds\victory");

            explosion = new ParticleSystem(_game, @"Particles\explosionSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            blood = new ParticleSystem(_game, @"Particles\bloodSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            powerup = new ParticleSystem(_game, @"Particles\powerupSettings") { DrawOrder = ParticleSystem.AdditiveDrawOrder };
            _game.Components.Add(powerup);
            _game.Components.Add(blood);
            _game.Components.Add(explosion);

            // Start the music right away
            PlayMusic(gameplayMusic);

            SpawnEnemies(MAX_ENEMY_SPAWN_COUNT + EnemyCountModifier);
            SpawnPowerUps(MAX_POWER_UP_SPAWN_COUNT + PowerUpCountModifier);

            EnemySpawn = TimeSpan.FromSeconds(ENEMY_SPAWN_TIME + enemySpawnTimeModifer);
               
        }

        public T RandomEnum<T>()
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[rand.Next(0, values.Length)];
        }

        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);

               MediaPlayer.Volume = 0.5f;

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }



        /// <summary>
        /// This function takes a Vector2 as input, and returns that vector "clamped"
        /// to the current graphics viewport. We use this function to make sure that 
        /// no one can go off of the screen.
        /// </summary>
        /// <param name="vector">an input vector</param>
        /// <returns>the input vector, clamped between the minimum and maximum of the
        /// viewport.</returns>
        private Vector2 ClampToViewport(Vector2 vector)
        {

            Viewport vp = ScreenManager.Instance.GraphicsDevice.Viewport;
            vector.X = MathHelper.Clamp(vector.X, vp.X, vp.X + vp.Width);
            vector.Y = MathHelper.Clamp(vector.Y, vp.Y, vp.Y + vp.Height);
            return vector;
        }

        //SCALING DIFF
        public void DifficultyCheck()
        {
            float tempscore = score / 1000f;
            float tempprevscore = prevScore / 1000f;
            if (scoreChange < score - prevScore)
            {
                EnemyHealthFactor = ((tempscore * difficultyScale) / 5);
                EnemyCountModifier = (int)Math.Floor(EnemyCountModifier * (tempscore - tempprevscore) * difficultyScale);
            }
            else
            {
                EnemyHealthFactor = ((tempscore * difficultyScale) / 5);
                EnemyCountModifier = (int)Math.Floor(EnemyCountModifier * tempscore * difficultyScale);
            }
            Console.WriteLine("Enemy Mod: {0}", EnemyCountModifier);
            scoreChange = score - prevScore;
            prevScore = score;
        }
        //-----------------------------------------------------------------------------------
        #region Update

        public void Update(GameTime gameTime)
        {

            //if (!Music)
            //    MediaPlayer.Volume = 0.0f;
            //else
            //    MediaPlayer.Volume = 0.5f;

            //if (!SoundEffects)
            //{
            //    for (int i = 0; i < MAX_PLAYERS; i++)
            //        players[i].SoundEffects = false;
            //}
            //else
            //{
            //    for (int i = 0; i < MAX_PLAYERS; i++)
            //        players[i].SoundEffects = true;
            //}

            #region Player Collisions & Player Projectile/Nuke Boundaries Check, Score Update
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                #region Player Projectile vs Enemies
                //Player projectile and enemy collisions
                for (int j = 0; j < players[i].projectiles.Count; j++)
                {
                    for (int k = 0; k < enemies.Count; k++)
                    {
                        
                        //check for collision with enemies
                        if (players[i].projectiles[j].Active && enemies[k].IsActive && enemies[k].IsAlive && players[i].projectiles[j].rotatedCollisionBox.Intersects(enemies[k].rotatedCollisionBox))
                        {
                            enemies[k].Health = enemies[k].Health - players[i].projectiles[j].Damage;
                            enemies[k].isHit = true;
                            if (!players[i].projectiles[j].ProjectileType.Equals(ProjectileType.Heavy))
                                players[i].projectiles[j].Active = false;
                            if (enemies[k].Health == 0)//PARTICLES
                                explosion.AddParticles(enemies[k].Position, Vector2.Zero);
                            blood.AddParticles(enemies[k].Position, Vector2.Zero);//PARTICLES
                        }
                        else if (enemies[k].IsActive && enemies[k].attackType.Equals(AttackType.Advanced_Chase) && players[i].projectiles[j].rotatedCollisionBox.Intersects(enemies[k].rotatedEvadeBounds))
                        {
                            enemies[k].ChangeEvadeState(true);
                            enemies[k].IsEvading = true;
                            enemies[k].EvadeProjectilePos = players[i].projectiles[j].Position;
                        }
                        else if (enemies[k].IsActive && enemies[k].IsEvading && !players[i].projectiles[j].rotatedCollisionBox.Intersects(enemies[k].rotatedEvadeBounds))
                        {
                            enemies[k].ChangeEvadeState(false);
                            enemies[k].IsEvading = false;
                        }
                    }
                }
                #endregion

                #region Player Nuke vs Enemies and Enemy's Projectiles
                for (int k = 0; k < enemies.Count; k++)
                {
                    if (players[i].NukeFired && enemies[k].IsActive && enemies[k].rotatedCollisionBox.Intersects(players[i].nuke.CollisionBounds))
                    {
                        enemies[k].Health = 0;
                        enemies[k].isHit = true;
                        enemies[k].KilledBy(players[i].playerIndex);
                    }
                    for (int l = 0; l < enemies[k].projectiles.Count; l++)
                    {
                        if (players[i].NukeFired && enemies[k].projectiles[l].Active && enemies[k].projectiles[l].rotatedCollisionBox.Intersects(players[i].nuke.CollisionBounds))
                        {
                            enemies[k].projectiles[l].Active = false;
                        }
                    }

                }
                #endregion

                #region Player vs Power-Ups
                //player and power-up collisions
                for (int j = 0; j < powerUps.Count; j++)
                {
                    if (powerUps[j].IsActive && players[i].rotatedCollisionBox.Intersects(powerUps[j].CollisionBounds))
                    {
                        if (!players[i].powerUps.Contains(powerUps[j]) && (powerUps[j].Type.Equals(PowerUpType.Fast) || powerUps[j].Type.Equals(PowerUpType.Heavy) || powerUps[j].Type.Equals(PowerUpType.Sonic)))
                        {
                            bool removeCurrentPowerUp = false;
                            for (int k = 0; k < players[i].powerUps.Count; k++)
                            {
                                if (players[i].powerUps[k].Type.Equals(PowerUpType.Fast) || players[i].powerUps[k].Type.Equals(PowerUpType.Heavy) || players[i].powerUps[k].Type.Equals(PowerUpType.Sonic))
                                {
                                    players[i].powerUps.RemoveAt(k);
                                    players[i].powerUps.Add(powerUps[j]);
                                    removeCurrentPowerUp = true;
                                    break;
                                }
                            }
                            if (!removeCurrentPowerUp)
                                players[i].powerUps.Add(powerUps[j]);
                            players[i].UpdateFireRate();
                            powerUps[j].IsActive = false;
                            powerup.AddParticles(powerUps[j].Position, Vector2.Zero);//PARTICLES
                        }
                        else if (powerUps[j].Type.Equals(PowerUpType.Nuke) && players[i].NukeCount < players[i].Max_Nuke_Count)
                        {
                            players[i].powerUps.Add(powerUps[j]);
                            players[i].NukeCount = players[i].NukeCount + 1;
                            powerUps[j].IsActive = false;
                            powerup.AddParticles(powerUps[j].Position, Vector2.Zero);//PARTICLES
                        }
                        else if (powerUps[j].Type.Equals(PowerUpType.Shield) && players[i].ShieldCount < players[i].Max_Sheild_Count)
                        {
                            players[i].powerUps.Add(powerUps[j]);
                            players[i].ShieldCount = players[i].ShieldCount + 1;
                            powerUps[j].IsActive = false;
                            powerup.AddParticles(powerUps[j].Position, Vector2.Zero);//PARTICLES
                        }

                    }
                }
                #endregion

                #region Player Projectiles & Nuke Boundaries Check
                for (int j = 0; j < players[i].projectiles.Count; j++)
                {
                    //remove any projectiles that go out of bounds
                    if (players[i].projectiles[j].Position.X < -20.0f || players[i].projectiles[j].Position.X > ScreenManager.Instance.GraphicsDevice.Viewport.Width + 20.0f ||
                        players[i].projectiles[j].Position.Y < -20.0f || players[i].projectiles[j].Position.Y > ScreenManager.Instance.GraphicsDevice.Viewport.Height + 20.0f)
                    {
                        players[i].projectiles.RemoveAt(j);
                    }
                }

                if (players[i].NukeFired)
                {
                    if (!nukePlayed)
                    {
                        //nukeSoundEffect.Play();
                        nukePlayed = true;
                    }
                    if (players[i].nuke.CollisionBounds.Top < -100.0f &&
                        players[i].nuke.CollisionBounds.Bottom > ScreenManager.Instance.GraphicsDevice.Viewport.Height + 100.0f &&
                        players[i].nuke.CollisionBounds.Left < -100.0f &&
                        players[i].nuke.CollisionBounds.Right > ScreenManager.Instance.GraphicsDevice.Viewport.Width + 100.0f)
                    {
                        players[i].nuke.Active = false;
                        players[i].NukeFired = false;
                        nukePlayed = false;
                    }
                }
                #endregion

                #region Player Score Update
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (!enemies[j].IsAlive && players[i].playerIndex.Equals(enemies[j].KilledByPlayer))
                    {
                        players[i].AddScore(enemies[j]);
                    }
                }
                #endregion

            }
            #endregion

            #region Enemy Chase/Ranged Collisions
            //check for collisions w/ enemies and ranged enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].rangeTypes.Contains(enemies[i].attackType))
                {
                    for (int j = 0; j < enemies[i].projectiles.Count; j++)
                    {
                        for (int k = 0; k < MAX_PLAYERS; k++)
                        {
                            if (enemies[i].IsActive && enemies[i].projectiles[j].Active &&  !players[k].isDead && enemies[i].projectiles[j].rotatedCollisionBox.Intersects(players[k].rotatedCollisionBox) && !players[k].ShieldUsed)
                            {
                                players[k].HasCollided = true;
                                enemies[i].projectiles[j].Active = false;
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < MAX_PLAYERS; k++)
                    {
                        if (enemies[i].IsActive && !players[k].isDead &&enemies[i].rotatedCollisionBox.Intersects(players[k].rotatedCollisionBox) && !players[k].ShieldUsed)
                        {
                            players[k].HasCollided = true;
                        }
                    }
                }
            }

            #endregion

            #region Enemy and Power-Up Clean Up
            //Remove collected Power-Ups
            for (int i = 0; i < powerUps.Count; i++)
            {
                if (!powerUps[i].IsActive)
                    powerUps.RemoveAt(i);
            }

            //Remove killed enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].IsActive)
                    enemies.RemoveAt(i);
                else if (enemies[i].Position.Y > ScreenManager.Instance.GraphicsDevice.Viewport.Height)
                    enemies.RemoveAt(i);
            }
            #endregion

            #region Level Victory & Failure
            if (players[0].isDead && players[1].isDead && !failureSoundPlayed)
            {
                failureSoundEffect.Play();
                failureSoundPlayed = true;
            }

            if (wavesCompleted >= (MAX_NUMBER_WAVES_TO_COMPLETE + maxNumberWavesModifier) && enemies.Count.Equals(0) && !victoryPlayed)
            {
                exitReached = true;
                victorySoundEffect.Play();
                victoryPlayed = true;
            }
            #endregion

            #region Difficulty Scaling, Level Progression, & Enemy/Power-Up Spawning
            if (EnemySpawn.TotalSeconds <= 0 && !exitReached)
            {
                //SCALING DIFF
                DifficultyCheck();
                SpawnEnemies(MAX_ENEMY_SPAWN_COUNT + EnemyCountModifier);
                SpawnPowerUps(MAX_POWER_UP_SPAWN_COUNT + PowerUpCountModifier);
                EnemySpawn = TimeSpan.FromSeconds(ENEMY_SPAWN_TIME + enemySpawnTimeModifer);
                wavesCompleted++;
            }
            else
            {
                EnemySpawn -= gameTime.ElapsedGameTime;
            }
            if (scrollPos < 1024 - MAX_LEVEL_SCROLL_SPEED)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    Vector2 pos = enemies[i].Position;
                    pos.Y += MAX_LEVEL_SCROLL_SPEED;
                    enemies[i].Position = pos;
                }
                for (int i = 0; i < powerUps.Count; i++)
                {
                    Vector2 pos = powerUps[i].Position;
                    pos.Y += MAX_LEVEL_SCROLL_SPEED;
                    powerUps[i].Position = pos;
                }
                scrollPos += MAX_LEVEL_SCROLL_SPEED;
              
            }
            else
            {
                    levelGenerator.RemoveAt(0);
                    levelGenerator.Add(_tileType[rand.Next(1, 6)]);
                    levelGenerator.Add(_tileType[rand.Next(1, 6)]);
                    scrollPos = 0;
            }
            #endregion
        }

        #endregion

        #region Spawn
        public void SpawnEnemies(int num)
        {
            //place a clamp on the max enemies on screen for performance sake
            int modNum = (int) MathHelper.Clamp(num, 0, MAX_ENEMIES);
            for (int i = 0; i < modNum; i++)
            {
                Vector2 pos;
                pos.X = rand.Next(25, (ScreenManager.Instance.GraphicsDevice.Viewport.Width - 25));
                //pos.Y = rand.Next(25, (ScreenManager.Instance.GraphicsDevice.Viewport.Height - 25));
                pos.Y = rand.Next(ScreenManager.Instance.GraphicsDevice.Viewport.Height * -1, 0);
                //SCALING DIFF
                enemies.Add(new Enemy(RandomEnum<AttackType>(), LevelType.Temperate, pos, EnemyHealthFactor));
                enemies[i].LoadContent(content);
            }
        }

        public void SpawnPowerUps(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Vector2 pos;
                pos.X = rand.Next(25, (ScreenManager.Instance.GraphicsDevice.Viewport.Width - 25));
                //pos.Y = rand.Next(25, (ScreenManager.Instance.GraphicsDevice.Viewport.Height - 25));
                pos.Y = rand.Next(ScreenManager.Instance.GraphicsDevice.Viewport.Height * -1, 0);
                powerUps.Add(new PowerUp(RandomEnum<PowerUpType>(), pos));
                powerUps[i].LoadContent(content);
            }
        }

        #endregion



        #region Draw

        public void Draw(DrawContext ctx)
        {

            for (int i = 0; i < 2; i++)
                ctx.Batch.Draw(levelGenerator[i], new Vector2(0.0f, scrollPos + (1024 * -i)), Color.White);
            for (int i = 0; i < enemies.Count; i++)
                enemies[i].Draw(ctx);
            for (int i = 0; i < powerUps.Count; i++)
                powerUps[i].Draw(ctx);
            for (int i = 0; i < MAX_PLAYERS; i++)
                players[i].Draw(ctx);
         
            
        }

        #endregion
    }
}
