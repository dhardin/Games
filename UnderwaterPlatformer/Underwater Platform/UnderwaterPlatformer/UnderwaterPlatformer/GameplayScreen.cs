using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework;
using Engine.TaskManagement;
using Engine.Sprites;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Engine.Primitives;

namespace UnderwaterPlatformer
{
    class GameplayScreen : GameScreen
    {
        GameLevel _level = new GameLevel();
        Player player = new Player();

        Texture2D _background;

        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;
        ParallaxingBackground bgLayer3;
        ParallaxingBackground bgLayer4;

        List<ParallaxLayer> LayerBackground = new List<ParallaxLayer>();


        private string _levelFile;
        AccelerometerState accelerometerState;
        Vector2 Offset;
        PrimitiveBatch primitiveBatch;// = new PrimitiveBatch(ScreenManager.GraphicsDevice;);
        Color tempColor = Color.LawnGreen;
        SpriteFont gameFont;
        SpriteFont scoreFont;
        AnchoredText levelWin, levelLose, scoreDisplay;
        bool LoseLevel, WinLevel;
        const int MAX_WIN_LOSE_WAIT_TIME = 5;
        TimeSpan WinLoseWaitTime = TimeSpan.FromSeconds(MAX_WIN_LOSE_WAIT_TIME);
        string currentScore = new string('0', 5);
        // Get number of levels
        string[] levelFiles = Directory.GetFiles(@"Levels", "*.txt");
        HighScore highScores = new HighScore();
        private string previousScore;

        float scaleBackground, scaleBubble;
        
        

        public GameplayScreen(string levelFile, string preScore)
        {
            _levelFile = levelFile;
            previousScore = preScore;
        }

        public string GetNextLevel()
        {
            string currentFile = "";
            string nextFile;
            int nextFileNumber;
            //get the current level
            for (int i = 0; i < _levelFile.Length; i++)
            {
                if (_levelFile[i] >= 48 && _levelFile[i] <= 57)
                    currentFile += _levelFile[i];
            }
            Int32.TryParse(currentFile, out nextFileNumber);
            nextFileNumber++;
            nextFile = nextFileNumber.ToString();

            //get the next level
            for (int i = 0; i < levelFiles.Length; i++)
            {
                if (levelFiles[i].Contains(nextFile))
                    return levelFiles[i];
            }
            

           return "";
        }

     

        public override void Activate(bool instancePreserved)
        {
            player.LoadContent(ScreenManager.Game.Content);
            _level.LoadContent(ScreenManager.Game.Content, _levelFile);
            _level.player = player;
            player.Position = _level.PlayerSpawn;
            player.Level = _level;
            _level.ScreenHeight = base.ScreenManager.GraphicsDevice.Viewport.Height;
            _level.ScreenWidth = base.ScreenManager.GraphicsDevice.Viewport.Width;

            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            bgLayer3 = new ParallaxingBackground();
            bgLayer4 = new ParallaxingBackground();

            gameFont = ScreenManager.Game.Content.Load<SpriteFont>("SpriteFont2");
            scoreFont = ScreenManager.Game.Content.Load<SpriteFont>("SpriteFont1");

            currentScore = previousScore;
            int tempScore = _level.Score;
            Int32.TryParse(previousScore, out tempScore);
            _level.Score = tempScore;
            //currentScore = previousScore;
            currentScore = currentScore.PadLeft(5 - currentScore.Length, '0');
            Vector2 position = new Vector2(_level.ScreenWidth * 0.5f, _level.ScreenHeight * 0.5f);
            Vector2 topRightPosition = new Vector2(_level.ScreenWidth, 0);
            levelLose = new AnchoredText(gameFont, " Game Over", position, TextAnchor.MiddleCenter);
            levelWin = new AnchoredText(gameFont, "Level Complete", position, TextAnchor.MiddleCenter);
            scoreDisplay = new AnchoredText(scoreFont, "Score: " + currentScore, topRightPosition,TextAnchor.TopRight);
            LoseLevel = false;
            WinLevel = false;

            Offset.Y = -1 * (_level._level.Height * _level._tileSize - _level.ScreenHeight);
            
            _background = ScreenManager.Game.Content.Load<Texture2D>(@"Background\Backdrop");
            // Load the parallaxing background
            bgLayer1.Initialize(ScreenManager.Game.Content, @"ParallaxBackgrounds\WATERBACKGROUND", (int)_level._levelSize.X, -4);
            bgLayer2.Initialize(ScreenManager.Game.Content, @"Background\layer2", (int)_level._levelSize.X, -2);
            bgLayer3.Initialize(ScreenManager.Game.Content, @"Background\layer3", (int)_level._levelSize.X, -3);
            bgLayer4.Initialize(ScreenManager.Game.Content, @"ParallaxBackgrounds\BACKGROUNDREEF", (int)_level._levelSize.X, -1);

            scaleBackground = 1.0f +  (_background.Height / (_level.Height * _level._tileSize));
            Accelerometer.Initialize();
            base.Activate(instancePreserved);

            primitiveBatch = new PrimitiveBatch(ScreenManager.GraphicsDevice);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            
            _level.Update(gameTime);
            // Update the parallaxing background
            
            if (player.movingRight)
            {
                bgLayer1.Speed = -2.5f;
                bgLayer2.Speed = -3f;
                bgLayer3.Speed = -2f;
                bgLayer4.Speed = -1f;
            }
            else if (player.movingLeft)
            {
                bgLayer1.Speed = -0.45f;
                bgLayer2.Speed = -0.5f;
                bgLayer3.Speed = -0.4f;
                bgLayer4.Speed = 1f;
               
            }
            else if (player.stationary || _level.levelStationary)
            {
                bgLayer1.Speed = -1.5f;
                bgLayer2.Speed = -2f;
                bgLayer3.Speed = -1;
                bgLayer4.Speed = -0;
            }
            bgLayer1.Update();
            bgLayer2.Update();
            bgLayer3.Update();
            bgLayer4.Update();
            currentScore = _level.Score + "";
            currentScore = currentScore.PadLeft(5 - currentScore.Length, '0');
            scoreDisplay.Text = "Score: " + currentScore;


            if (!WinLevel && !LoseLevel)
            {
                player.Update(gameTime, Keyboard.GetState(PlayerIndex.One), GamePad.GetState(PlayerIndex.One), accelerometerState, DisplayOrientation.Default);
                scaleBubble = (float)(player.Oxygen_Time.TotalSeconds / player.MAX_OXYGEN_TIME);
            }
            else
                player.Update(gameTime, accelerometerState, DisplayOrientation.Default);

            if (player.Position.Y > (_level._level.Height * _level._tileSize + player.CollsionBounds.Height))
            {
                LoseLevel = true;
            }
            if (Math.Abs((_level.exitSpawn - player.Position).X) <= 30.0f && Math.Abs((_level.exitSpawn - player.Position ).Y) <= 30.0f)
            {
                WinLevel = true;
                player.invincibilityFlicker = false;
            }

            if (player.Oxygen.Equals(0) && !WinLevel)
            {
                LoseLevel = true;
                player.invincibilityFlicker = false;
            }

            if (player.IsOnGround && !player.IsAlive)
            {
                player.landSoundPlayed = true;
            }

            if (WinLevel || LoseLevel)
            {
                WinLoseWaitTime -= gameTime.ElapsedGameTime;
                if (WinLoseWaitTime.TotalSeconds <= 0)
                {
                    if (LoseLevel)
                    {
                        highScores.AddScore(currentScore);
                        currentScore = "0";
                        ExitScreen();
                    }
                    else if (WinLevel && !GetNextLevel().Equals(""))
                    {
                        ScreenManager.AddScreen(new GameplayScreen(GetNextLevel(), currentScore), ControllingPlayer);
                        ExitScreen();
                    }
                    else
                    {
                        
                        highScores.AddScore(currentScore);
                        currentScore = "0";
                        ExitScreen();
                    }
                }
            }

           

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Aqua);

