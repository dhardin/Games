#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.GameStateManagerment;
using Engine.Map;
using System.Collections.Generic;
#endregion

namespace Engine.Screens
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public class ImageMenuEntry
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        /// <summary>
        /// The texture for this entry (mini level).
        /// </summary>
        Texture2D[,] levelTexture = new Texture2D[20,20];

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text and texture.
        /// </summary>
        public ImageMenuEntry(string text, Texture2D[,] texture)
        {
            this.text = text;

            if (texture != null)
            {
                for (int i = 0; i < 20; i++)
                    for (int j = 0; j < 20; j++)
                        levelTexture[i, j] = texture[i, j];
            }
            else
                levelTexture = null;
        }


        #endregion

        #region Draw & Update

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(ImageMenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(ImageMenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;
            // Fade in when selected
            Color bgColor = isSelected ? Color.White : Color.White;
            // Grow slightly
            float scale = 1 + 1 * 0.05f;
            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(font.MeasureString(text).X / 2, font.LineSpacing / 2);

            // Mini level
            if (levelTexture != null)
            {
                Vector2 start = new Vector2(position.X - 48, position.Y - 48);

                for (int y = 0; y < 20; y++)
                {
                    for (int x = 0; x < 20; x++)
                    {
                        Texture2D texture = levelTexture[y,x];
                        Vector2 pos = new Vector2(start.X + x * (texture.Width*0.2f), start.Y + y * (texture.Width*0.2f));
                        spriteBatch.Draw(texture, pos, new Rectangle(0,0,texture.Width, texture.Height), 
                            bgColor, 0, new Vector2(0, 0), scale * 0.2f, SpriteEffects.None, 0);
                    }
                }
                // Border <top, bottom, left, right>
                spriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)start.X,    (int)start.Y,   97, 3), color);
                spriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)start.X,    (int)start.Y+97,97, 3), color);
                spriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)start.X,    (int)start.Y,   2, 99), color);
                spriteBatch.Draw(screenManager.BlankTexture, new Rectangle((int)start.X+95, (int)start.Y,   2, 99), color);
            }
            // Text
            spriteBatch.DrawString(font, text, new Vector2(position.X+2, position.Y+2), new Color(0.0f,0.0f,0.0f,0.85f), 0, origin, scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(ImageMenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(ImageMenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        #endregion
    }
}
