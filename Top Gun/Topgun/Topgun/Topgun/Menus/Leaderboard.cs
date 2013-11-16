using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topgun
{
    class Leaderboard : MenuScreen
    {

        private List<string> _scores = new List<string>();

        private Texture2D _background;
        private string levelFile;
        private string newScore;
        private MenuEntry close;
        private MenuEntry next;
        private string newLevel;
        private bool showNext;

        public Leaderboard(string level, int newScore) : base("Leaderboard")
        {
            this.levelFile = level;

            if (newScore == -1)
                this.newScore = null;
            else
                this.newScore = newScore.ToString("00000");
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");
            /// <summary>
            /// Read scores from file
            /// </summary>
            _scores.AddRange(File.ReadAllLines(@"Leaderboards\" + levelFile));
            
            if (newScore != null)
            {
                /// <summary>
                /// Add new score,
                /// Sort descending,
                /// Limit to 10 scores
                /// </summary>
                _scores.Add(newScore);
                _scores.Sort();
                _scores.Reverse();
                _scores.RemoveAt(_scores.Count() - 1);

                /// <summary>
                /// Write scores to file
                /// </summary>
                File.WriteAllLines(@"Leaderboards\" + levelFile, _scores);

                /// <summary>
                /// get next level
                /// </summary>
                newLevel = Path.GetFileNameWithoutExtension(levelFile);
                int oldIndex1 = (int)(newLevel.ToCharArray()[newLevel.Length - 2] - '0') * 10;
                int oldIndex2 =  (int)(newLevel.ToCharArray()[newLevel.Length - 1] - '0');
                int newIndex = oldIndex1 + oldIndex2 + 1;
                newLevel = newLevel.Replace((oldIndex1 + oldIndex2).ToString("00"), newIndex.ToString("00"));
                newLevel = @"Levels\" + newLevel + ".txt";

                showNext = File.Exists(newLevel) ? true : false;

                if (showNext)
                {
                    next = new MenuEntry("Next Level");
                    next.Selected += new EventHandler<PlayerIndexEventArgs>(next_Selected);
                    MenuEntries.Add(next);
                }

                close = new MenuEntry("Level Select");
                close.Selected += new EventHandler<PlayerIndexEventArgs>(close_Selected);
                MenuEntries.Add(close);
            }
            else // Accessed from Main Menu
            {
                close = new MenuEntry("Back");
                close.Selected += new EventHandler<PlayerIndexEventArgs>(back_Selected);
                MenuEntries.Add(close);
            }
        }

        protected override void UpdateMenuEntryLocations()
        {
            base.UpdateMenuEntryLocations();

            if (showNext)
                next.Position = new Vector2(next.Position.X, 525);
            close.Position = new Vector2(close.Position.X, 560);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.WhiteSmoke);

            float scale = (float)ScreenManager.GraphicsDevice.Viewport.Height / (float)_background.Height;
            var size = new Rectangle(0, 0, _background.Width, _background.Height);
            var bgPos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            var origin = new Vector2(_background.Width / 2, _background.Height / 2);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            string level = Path.GetFileNameWithoutExtension(levelFile);
            Vector2 levelPos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 
                ScreenManager.Font.MeasureString(level).X / 2, 100);

            spriteBatch.Begin();

            spriteBatch.Draw(_background, bgPos, size, Color.White, 0, origin, scale, SpriteEffects.None, 0);

                spriteBatch.DrawString(font, level, new Vector2(levelPos.X+2, levelPos.Y+2), new Color(0.0f,0.0f,0.0f,0.85f));
                spriteBatch.DrawString(font, level, levelPos, Color.White);

                for (int i = 0; i < 10; i++)
                {
                    string str = (i + 1).ToString("00") + ") " + _scores[i];
                    Vector2 pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 
                        ScreenManager.Font.MeasureString(str).X / 2, 150 + 35 * i);
                    Color color = _scores[i] == newScore ? Color.Yellow : Color.White;

                    spriteBatch.DrawString(font, str, new Vector2(pos.X+2, pos.Y+2), new Color(0.0f, 0.0f, 0.0f, 0.85f));
                    spriteBatch.DrawString(font, str, pos, color);
                }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            if (newScore == null)
                back_Selected(this, null);
            else
                close_Selected(this, null);
        }

        void close_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
            ScreenManager.AddScreen(new Hiscores(0), null);
        }

        void next_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
            ScreenManager.AddScreen(new GameplayScreen(newLevel), null);
        }

    }
}
