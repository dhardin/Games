using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UnderwaterPlatformer
{
    class SplashMenu : GameScreen
    {
        Texture2D Background;
        string assetName;

        public SplashMenu(string assetname)
        {
            assetName = assetname;
        }

        public override void Activate(bool instancePreserved)
        {
            Background = ScreenManager.Game.Content.Load<Texture2D>("Splash");
            base.Activate(instancePreserved);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            {
                ScreenManager.RemoveScreen(this);
                ScreenManager.AddScreen(new MainMenuGraphic("MenuGraphic"), ControllingPlayer);
                ScreenManager.AddScreen(new MainMenu(), ControllingPlayer);
            }

        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            DrawContext ctx = new DrawContext();
            ctx.Batch = ScreenManager.SpriteBatch;
            ctx.Blank = ScreenManager.BlankTexture;
            ctx.Device = ScreenManager.GraphicsDevice;
            ctx.Time = gameTime;

            ctx.Batch.Begin();
            ctx.Batch.Draw(Background, Vector2.Zero, Color.White);
            ctx.Batch.End();          

            base.Draw(gameTime);
        }
    }
}
