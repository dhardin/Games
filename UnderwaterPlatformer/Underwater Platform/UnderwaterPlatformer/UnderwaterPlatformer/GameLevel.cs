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
using Microsoft.Xna.Framework.Input;
using UnderwaterPlatformer.Enemies;
using Microsoft.Xna.Framework.Media;

namespace UnderwaterPlatformer
{
    class GameLevel
    {
        #region Fields
        public Level _level;
        readonly Dictionary<char, Texture2D> _levelTextures = new Dictionary<char, Texture2D>();
        readonly Dictionary<char, Texture2D> _worldTextures = new Dictionary<char, Texture2D>();
        readonly Dictionary<char, Texture2D> _worldEntities = new Dictionary<char, Texture2D>();
        

        public List<WorldObject> WorldObjects = new List<WorldObject>();
        List<WorldObject> WorldEntities = new List<WorldObject>();
        List<Bubble> Bubbles = new List<Bubble>();
        List<BubbleGround> GroundBubbles = new List<BubbleGround>();
        List<Vector2> BubbleSpawnPoint = new List<Vector2>();
        public List<WorldObject> Oxygen = new List<WorldObject>();
        List<Fish> Fish = new List<Fish>();
        List<Shark> Shark = new List<Shark>();
        public List<BoundingBox> TileCollisionBoxes = new List<BoundingBox>();

        Random rand = new Random();


        //level sound effects
        SoundEffect bubbleSpawnSoundEffect;
        SoundEffect bubblePopSoundEffect;
        SoundEffect diamondCollectSoundEffect;

        // The music played during gameplay
        Song gameplayMusic;
        Song underwaterNoise;

        public int _tileSize;
        public Vector2 _levelSize;
        ContentManager _content;
        public Player player;
        Vector2 playerSpawn;
        public Vector2 exitSpawn;
        public int numBubblesGone = 0;
        private int numBubblesSpawn = 0;
        private const int MAX_BUBBLE_SPAWN_TIME = 20;
        

        private int score;
        private const int DIAMOND_SCORE = 50;

        public bool levelStationary;


        Tile[,] tiles;


        Vector2 offset = Vector2.Zero;

        private const char playerKey = 'P';
        private const char blankKey = '~';
        private const char exitKey = 'x';
        private const char diamondKey = 'd';
        private const string collisionTiles = "-_[]./\\LJ";
        private const string platformTiles = "tT";
        private const string fishKeys = "fMF";
        private const char sharkKey = 'S';
        private const char bubbleKey = 'b';

        static readonly Color InvalidColor = Color.Transparent;

        Texture2D largeBubble;
        public TimeSpan BubbleSpawnTime = TimeSpan.FromSeconds(5.0);
        
        #endregion

        #region Properties
                
        public TimeSpan lastBubbleSpawn { get; set; }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public Vector2 PlayerSpawn
        {
            get { return playerSpawn; }
        }


        public float ScreenHeight
        {
            get;
            set;
        }
        public float ScreenWidth
        {
            get;
            set;
        }
        #endregion