            DrawContext ctx = new DrawContext();
            ctx.Offset = Offset;
            ctx.Batch = ScreenManager.SpriteBatch;
            ctx.Blank = ScreenManager.BlankTexture;
            ctx.Device = ScreenManager.GraphicsDevice;
            ctx.Time = gameTime;

            ctx.Batch.Begin();
            ctx.Batch.Draw(_background, new Vector2(0.0f, ctx.Device.Viewport.TitleSafeArea.Y), null, Color.White, 0f, Vector2.Zero, scaleBackground, SpriteEffects.None, 0f);
            bgLayer1.Draw(ctx.Batch);
            bgLayer2.Draw(ctx.Batch);
            bgLayer3.Draw(ctx.Batch);
            bgLayer4.Draw(ctx.Batch);

            _level.Draw(ctx);
            scoreDisplay.Draw(ctx.Batch);

            if (LoseLevel)
                levelLose.Draw(ctx.Batch);
            if (WinLevel)
                levelWin.Draw(ctx.Batch);

            //draw oxygen
            for (int i = 0; i < player.Oxygen - _level.numBubblesGone; i++)
            {
                Vector2 offset = new Vector2(_level.Oxygen[i].Position.X + ctx.Device.Viewport.TitleSafeArea.X + ctx.Device.Viewport.Width / 3, _level.Oxygen[i].Position.Y + ctx.Device.Viewport.TitleSafeArea.Y);
                if (i < player.Oxygen - _level.numBubblesGone - 1)
                    ctx.Batch.Draw(_level.Oxygen[i].Texture, offset, Color.FromNonPremultiplied(255, 255, 255, (int)(150)));
                else
                    ctx.Batch.Draw(_level.Oxygen[i].Texture, offset , null, Color.FromNonPremultiplied(255, 255, 255, (int)(150)), 0f,Vector2.Zero, scaleBubble, SpriteEffects.None, 0.0f);
           
            }

            ctx.Batch.End();

            base.Draw(gameTime);

        }
        public void DrawBoundingBox(BoundingBox box, Vector2 offset, Color color)
        {

            // tell the primitive batch to start drawing lines
            primitiveBatch.Begin(PrimitiveType.LineList);

            // from the top left to the top right
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Min.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Min.Y), color);

            // from the top right to the bottom right
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Min.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Max.Y), color);

            // from the bottom right to the bottom left
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Max.X, box.Max.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Max.Y), color);

            // from the bottom left to the top left
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Max.Y), color);
            primitiveBatch.AddVertex(
                offset + new Vector2(box.Min.X, box.Min.Y), color);

            // and we're done.
            primitiveBatch.End();
        }
    }
}
