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
#endregion

namespace Engine.Screens
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public class SplashScreen : GameScreen
    {

        SplashEntry menuEntry;

        InputAction buttonPressed;

        public SplashEntry MenuEntry
        {
            get { return menuEntry; }
            set { menuEntry = value; }
        }

        public SplashScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            buttonPressed = new InputAction(
                new Buttons[] { 
                                Buttons.A, Buttons.Start, Buttons.X, Buttons.Y, 
                                Buttons.RightShoulder, Buttons.RightStick, Buttons.LeftShoulder, 
                                Buttons.LeftStick, Buttons.Back, Buttons.DPadDown, Buttons.DPadLeft, 
                                Buttons.DPadRight, Buttons.DPadUp, Buttons.LeftTrigger, Buttons.RightTrigger 
                                },
                new Keys[] { 
                                Keys.Space, Keys.W, Keys.A, Keys.S, Keys.D, Keys.Up, Keys.Down, 
                                Keys.Left, Keys.Right, Keys.Escape, Keys.Enter
                                },
                true);
        }

        #region Handle Input



        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;
            
            if (buttonPressed.Evaluate(input, PlayerIndex.One, out playerIndex))
            {
                OnSelectEntry();
            }
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry()
        {
            menuEntry.OnSelectEntry(PlayerIndex.One);
        }

        #endregion

        #region Update and Draw

        protected virtual void UpdateSplashEntryLocations()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = new Vector2(0f, ScreenManager.GraphicsDevice.Viewport.Height / 6 * 5 - menuEntry.GetHeight(this) / 2 );

            SplashEntry SplashEntry = menuEntry;

            position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - SplashEntry.GetWidth(this) / 2;

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            SplashEntry.Position = position;
            
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                menuEntry.Update(this, gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateSplashEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

                SplashEntry SplashEntry = menuEntry;

                SplashEntry.Draw(this, gameTime);

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            spriteBatch.End();
        }

        #endregion
    }
}
