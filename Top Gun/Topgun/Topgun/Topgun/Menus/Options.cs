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
using Engine.GameStateManagerment;
using Engine.Game;
using Engine.Screens;
using System.Diagnostics;

namespace Topgun
{

    class Options : MenuScreen
    {
        Texture2D _background;
        Settings _settings = new Settings();
        MenuEntry fullScreen, sound, music;
        private bool _fullscreen;

        public Options() : base("Options")
        {
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");

            _settings.Load();
            _fullscreen = _settings.FullScreen;

            var quit = new MenuEntry("Back");
            fullScreen  = new MenuEntry("FullScreen:       " + _settings.FullScreen.ToString());
            sound       = new MenuEntry("Sound Effects:    " + _settings.Sound.ToString());
            music       = new MenuEntry("Background Music: " + _settings.Music.ToString());

            sound.Selected += new EventHandler<PlayerIndexEventArgs>(sound_Selected);
            music.Selected += new EventHandler<PlayerIndexEventArgs>(music_Selected);
            fullScreen.Selected += new EventHandler<PlayerIndexEventArgs>(fullScreen_Selected);
            quit.Selected += new EventHandler<PlayerIndexEventArgs>(quit_Selected);

            MenuEntries.Add(sound);
            MenuEntries.Add(music);
            MenuEntries.Add(fullScreen);
            MenuEntries.Add(quit);

        }

        void music_Selected(object sender, PlayerIndexEventArgs e)
        {
            _settings.Music = _settings.Music ? false : true;
            music.Text = "Background Music: " + _settings.Music.ToString();
        }

        void sound_Selected(object sender, PlayerIndexEventArgs e)
        {
            _settings.Sound = _settings.Sound ? false : true;
            sound.Text = "Sound Effects:    " + _settings.Sound.ToString();
        }

        void fullScreen_Selected(object sender, PlayerIndexEventArgs e)
        {
            _settings.FullScreen = _settings.FullScreen ? false : true;
            fullScreen.Text = "FullScreen:       " + _settings.FullScreen.ToString();
        }

        void quit_Selected(object sender, PlayerIndexEventArgs e)
        {
            _settings.Save();
            if (_settings.FullScreen != _fullscreen)
                Restart();
            else
                ExitScreen();
        }

        private void Restart()
        {
            ScreenManager.Game.Exit();
            Process.Start("Topgun.exe");
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.WhiteSmoke);

            float scale = (float)ScreenManager.GraphicsDevice.Viewport.Height / (float)_background.Height;
            var size = new Rectangle(0, 0, _background.Width, _background.Height);
            var pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            var origin = new Vector2(_background.Width / 2, _background.Height / 2);

            ScreenManager.SpriteBatch.Begin();
                ScreenManager.SpriteBatch.Draw(_background, pos, size, Color.White, 0, origin, scale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