        #region Load Content
        public void LoadContent(ContentManager content, string levelFile)
        {
            _content = content;
            _level = new Level(levelFile);
            _levelTextures['.'] = content.Load<Texture2D>(@"Tiles\Ground2");
            _levelTextures['-'] = content.Load<Texture2D>(@"Tiles\Ground_Up2");
            _levelTextures['_'] = content.Load<Texture2D>(@"Tiles\Ground_Down2");
            _levelTextures[']'] = content.Load<Texture2D>(@"Tiles\Ground_Right2");
            _levelTextures['['] = content.Load<Texture2D>(@"Tiles\Ground_Left2");
            _levelTextures['.'] = content.Load<Texture2D>(@"Tiles\Ground2");
            _levelTextures['/'] = content.Load<Texture2D>(@"Tiles\Ground_UpLeft2");
            _levelTextures['\\'] = content.Load<Texture2D>(@"Tiles\Ground_UpRight2");
            _levelTextures['L'] = content.Load<Texture2D>(@"Tiles\Ground_DownLeft2");
            _levelTextures['J'] = content.Load<Texture2D>(@"Tiles\Ground_DownRight2");
            _levelTextures['~'] = content.Load<Texture2D>(@"Tiles\Blank");
            

            _worldTextures['t'] = content.Load<Texture2D>(@"Water Decor\TreeSmall");
            _worldTextures['T'] = content.Load<Texture2D>(@"Water Decor\TreeTall");

            _worldEntities['d'] = content.Load<Texture2D>(@"Diamonds\Diamond_Blue");
            _worldEntities['x'] = content.Load<Texture2D>(@"Level\Exit");
            //_worldEntities['b'] = content.Load<Texture2D>(@"Bubbles\bubble_ground");
            
            _tileSize = _levelTextures['.'].Width;
            _levelSize.X = _level.Width * _tileSize;
            _levelSize.Y = _level.Height * _tileSize;

            tiles = new Tile[_level.Width, _level.Height];


            largeBubble = content.Load<Texture2D>(@"Bubbles\bubble_large");
            
            //populate tile collision array and object/entity lists
            for (int row = 0; row < _level.Height; row++)
            {
                for (int col = 0; col < _level.Width; col++)
                {
                    char c = _level.TileAt(col, row);
                    TileCollisionBoxes.Add(new BoundingBox(new Vector3(col * _tileSize, row * _tileSize, 0.0f), new Vector3(col * _tileSize + _tileSize, row * _tileSize + _tileSize, 0.0f)));
                  
                    //only add a world object if it is recongnized as a world texture
                    if (_worldEntities.ContainsKey(c) || c.Equals(blankKey))
                    {
                        if (!c.Equals(blankKey))
                        { 
                            if (c.Equals(playerKey))
                                WorldEntities.Add(new WorldObject(_levelTextures[blankKey], new Vector2(col * _tileSize, row * _tileSize)));
                            if (fishKeys.Contains(c))
                            {
                                WorldEntities.Add(new WorldObject(_levelTextures[blankKey], new Vector2(col * _tileSize, row * _tileSize)));
                            }
                            else if (c.Equals(sharkKey))
                            {
                                WorldEntities.Add(new WorldObject(_levelTextures[blankKey], new Vector2(col * _tileSize, row * _tileSize)));
                            }
                            else
                                WorldEntities.Add(new WorldObject(_worldEntities[c], new Vector2(col * _tileSize, row * _tileSize)));
                        }
                        tiles[col, row] = new Tile(_levelTextures[blankKey], TileCollision.Passable);
                    }
                    else if (_worldTextures.ContainsKey(c))
                    {
                        WorldObjects.Add(new WorldObject(_worldTextures[c], new Vector2(col * _tileSize, row * _tileSize)));
                        tiles[col, row] = new Tile(_levelTextures[blankKey], TileCollision.Platform);
                    }
                    else if (_levelTextures.ContainsKey(c))
                    {
                        tiles[col, row] = new Tile(_levelTextures[c], TileCollision.Impassable);
                    }
                   
                    if (c.Equals(playerKey))
                    {
                        player = new Player();
                        playerSpawn = new Vector2(col * _tileSize, row * _tileSize);
                        offset.X = player.Position.X;
                    }
                    else if (c.Equals(exitKey))
                    {
                        exitSpawn = new Vector2(col * _tileSize, row * _tileSize);
                    }
                    else if (c.Equals(bubbleKey))
                    {
                        Bubbles.Add(new Bubble(BubbleType.LargeBubbe, new Vector2(col * _tileSize, row * _tileSize)));
                        BubbleSpawnPoint.Add(new Vector2(col * _tileSize, row * _tileSize));
                        GroundBubbles.Add(new BubbleGround(new Vector2(col * _tileSize, row * _tileSize)));
                        numBubblesSpawn++;
                    }
                    else if (fishKeys.Contains(c))
                    {
                        Fish TempFish;
                        switch (c)
                        {
                            case 'f':
                                TempFish = new Fish(FishType.SmallFish);
                                break;
                            case 'F':
                                TempFish = new Fish(FishType.BigFish);
                                break;
                            case 'M':
                                TempFish = new Fish(FishType.MediumFish);
                                break;
                            default:
                                TempFish = new Fish(FishType.SmallFish);
                                break;
                        }

                        TempFish.LoadContent(content, rand.Next(0, 2));
                        Vector2 CurPosition = new Vector2(col * _tileSize, row * _tileSize);
                        TempFish.Position = CurPosition;
                        Fish.Add(TempFish);
                    }
                    else if (c.Equals(sharkKey))
                    {
                        Shark TempShark = new Shark();
                        TempShark.LoadContent(content, rand.Next(0, 2));
                        Vector2 CurPosition = new Vector2(col * _tileSize, row * _tileSize);
                        TempShark.Position = CurPosition;
                        Shark.Add(TempShark);
                    }
                    
                }
            }

            //load content for world bubbles
            for (int i = 0; i < Bubbles.Count; i++)
            {
                Bubbles[i].LoadContent(_content);
                GroundBubbles[i].LoadContent(_content);
            }

            float bubbleWidth = largeBubble.Width;
            float bubbleHeight = largeBubble.Height;
            for (int i = 0; i < 5; i++)
            {
                Oxygen.Add(new WorldObject(largeBubble, new Vector2(i * bubbleWidth, 0.0f)));
            }

            score = 0;
            lastBubbleSpawn = TimeSpan.FromSeconds(0);

            gameplayMusic = content.Load<Song>(@"Sounds\Music\DST-BeyondTheseForests");
            //gameplayMusic = content.Load<Song>(@"Sounds\Music\DST-Blekinge");
            underwaterNoise = content.Load<Song>(@"Sounds\Music\deep_underwater_ambience_003");
            diamondCollectSoundEffect = content.Load<SoundEffect>(@"Diamonds\Sounds\multimedia_minimise_window_sound_good_for_closing_windows_menus_etc");

            bubblePopSoundEffect = content.Load<SoundEffect>(@"Bubbles\Sounds\short_pop_sound");
            bubbleSpawnSoundEffect = content.Load<SoundEffect>(@"Bubbles\Sounds\Large Bubble-SoundBible.com-1084083477");
            
            
            // Start the music right away
            PlayMusic(gameplayMusic);
            
        }

