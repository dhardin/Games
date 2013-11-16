///<summary>
/// This program uses the A* algorithm to move an NPC from a location to
/// the object (a star) which the NPC is trying to find's location
///</summary>


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
using GameStateManagement;


namespace AStarPathfinding
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        //ScreenManager screenManager;
       // IScreenFactory screenFactory;
        SpriteBatch spriteBatch;

        //map object which stores map info
        Map map = new Map();

        //pathfinder class used to figure out best path to objet
        Pathfinder pathfinder;

        //class represents our star object
        StarNPC star;

        //represents our NPC which transverses accross the map
        NPC npc_pathfinder;

        //star texture
        Texture2D starTexture;

        //GUI textures
        Texture2D b_button_Texture;
        Texture2D y_button_Texture;
        Texture2D x_button_Texture;
        Texture2D whiteRectangle;

        //textures which represent the map
        List<Texture2D> map_textures;

        //center of the next map texture
        Vector2 map_texture_origin;

        //vectors used to determine distance left to traveld {X: , Y:}
        Vector2 dataVector;
        Vector2 difference;
        
        //fonts used in GUI
        SpriteFont font;
        SpriteFont dataFont;
        //stores the run time of algorithm
        string status;

        //Button objects
        Button b_button;
        Button y_button;
        Button x_button;

        //stores a list of vectors returned by pathfinder
        List<Vector2> path;
        //stores a list of spline vectors returned by pathfinder
        List<Vector2> splinePath;

        //list of open locations on the map
        List<Position> openLocations;

        //what index in the list the path is at
        int pathIndex;
        int splinePathIndex;
        //starting point on the map
        Point pathPoint;

        //end point on the map where you want to transverse to
        Point startPoint;

        //whether the npc has found the star or not
        bool isStarFound;

        //whether or not the player has played
        bool hasPlayed;

        // The music played during gameplay
        Song gameplayMusic;

        // The sound used when the NPC finds the star
        SoundEffect starFound;

        //returns the land type the NPC is currently on
        string landType;

        //used for pause button
        private bool paused = false;
        private bool pauseKeyDown = false;

        //used to draw heuristic
        BasicEffect basicEffect;
        VertexPositionColor[] vertices;

        #endregion

        #region Initialization
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create the screen factory and add it to the Services
            //screenFactory = new ScreenFactory();
           // Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            //screenManager = new ScreenManager(this);
            //Components.Add(screenManager);
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// </summary>
        protected override void Initialize()
        {
            //show mouse cursor
            this.IsMouseVisible = true;

            //setup our pathfinder object to our map object
            pathfinder = new Pathfinder(map);

            //set our starting index when searching through our path list
            pathIndex = 0;
            splinePathIndex = 0;
            
            //create our npc pathfinder
            npc_pathfinder = new NPC();
            //set what the default land type that the npc is positioned on
            landType = "";

            //setup our basic effects for drawing heuristics
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;

            //set our star found status to false since we're playing for the first time
            isStarFound = false;

            pathPoint = new Point();
            openLocations = new List<Position>();
            splinePath = new List<Vector2>();
            star = new StarNPC();
            b_button = new Button();
            y_button = new Button();
            x_button = new Button();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load gameplay music
            gameplayMusic = Content.Load<Song>("sound/chocobo_music");

            //load sound effect
            starFound = Content.Load<SoundEffect>("sound/chocobo_wark");
            
            //load map resources
            map_textures = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Game World/tree"),
                Content.Load<Texture2D>("Game World/grass"),
                Content.Load<Texture2D>("Game World/light_marsh"),
                Content.Load<Texture2D>("Game World/marsh"),
                Content.Load<Texture2D>("Game World/rock"),
            };

            map.SetTextures(map_textures);
            graphics.PreferredBackBufferHeight = map.Height * map_textures[0].Height;
            graphics.PreferredBackBufferWidth = map.Width * map_textures[0].Width;
            graphics.ApplyChanges();
            b_button.position.X = 70;
            b_button.position.Y = graphics.PreferredBackBufferHeight - 30;
            y_button.position.X = 130;
            y_button.position.Y = graphics.PreferredBackBufferHeight - 30;
            x_button.position.X = 230;
            x_button.position.Y = graphics.PreferredBackBufferHeight - 30;
            //place star and load star resources
            starTexture = Content.Load<Texture2D>("Star/star");
            PlaceStar();
            star.Initialize(starTexture, star.Position);

            // Load the player resources
            List<Animation> npc_pathfinderAnimations = new List<Animation>();
            Animation npc_up = new Animation();
            Animation npc_down = new Animation();
            Animation npc_right = new Animation();
            Animation npc_left = new Animation();
            npc_pathfinderAnimations.Add(npc_down);
            npc_pathfinderAnimations.Add(npc_up);
            npc_pathfinderAnimations.Add(npc_right);
            npc_pathfinderAnimations.Add(npc_left);
            Texture2D npc_pathfinderTextureUp = Content.Load<Texture2D>("Chocobo/chocobo_up");
            npc_up.Initialize(npc_pathfinderTextureUp, Vector2.Zero, 13, 24, 4, 120, Color.White, 1f, true);
            Texture2D npc_pathfinderTextureDown = Content.Load<Texture2D>("Chocobo/chocobo_down");
            npc_down.Initialize(npc_pathfinderTextureDown, Vector2.Zero, 13, 24, 4, 120, Color.White, 1f, true);
            Texture2D npc_pathfinderTextureRight = Content.Load<Texture2D>("Chocobo/chocobo_right");
            npc_right.Initialize(npc_pathfinderTextureRight, Vector2.Zero, 24, 25, 4, 120, Color.White, 1f, true);
            Texture2D npc_pathfinderTextureLeft = Content.Load<Texture2D>("Chocobo/chocobo_left");
            npc_left.Initialize(npc_pathfinderTextureLeft, Vector2.Zero, 24, 25, 4, 120, Color.White, 1f, true);

            Vector2 npcPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y);
            npc_pathfinder.Initialize(npc_pathfinderAnimations, npcPosition);
            
            
            //load controller resources
            b_button_Texture = Content.Load<Texture2D>("Xbox 360/xboxControllerButtonB");
            b_button.Initialize(b_button_Texture, b_button.Position);
            y_button_Texture = Content.Load<Texture2D>("Xbox 360/xboxControllerButtonY");
            y_button.Initialize(y_button_Texture, y_button.Position);
            x_button_Texture = Content.Load<Texture2D>("Xbox 360/xboxControllerButtonX");
            x_button.Initialize(x_button_Texture, x_button.Position);

            whiteRectangle = new Texture2D(graphics.GraphicsDevice, map_textures[0].Width, map_textures[0].Height);
            Color[] data = new Color[whiteRectangle.Width * whiteRectangle.Height];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.White;
            whiteRectangle.SetData(data);

            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, graphics.PreferredBackBufferWidth,     // left, right
                graphics.PreferredBackBufferHeight, 0,    // bottom, top
                0, 1);                                         // near, far plane


            // Load the game font
            font = Content.Load<SpriteFont>("gameFont");
            dataFont = Content.Load<SpriteFont>("AStarFont");

            // Start the music right away
            PlayMusic(gameplayMusic);
        }

        /// <summary>
        /// UnloadContent is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
        {
            whiteRectangle.Dispose();
        }

        #endregion

        #region Public/Private Methods
        /// <summary>
        /// set pause game time
        /// </summary>
        private void BeginPause(bool UserInitiated)
        {
            paused = true;
            //TODO: Pause audio playback
            //TODO: Pause controller vibration
        }
        /// <summary>
        /// end pause game time
        /// </summary>
        private void EndPause()
        {
            //TODO: Resume audio
            //TODO: Resume controller vibration
            paused = false;
        }
        /// <summary>
        /// check to see if user paused game
        ///  </summary>
        private void checkPauseKey(KeyboardState keyboardState, GamePadState gamePadState)
        {
            bool pauseKeyDownThisFrame = (keyboardState.IsKeyDown(Keys.P) ||
                (gamePadState.Buttons.Start == ButtonState.Pressed));
            // If key was not down before, but is down now, we toggle the
            // pause setting
            if (!pauseKeyDown && pauseKeyDownThisFrame)
            {
                if (!paused)
                {
                    BeginPause(true);
                }
                else
                    EndPause();
            }
            pauseKeyDown = pauseKeyDownThisFrame;
        }
        /// <summary>
        /// check to see if a guide was used to pause the game
        ///  </summary>
        //private void checkPauseGuide()
        //{
        //    // Pause if the Guide is up
        //    if (!paused && Guide.IsVisible)
        //        BeginPause(false);
        //    // If we paused for the guide, unpause if the guide
        //    // went away
        //    else if (paused && pausedForGuide && !Guide.IsVisible)
        //        EndPause();
        //}
        /// <summary>
        /// Randomly places position of star in empty (grass) cell
        /// </summary>
        public void PlaceStar()
        {
            //generate a randome number to use when placing star
            Random randomNum = new Random();
            //get the "viewable" map height and width
            int mapHeight = GraphicsDevice.Viewport.Height/map_textures[0].Height;
            int mapWidth = GraphicsDevice.Viewport.Width/map_textures[0].Width;

            //create a list of all available spots (grass in this case)
            for (int row = 0; row < mapHeight; row++)
            {
                //offset column by 1 so we can place our NPC that will search for the star
                for (int column = 0; column < mapWidth; column++)
                {
                    //if location on map is walkable, add it's x & y coordinates to the list
                    if (map.GetIndex(column, row) != 0)
                    {
                        Position p = new Position(column * map_textures[0].Width, row * map_textures[0].Height); //multiplay by map textures width and height for next position
                        openLocations.Add(p);//add the walkable map tile position as an open location for star placement
                    }
                }
            }
            //get a random available position
            int random = randomNum.Next(0, openLocations.Count -1);
            //set star's position to the random available position on the map
            star.position.X = openLocations[random].X;
            star.position.Y = openLocations[random].Y;
            //need to convert star position to int for finding a path to star
            long tempX = (long)star.Position.X;
            long tempY = (long)star.Position.Y;
            int starPositionX = (int)tempX / map_textures[0].Width;
            int starPositionY = (int)tempY / map_textures[0].Height;
            //need to convert the NPC's position to int for finding path to star
            tempX = (long)npc_pathfinder.Position.X;
            tempY = (long)npc_pathfinder.Position.Y;
            int npc_pathfinderPositionX = (int)tempX / map_textures[0].Width;
            int npc_pathfinderPositionY = (int)tempY / map_textures[0].Height;
            pathPoint.X = starPositionX;
            pathPoint.Y = starPositionY;
            startPoint.X = npc_pathfinderPositionX;
            startPoint.Y = npc_pathfinderPositionY;
            //initialize list to path from the NPC to the star's location
            DateTime now = DateTime.Now;

            path = pathfinder.FindPath(startPoint, pathPoint);
            splinePath = pathfinder.SplineTransform(ref path);
            TimeSpan delta = DateTime.Now - now;
            status = delta.ToString();
            
        }

        public void replaceStar()
        {
            Random randomNum = new Random();
            //get a random available position
            //get a random available position
            int random = randomNum.Next(0, openLocations.Count - 1);
            //set star's position to the random available position on the map
            star.position.X = openLocations[random].X;
            star.position.Y = openLocations[random].Y;
            //need to convert star position to int for finding a path to star
            long tempX = (long)star.Position.X;
            long tempY = (long)star.Position.Y;
            int starPositionX = (int)tempX / map_textures[0].Width;
            int starPositionY = (int)tempY / map_textures[0].Height;
            //need to convert the NPC's position to int for finding path to star
            tempX = (long)npc_pathfinder.Position.X;
            tempY = (long)npc_pathfinder.Position.Y;
            int npc_pathfinderPositionX = (int)tempX / map_textures[0].Width;
            int npc_pathfinderPositionY = (int)tempY / map_textures[0].Height;
            pathPoint.X = starPositionX;
            pathPoint.Y = starPositionY;
            startPoint.X = npc_pathfinderPositionX;
            startPoint.Y = npc_pathfinderPositionY;
            //initialize list to path from the NPC to the star's location
            path = pathfinder.FindPath(startPoint, pathPoint);
            splinePath = pathfinder.SplineTransform(ref path);
        }

        public int GetMovePenaly(Vector2 nextMapTile)
        {
            //set temp values to store the conversion type to int
            int tempX;
            int tempY;
            //store NPC's current posistion on the map
            long mapPositionX = (long)npc_pathfinder.Position.X;
            long mapPositionY = (long)npc_pathfinder.Position.Y;
            //store the standard map tile size
            float tileWidth = map_textures[0].Width;
            float tileHeight = map_textures[0].Height;

            //initialize default land type to grass (i.e., no move penalty)
            int land = 0;

            //see if NPC's current X coordinates is less than the next map tile's X coordinates
             if (npc_pathfinder.Position.X < nextMapTile.X)
             {
                 //set the desination map X position to the next map tile's X coordinates
                 mapPositionX = (long)(npc_pathfinder.Position.X + tileWidth);
             }
             //see if the NPC's current X coordinates are greater than the next map tile's X coordinates
             else if (npc_pathfinder.Position.X > nextMapTile.X)
             {
                 //set the desination map X position to the next map tile's X coordinates
                 mapPositionX = (long)(npc_pathfinder.Position.X - tileWidth);
             }
             //see if the NPC's current Y coorinates are less than the next map tile's Y coordinates
             if (npc_pathfinder.Position.Y < nextMapTile.Y)
             {
                 //set the desination map Y position to the next map tile's Y coordinates
                 mapPositionY = (long)(npc_pathfinder.Position.Y + tileHeight);
             }
             //see if the NPC's current Y coordinates are greater than the next map tile's Y coordinates
             else if (npc_pathfinder.Position.Y > nextMapTile.Y)
             {
                 mapPositionY = (long)(npc_pathfinder.Position.Y - tileHeight);
             }
            //convert the next map position's X & Y coordinates to acceptable when checking map array
             tempX = (int)mapPositionX / map_textures[0].Width;
             tempY = (int)mapPositionY / map_textures[0].Height;
            //set the land type to the next map tile's land type and return the move penalty assosiated with that map type
            //for debugging purposes, a switch statement is used so we can set the cooresponding string to the land type
            //you may just return the land integer if you are not debugging and have numberous land types
             land = map.GetIndex(tempX, tempY);
             switch (land)
             {
                //land type is light marsh
                 case 2:
                     landType = "Light Marsh";
                     return 2;
                //land type is marsh
                 case 3:
                     landType = "Marsh";
                     return 3;
                 //land type is rock
                 case 4:
                     landType = "Rock";
                     return 4;
                //default land type is grass
                 default:
                     landType = "Grass";
                     return 0;
             }

        }
        /// <summary>
        /// Find move npc pathfind towards the star in the map 
        /// </summary>
        public void npcPathfind()
        {
            //move npc to the next pathfinder position in the list
            //need to add or subtract the destination position depending on value of npc position
            if (splinePath.Count > splinePathIndex)
            {
                npc_pathfinder.Move(splinePath[splinePathIndex], GetMovePenaly(splinePath[splinePathIndex]));
            }
            //if (path.Count > pathIndex)
            //{
            //    npc_pathfinder.Move(path[pathIndex], GetMovePenaly(path[pathIndex]));
            //}
            //NPC found the star!
            else
                isStarFound = true;
        }

        public void ColorPath()
        {
            int width = map_textures[0].Width;
            int height = map_textures[0].Height;
            long tempX = 0;
            long tempY = 0;

            GraphicsDevice.Clear(Color.White);
            for (int i = 0; i < path.Count; i++)
            {
                tempX = (long)path[i].X;
                tempY = (long)path[i].Y;
                // Option One (if you have integer size and coordinates)
                spriteBatch.Draw(whiteRectangle, new Rectangle((int)tempX, (int)tempY, width, height), Color.Red * 0.3f);
            }
        }

        public void DrawHeuristic()
        {
            vertices = new VertexPositionColor[2];
            vertices[0].Position = new Vector3(npc_pathfinder.Position.X, npc_pathfinder.Position.Y, 0);
            vertices[0].Color = Color.LightGreen;
            vertices[1].Position = new Vector3(star.position.X, star.position.Y, 0);
            vertices[1].Color = Color.LightSkyBlue;

            //Draw Lines
            basicEffect.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawUserPrimitives
            <VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }

 

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

