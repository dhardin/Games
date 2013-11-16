using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Input;

namespace Topgun
{
    class MainMenu : MenuScreen
    {
        Texture2D _background;

        SoundEffect MenuItemSelect;
        SoundEffect MenuItemSwitch;

        public MainMenu()
            : base("")
        {

        }

        public override void Activate(bool instancePreserved)
        {
            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");

            var play = new MenuEntry("Play");
            var quit = new MenuEntry("Exit");
            var hiscores = new MenuEntry("Hiscores");
            var credits = new MenuEntry("Credits");

            play.Selected += new EventHandler<PlayerIndexEventArgs>(play_Selected);
            quit.Selected += new EventHandler<PlayerIndexEventArgs>(quit_Selected);
            hiscores.Selected += new EventHandler<PlayerIndexEventArgs>(hiscores_Selected);
            credits.Selected += new EventHandler<PlayerIndexEventArgs>(credits_Selected);

            MenuEntries.Add(play);
            MenuEntries.Add(hiscores);
            MenuEntries.Add(credits);
            MenuEntries.Add(quit);

            MenuItemSelect = this.ScreenManager.Game.Content.Load<SoundEffect>(@"Sounds\SERVO1A");
            MenuItemSwitch = this.ScreenManager.Game.Content.Load<SoundEffect>(@"Sounds\CLICK10A");




            base.Activate(instancePreserved);
        }

        public override void HandleInput(GameTime gameTime, Engine.GameStateManagerment.InputState input)
        {
            PlayerIndex playerIndex;

            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex) || menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
                MenuItemSwitch.Play();

            base.HandleInput(gameTime, input);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.WhiteSmoke);

            float scale = (float)ScreenManager.GraphicsDevice.Viewport.Height / (float)_background.Height;
            var size = new Rectangle(0, 0, _background.Width, _background.Height);
            var pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            var origin = new Vector2(_background.Width / 2, _background.Height / 2);

            ScreenManager.SpriteBatch.Begin();

            /// <summary>
            /// Splash 
            /// </summary>
            ScreenManager.SpriteBatch.Draw(_background, pos, size, Color.White, 0, origin, scale, SpriteEffects.None, 0);

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnCancel(Microsoft.Xna.Framework.PlayerIndex playerIndex)
        {
            MenuItemSelect.Play();
            quit_Selected(this, null);
        }


        void credits_Selected(object sender, PlayerIndexEventArgs e)
        {
            MenuItemSelect.Play();
            ScreenManager.AddScreen(new Credits(), null);
        }

        void hiscores_Selected(object sender, PlayerIndexEventArgs e)
        {
            MenuItemSelect.Play();
            ScreenManager.AddScreen(new HighScoreMenu(), ControllingPlayer);
        }

        void quit_Selected(object sender, PlayerIndexEventArgs e)
        {
            MenuItemSelect.Play();
            ScreenManager.Game.Exit();
        }

        void play_Selected(object sender, PlayerIndexEventArgs e)
        {
            MenuItemSelect.Play();
            ScreenManager.AddScreen(new LevelMenu(0), ControllingPlayer);
        }
    }
}
