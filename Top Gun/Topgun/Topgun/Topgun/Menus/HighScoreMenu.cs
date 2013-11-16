using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Topgun
{
    class HighScoreMenu : MenuScreen
    {
        private HighScore highScores;
        Texture2D _background;

        SoundEffect MenuItemSelect;
        SoundEffect MenuItemSwitch;

        public HighScoreMenu() : base ("High Scores")
        {
             highScores = new HighScore();
            

             for (int i = 0; i < highScores.HighScores.Count(); i++)
             {
                 string scoreEntry = highScores.HighScores[i];
                 MenuEntry highScore = new MenuEntry(scoreEntry.PadRight(10));
                 highScore.Selected += new EventHandler<PlayerIndexEventArgs>(score_selected);
                 MenuEntries.Add(highScore);
             }

             MenuEntry Clear = new MenuEntry("Clear");
             Clear.Selected += new EventHandler<PlayerIndexEventArgs>(Clear_Selected);
             MenuEntries.Add(Clear);
             MenuEntry Back = new MenuEntry("Back");
             Back.Selected += new EventHandler<PlayerIndexEventArgs>(back_Selected);
             MenuEntries.Add(Back);

            

            
        }

        public override void Activate(bool instancePreserved)
        {
            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");
        }

        void Clear_Selected(object sender, PlayerIndexEventArgs e)
        {
            highScores.Erase();
            highScores.AddScore("0", PlayerIndex.One);
            highScores.AddScore("0", PlayerIndex.Two);
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new HighScoreMenu(), null);
        }
        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
           // MenuItemSelect.Play();
            ExitScreen();
        }
        void score_selected(object sender, PlayerIndexEventArgs e)
        {
            //MenuItemSwitch.Play();
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

    }
}



   