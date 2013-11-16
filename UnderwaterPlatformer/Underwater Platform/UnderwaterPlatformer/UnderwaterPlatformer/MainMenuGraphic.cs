using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UnderwaterPlatformer
{
    class MainMenuGraphic : GameScreen
    {
        Texture2D Background;
        string assetName;

        public MainMenuGraphic(string assetname)
        {
            assetName = assetname;
        }

        public override void Activate(bool instancePreserved)
        {
            Background = ScreenManager.Game.Content.Load<Texture2D>("MenuGraphic");
            base.Activate(instancePreserved);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

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
