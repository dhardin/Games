using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topgun
{
    class Splash : SplashScreen
    {
        Texture2D _background;

        public override void Activate(bool instancePreserved)
        {
            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");

            var play = new SplashEntry("Press any button to continue...");

            play.Selected += new EventHandler<PlayerIndexEventArgs>(play_Selected);

            MenuEntry = play;

            base.Activate(instancePreserved);
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

        void play_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new MainMenu(), ControllingPlayer);
        }
    }
}