#endregion

        #region Update and Draw
        /// <summary>
        /// Allows the game to run logic such as updating the world.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            checkPauseKey(Keyboard.GetState(PlayerIndex.One), GamePad.GetState(PlayerIndex.One));

            //checkPauseGuide();
            if (!paused)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }
                //check to see if the NPC found the star
                if ((!isStarFound))
                {
                    //continue to move to the next point on the path
                    npcPathfind();
                    //update animations and position
                    npc_pathfinder.Update(gameTime);
                    //update data vector positioning and the difference vector value
                    dataVector.X = npc_pathfinder.Position.X - (dataFont.MeasureString("Distance: " + difference).X / 2);
                    dataVector.Y = npc_pathfinder.Position.Y + npc_pathfinder.Height;
                    difference = npc_pathfinder.Position - star.Position;
                }

                //if player has has not reached star position
                if (splinePathIndex < splinePath.Count() && npc_pathfinder.Position.Equals(splinePath[splinePathIndex]))
                {
                    splinePathIndex++;
                }
                //if (pathIndex < path.Count() && npc_pathfinder.Position.Equals(path[pathIndex]))
                //{
                //    pathIndex++;
                //}
                //see if player wishes to reset path find
                if ((GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Space)) && isStarFound)
                {
                    //reset star position, status, path, and star found sound play variable
                    replaceStar();
                    pathIndex = 0;
                    splinePathIndex = 0;
                    isStarFound = false;
                    hasPlayed = false;
                    npc_pathfinder.NPC_Animation[npc_pathfinder.NPC_Direction].CurrentFrame = 0;

                }
                if (isStarFound && hasPlayed == false)
                {
                    //NPC has found the star.  Play success sound and set variable so sound doesn't repeat
                    starFound.Play();
                    hasPlayed = true;
                    npc_pathfinder.NPC_Animation[npc_pathfinder.NPC_Direction].CurrentFrame = 0;
                }
               
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();

            map.Draw(spriteBatch);
            ColorPath();
            
            //Draw the NPC's current coordinates, current land type NPC is on and reset star instruction
            
            star.Draw(spriteBatch);
            npc_pathfinder.Draw(spriteBatch);
            spriteBatch.DrawString(dataFont, "Distance: " + difference, dataVector, Color.White);
            spriteBatch.DrawString(font, "NPC Coordinates", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(font, "X: " + (npc_pathfinder.Position.X / 32) + " Y: " + (npc_pathfinder.Position.Y / 32), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 12), Color.White);
            spriteBatch.DrawString(font, "Land: " + landType, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 24), Color.White);
            spriteBatch.DrawString(font, "Movement Speed: " + (npc_pathfinder.NPC_MoveSpeed - npc_pathfinder.NPC_MovePenalty) + " " + status, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 36), Color.White);
            spriteBatch.DrawString(font, "Reset Star", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.Height - 32), Color.White);
            spriteBatch.DrawString(font, "Step", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.Height - 32), Color.White);
            spriteBatch.DrawString(font, "Free", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 200, GraphicsDevice.Viewport.Height - 32), Color.White);
            if (paused)
                spriteBatch.DrawString(font, "P A U S E D", new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2), Color.White);
            b_button.Draw(spriteBatch);
            x_button.Draw(spriteBatch);
            y_button.Draw(spriteBatch);
            
            spriteBatch.End();
            DrawHeuristic();
            
            base.Draw(gameTime);
        }

        #endregion
    }
}
