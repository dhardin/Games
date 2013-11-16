#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Engine.Screens
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public class ImageMenuScreen : GameScreen
    {
        #region Fields

        List<ImageMenuEntry> menuEntries = new List<ImageMenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        public InputAction menuUp;
        public InputAction menuDown;
        public InputAction menuLeft;
        public InputAction menuRight;
        InputAction menuSelect;
        InputAction menuCancel;




        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        public IList<ImageMenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageMenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            menuUp = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up },
                true);
            menuDown = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down },
                true);
            menuLeft = new InputAction(
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
                new Keys[] { Keys.Left },
                true);
            menuRight = new InputAction(
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
                new Keys[] { Keys.Right },
                true);

            menuSelect = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Enter, Keys.Space },
                true);
            menuCancel = new InputAction(
                new Buttons[] { Buttons.B, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            //ScreenManager s = new ScreenManager();



        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            // Select:
            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // Special cases:
                if (selectedEntry == 5)
                    selectedEntry = 5;
                else if (selectedEntry == 0) //back
                    selectedEntry = 0;
                else if (selectedEntry == 26)//next
                    selectedEntry = 26;
                // Default:
                else
                    selectedEntry -= 5;

                if (selectedEntry < 0)
                    selectedEntry += 5;
            }
            if (menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // Special cases:
                if (selectedEntry == 21)
                    selectedEntry = 21;
                else if (selectedEntry == 0) //back
                    selectedEntry = 0;
                else if (selectedEntry == 26)//next
                    selectedEntry = 26;
                // Default:
                else
                    selectedEntry += 5;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry -= 5;
            }
            if (menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry--;
                //MenuItemSwitch.Play();
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            if (menuRight.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;
                //MenuItemSwitch.Play();
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Execute:
            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                //MenuItemSelect.Play();
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // MenuItemSelect.Play();
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //Vector2 position = new Vector2(0f, 120f);
            Vector2 defaultPos = new Vector2(0f, ScreenManager.GraphicsDevice.Viewport.Height / 6);


            Vector2 position = defaultPos;

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                ImageMenuEntry menuEntry = menuEntries[i];

                // start new row when needed
                if (((i - 1) % 5) == 0 && i > 1) position.Y += ScreenManager.GraphicsDevice.Viewport.Height / 6;

                if (i == 0) //back
                {
                    position.X = 25 + menuEntry.GetWidth(this) / 2;
                    position.Y = ScreenManager.GraphicsDevice.Viewport.Height - 25;
                }
                else if (i == 26) //next
                {
                    position.X = ScreenManager.GraphicsDevice.Viewport.Width - 25 - menuEntry.GetWidth(this) / 2;
                    position.Y = ScreenManager.GraphicsDevice.Viewport.Height - 25;
                }
                else // Form a grid 
                    position.X = (ScreenManager.GraphicsDevice.Viewport.Width / 6) * (((i - 1) % 5) + 1);

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

                if (i == 0) //reset position after back
                    position.Y = defaultPos.Y;
            }
        }


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                if (MenuEntries[i] != null)
                {
                    bool isSelected = IsActive && (i == selectedEntry);

                    menuEntries[i].Update(this, isSelected, gameTime);
                }
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                ImageMenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 35);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(255, 255, 255) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, new Vector2(titlePosition.X + 2, titlePosition.Y + 2), new Color(0.0f, 0.0f, 0.0f, 0.85f), 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}