        #endregion

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

                MediaPlayer.Volume = 0.5f;

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        #region Bounds and collision

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            
            // Prevent escaping past the level ends.
            if (x < 0 || x >= _level.Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= _level.Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * _tileSize, y * _tileSize, _tileSize, _tileSize);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion


        public void Update(GameTime gameTime)
        {

            Vector2 NewPos = Vector2.Zero;
            Vector2 oldPos = player.Position;
            Vector2 difference = player.Position - offset;

            #region Camera Offset
            /*
             * If the player's position is greater than the middle of the screen, move the offset in the opposite direction as player
             */
            //shift x position
            if (player.Position.X < (ScreenWidth * 0.5f))
            {
                offset.X += player.Position.X - ScreenWidth;
            }

            if (player.Position.X > (ScreenWidth * 0.5) && player.Position.X < _levelSize.X - ScreenWidth * 0.5f)
            {
                offset.X += difference.X - ScreenWidth * 0.5f;
                levelStationary = false;
            }
            else
            {
                offset.X += difference.X;
                levelStationary = true;
                player.stationary = true;
                player.movingRight = false;
                player.movingLeft = false;
            }
            //shift y position
            if (player.Position.Y < _levelSize.Y - ScreenHeight * 0.5)
            {
                float levelDifferenceY = player.Position.Y - (_levelSize.Y - ScreenHeight * 0.5f);
                offset.Y += levelDifferenceY - offset.Y;
            }

            #endregion

            if (player.IsJumping && !player.jumpSoundPlayed)
            {
                player.jumpSound.Play();
                player.jumpSoundPlayed = true;
                
            }

             var tileRect = new Rectangle(0, 0, _tileSize, _tileSize);

             #region Collisions
             // Check for collisions
            for (int col = 0; col < _level.Width; col++)
            {
                for (int row = 0; row < _level.Height; row++)
                {
                    char tile = _level.TileAt(col, row);
                    
                    tileRect.X = col* _tileSize;
                    tileRect.Y = row * _tileSize;
                    Vector2 newPosition;
                    
                    //fix character collision if  collides with level
                    if (collisionTiles.Contains(tile) && player.CollidesWith(tileRect, out newPosition))
                    {

                        if (player.Position.Y > newPosition.Y)
                        {
                            player.IsOnGround = true;
                            player.CurrentJumps = 0;
                            if (Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Space) && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.W))
                                player.jumpSoundPlayed = false;
                            if (!player.landSoundPlayed && player.IsOnGround)
                            {
                                player.landSound.Play();
                                player.landSoundPlayed = true;
                            }
                        }
                        player.Position = newPosition;
                    }     
                }
            }

