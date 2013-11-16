using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.GameStateManagerment;

namespace Engine.Screens
{

    public class SplashEntry
    {

        string text;
        Vector2 position;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public event EventHandler<PlayerIndexEventArgs> Selected;

        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        public SplashEntry(string text)
        {
            this.text = text;
        }

        #region Update and Draw

        public virtual void Update(SplashScreen screen, GameTime gameTime){}

        public virtual void Draw(SplashScreen screen, GameTime gameTime)
        {
            Color color = Color.White;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;        
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f;

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);


            spriteBatch.DrawString(font, text, new Vector2(position.X+2, position.Y+2), new Color(0.0f,0.0f,0.0f,0.85f), 0, origin, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(SplashScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        public virtual int GetWidth(SplashScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        #endregion
    }
}
