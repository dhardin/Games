using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Engine.GameStateManagerment;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Map;
using Microsoft.Xna.Framework.Audio;

namespace Topgun
{
    class LevelMenu : ImageMenuScreen
    {
        #region Fields
        Texture2D[,] levelTexture = new Texture2D[20, 20];
        Texture2D _background;
        SoundEffect MenuItemSelect;
        SoundEffect MenuItemSwitch;

        List<string> levelTypes = new List<string>();


        #endregion

        #region Properties
        private int screenIndex { get; set; }
        #endregion

        /// <summary>
        /// Level Menu
        /// </summary>
        /// <param name="ScreenIndex">Level Select Screen number</param>
        public LevelMenu(int ScreenIndex)
            : base("Level Select")
        {
            screenIndex = ScreenIndex;
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            _background = ScreenManager.Game.Content.Load<Texture2D>("Splash");

            // Load level Texture

            // Get levels
            //string[] levelFiles = Directory.GetFiles(@"Levels", "*.txt");
            levelTypes.Add("Temperate");
            levelTypes.Add("Space");
            levelTypes.Add("Tropical");
            levelTypes.Add("Sky");
            levelTypes.Add("Volcanic");
            levelTypes.Add("Arctic");

            int numLevels = levelTypes.Count;

            ///<summary>
            /// Back 
            ///</summary>
            ImageMenuEntry back = new ImageMenuEntry("Back", null);
            back.Selected += new EventHandler<PlayerIndexEventArgs>(back_Selected);
            MenuEntries.Add(back);

            ///<summary>
            /// Levels 
            ///</summary>
            int firstItem = screenIndex * 25;
            int lastItem = firstItem + 25;
            for (int i = firstItem; i < lastItem; i++)
            {
                if (i < numLevels)
                {
                    string levelFile = levelTypes[i];
                    var gameLevel = new Level();

                    var menuEntry = new ImageMenuEntry(levelFile, null);//new ImageMenuEntry(Path.GetFileNameWithoutExtension(levelFile), null); //levelTexture);
                    menuEntry.Selected += (s, e) => play_Selected(levelFile);

                    MenuEntries.Add(menuEntry);
                }
                else break;
            }

            ///<summary>
            /// Next
            ///</summary>
            if ((screenIndex + 1) < Math.Ceiling((float)numLevels / 25))
            {
                ImageMenuEntry next = new ImageMenuEntry("Next", null);
                next.Selected += new EventHandler<PlayerIndexEventArgs>(next_Selected);
                MenuEntries.Add(next);
            }

            MenuItemSelect = this.ScreenManager.Game.Content.Load<SoundEffect>(@"Sounds\SERVO1A");
            MenuItemSwitch = this.ScreenManager.Game.Content.Load<SoundEffect>(@"Sounds\CLICK10A");


        }

        public override void HandleInput(GameTime gameTime, Engine.GameStateManagerment.InputState input)
        {
            PlayerIndex playerIndex;

            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex) ||
                menuDown.Evaluate(input, ControllingPlayer, out playerIndex) ||
                menuRight.Evaluate(input, ControllingPlayer, out playerIndex) ||
                menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                MenuItemSwitch.Play();
            }

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

        /// <summary>
        ///  Override cancel (B & ESC) to go back 1 screen
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            MenuItemSelect.Play();
            back_Selected(this, null);
        }

        #region events

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            MenuItemSelect.Play();
            ExitScreen();
            if (screenIndex > 0)
                ScreenManager.AddScreen(new LevelMenu(--screenIndex), null);
        }

        void play_Selected(string levelFile)
        {
            MenuItemSelect.Play();
            ScreenManager.AddScreen(new GameplayScreen(levelFile), ControllingPlayer);
        }

        void next_Selected(object sender, PlayerIndexEventArgs e)
        {
            MenuItemSelect.Play();
            ExitScreen();
            ScreenManager.AddScreen(new LevelMenu(++screenIndex), null);
        }


        #endregion

    }
}
