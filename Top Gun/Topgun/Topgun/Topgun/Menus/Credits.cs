using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Topgun
{
    class Credits : MenuScreen
    {
        private Texture2D _background;

        SoundEffect MenuItemSelect;
        SoundEffect MenuItemSwitch;

        public Credits()
            : base("Credits")
        {

        }

        public override void Activate(bool instancePreserved)
        {
            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");

            var c1 = new MenuEntry("Jason Bainbridge");
            var c2 = new MenuEntry("Nick Sexton");
            var c3 = new MenuEntry("Dan Rosendale");
            var c4 = new MenuEntry("Dustin Hardin");

            var quit = new MenuEntry("Back");

            quit.Selected += new EventHandler<PlayerIndexEventArgs>(quit_Selected);

            MenuEntries.Add(c1);
            MenuEntries.Add(c2);
            MenuEntries.Add(c3);
            MenuEntries.Add(c4);
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
            var bgPos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            var origin = new Vector2(_background.Width / 2, _background.Height / 2);
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(_background, bgPos, size, Color.White, 0, origin, scale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        void quit_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }
    }
}
