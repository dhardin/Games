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
    class Hiscores : MenuScreen
    {
        private int screenIndex;
        private Texture2D _background;

        public Hiscores(int ScreenIndex)
            : base("Hiscores")
        {
            screenIndex = ScreenIndex;
        }

        public override void Activate(bool instancePreserved)
        {
 	        base.Activate(instancePreserved);

            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");

            // Get number of levels
            string[] levelFiles = Directory.GetFiles(@"Leaderboards", "*.txt");
            int numLevels = levelFiles.Length;

            ///<summary>
            /// Back 
            ///</summary>
            MenuEntry back = new MenuEntry("Back");
            back.Selected += new EventHandler<PlayerIndexEventArgs>(back_Selected);
            MenuEntries.Add(back);

            ///<summary>
            /// Levels 
            ///</summary>
            int firstItem = screenIndex * 10;
            int lastItem = firstItem + 10;
            for (int i = firstItem; i < lastItem; i++)
            {
                if (i < numLevels)
                {
                    string levelFile = levelFiles[i];
                    var menuEntry = new MenuEntry(Path.GetFileNameWithoutExtension(levelFile));
                    menuEntry.Selected += (s, e) => play_Selected(Path.GetFileName(levelFile));

                    MenuEntries.Add(menuEntry);
                }
                else break;
            }

            ///<summary>
            /// Next
            ///</summary>
            if ((screenIndex + 1) < Math.Ceiling((float)numLevels / 10))
            {
                MenuEntry next = new MenuEntry("Next");
                next.Selected += new EventHandler<PlayerIndexEventArgs>(next_Selected);
                MenuEntries.Add(next);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.WhiteSmoke);

            float scale = (float)ScreenManager.GraphicsDevice.Viewport.Height / (float)_background.Height;
            var size = new Rectangle(0, 0, _background.Width, _background.Height);
            var bgPos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            var origin = new Vector2(_background.Width / 2, _background.Height / 2);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(_background, bgPos, size, Color.White, 0, origin, scale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            back_Selected(this, null);
        }

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
            if (screenIndex <= 0)
                ScreenManager.AddScreen(new MainMenu(), null);
            else
                ScreenManager.AddScreen(new Hiscores(--screenIndex), null);
        }

        void play_Selected(string levelFile)
        {
            ExitScreen();
            ScreenManager.AddScreen(new Leaderboard(levelFile, -1), ControllingPlayer);
        }

        void next_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
            ScreenManager.AddScreen(new Hiscores(++screenIndex), null);
        }

    }
}