             //check to see if the collision is a platform and fix if player's position is above object
            for (int i = 0; i < WorldObjects.Count; i++)
            {
                Vector2 newPosition;
                    if (player.CollidesWith(WorldObjects[i].CollsionBounds, out newPosition) && (player.CollsionBounds.Bottom - WorldObjects[i].CollsionBounds.Top) <= 3.0f)
                    {
                         player.IsOnGround = true;
                         player.IsOnPlatform = true;
                          player.Position = newPosition;
                          if (Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Space) && Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.W))
                            player.jumpSoundPlayed = false;
                          if (!player.landSoundPlayed)
                          {
                              player.landSound.Play(1, 0, 0);
                              player.landSoundPlayed = true;
                          }
                    }
            }

            if (!player.IsOnGround)
                player.landSoundPlayed = false;

            //check to see if player collides with any dimonds
            for (int i = 0; i < WorldEntities.Count; i++)
            {
                if (WorldEntities[i].Texture.Equals(_worldEntities[diamondKey]) && player.CollsionBounds.Intersects(WorldEntities[i].CollsionBounds))
                {
                    Vector2 pos = WorldEntities[i].Position;
                    WorldEntities.RemoveAt(i);
                    WorldEntities.Add(new WorldObject(_levelTextures[blankKey], pos));
                    Score += DIAMOND_SCORE;
                    diamondCollectSoundEffect.Play(0.08f, -1.0f, 0.0f);
                }

            }

            if (player.HasCollided == true)
                player.CollideWaitTime -= gameTime.ElapsedGameTime;
            if (player.CollideWaitTime <= TimeSpan.FromSeconds(0))
            {
                player.CollideWaitTime = TimeSpan.FromSeconds(2);
                player.HasCollided = false;
            }
            //update the Fish and check for collisions
            for (int i = 0; i < Fish.Count; i++)
            {
                if (!player.HasCollided && !Fish[i].type.Equals(FishType.SmallFish) && Fish[i].CollsionBounds.Intersects(player.CollsionBounds))
                {
                    if (player.Oxygen > 0)
                    {
                        player.Oxygen--;
                        player.Oxygen_Time = TimeSpan.FromSeconds(player.MAX_OXYGEN_TIME);
                        player.HasCollided = true;
                        player.hitSound.Play(1, 0, 0);
                    }
                }
                Fish[i].Update(gameTime);
            }
            //update the Shark
            for (int i = 0; i < Shark.Count; i++)
            {
                if (!player.HasCollided && Shark[i].CollsionBounds.Intersects(player.CollsionBounds))
                {
                    if (player.Oxygen > 0)
                    {
                        player.Oxygen--;
                        player.Oxygen_Time = TimeSpan.FromSeconds(player.MAX_OXYGEN_TIME);
                        player.HasCollided = true;
                        player.hitSound.Play(1, 0, 0);
                    }
                }
                Shark[i].Update(gameTime);
            }

            //check for bubble collisions, update bubbles
            for (int i = 0; i < Bubbles.Count; i++)
            {
                Bubbles[i].Update(gameTime);
               
                if (player.CollsionBounds.Intersects(Bubbles[i].CollsionBounds))
                {
                    if (player.Oxygen < player.MAX_OXYGEN_)
                        player.Oxygen++;
                    else
                        player.Oxygen = player.MAX_OXYGEN_;
                    player.Oxygen_Time = TimeSpan.FromSeconds(player.MAX_OXYGEN_TIME);
                    Bubbles.RemoveAt(i);
                    bubblePopSoundEffect.Play(1, 0, 0);
                }
            }

            for (int i = 0; i < GroundBubbles.Count; i++)
                GroundBubbles[i].Update(gameTime);
             #endregion


            
            for (int i = 0; i < Bubbles.Count; i++)
            {
                if (Bubbles[i].Position.Y < 0)
                {
                    Bubbles.RemoveAt(i);
                }
            }

            if (lastBubbleSpawn > TimeSpan.FromSeconds(MAX_BUBBLE_SPAWN_TIME))
            {
                for (int i = 0; i < BubbleSpawnPoint.Count; i++)
                {
                    Bubbles.Add(new Bubble(BubbleType.LargeBubbe, BubbleSpawnPoint[i]));
                    Bubbles[Bubbles.Count - 1].LoadContent(_content);
                    bubbleSpawnSoundEffect.Play(0.1f, 0.0f, 0.0f);
                }
                lastBubbleSpawn = TimeSpan.FromSeconds(0);
            }

            if (player.Oxygen_Time.TotalSeconds <= 0)
            {
                player.Oxygen--;
                player.Oxygen_Time = TimeSpan.FromSeconds(player.MAX_OXYGEN_TIME);
            }


            player.Oxygen_Time -= gameTime.ElapsedGameTime;
            lastBubbleSpawn += gameTime.ElapsedGameTime;
        }


        public void Draw(DrawContext ctx)
        {
            //clamp camera to level
            offset.X = MathHelper.Clamp(offset.X, 0.0f, _levelSize.X - ctx.Device.Viewport.Width);
            offset.Y = MathHelper.Clamp(offset.Y, _levelSize.Y * -1, 0.0f);

            //clamp player to level
            Vector2 TempPos = player.Position;
            TempPos.X = MathHelper.Clamp(TempPos.X, 0.0f, _levelSize.X);
            player.Position = TempPos;


            ctx.Offset = ctx.Offset - offset;
            Vector2 start = ctx.Offset;


            for (int y = 0; y < _level.Height; y++)
            {
                for (int x = 0; x < _level.Width; x++)
                {

                    char tile = _level.TileAt(x, y);
                    if (_levelTextures.ContainsKey(tile))
                    {
                        Texture2D texture = _levelTextures[tile];

                        var pos = new Vector2(start.X + x * _tileSize, start.Y + y * _tileSize - _tileSize + _tileSize);
                        ctx.Batch.Draw(texture, pos, Color.White);
                    }
                }
            }
            //draw world objects
            for (int i = 0; i < WorldObjects.Count; i++)
            {
                Vector2 textureOffset = new Vector2(WorldObjects[i].Texture.Width * 0.5f, WorldObjects[i].Texture.Height - _tileSize);
                ctx.Batch.Draw(WorldObjects[i].Texture, (WorldObjects[i].Position) + ctx.Offset - textureOffset, Color.White);
            }
            //draw world entities
            for (int i = 0; i < WorldEntities.Count; i++)
            {
                Vector2 textureOffset = new Vector2(WorldEntities[i].Texture.Width * 0.5f, WorldEntities[i].Texture.Height - _tileSize);
                if (WorldEntities[i].Texture.Equals(_worldEntities[diamondKey]))
                    ctx.Batch.Draw(WorldEntities[i].Texture, (WorldEntities[i].Position) + ctx.Offset - textureOffset, Color.FromNonPremultiplied(255, 255, 255, 160));
                else
                    ctx.Batch.Draw(WorldEntities[i].Texture, (WorldEntities[i].Position) + ctx.Offset - textureOffset, Color.White);
            }

            //draw fish
            for (int i = 0; i < Fish.Count; i++)
                Fish[i].Draw(ctx);

            //draw shark
            for (int i = 0; i < Shark.Count; i++)
                Shark[i].Draw(ctx);

            Vector2 playerOffset = new Vector2(0.0f, -3);
            player.offset = playerOffset;
            player.Draw(ctx);

            //draw world bubbles
            for (int i = 0; i < Bubbles.Count; i++)
                Bubbles[i].Draw(ctx);

            for (int i = 0; i < GroundBubbles.Count; i++)
                GroundBubbles[i].Draw(ctx);

        }
    }
}
