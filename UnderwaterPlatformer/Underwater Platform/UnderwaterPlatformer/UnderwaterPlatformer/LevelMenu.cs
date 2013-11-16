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

namespace UnderwaterPlatformer
{
    class LevelMenu : ImageMenuScreen
    {
        #region Fields
        List<ImageMenuEntry> level = new List<ImageMenuEntry>(); // List of level menu entries
        Dictionary<char, Texture2D> _levelTextures = new Dictionary<char, Texture2D>();
        Texture2D[,] levelTexture = new Texture2D[20, 20];
        #endregion

        #region Properties
        public int numLevels { get; set; }
        private int screenIndex { get; set; }
        #endregion

        #region LevelMenu

        /// <summary>
        /// Level Menu
        /// </summary>
        /// <param name="ScreenIndex">Level Select Screen number</param>
        /// <param name="content">Screenmanager.Game.Content</param>
        public LevelMenu(int ScreenIndex)
            : base("Level Select")
        {
            screenIndex = ScreenIndex;

        }

        #endregion

        #region Overrides

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            _levelTextures['-'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_Up");
            _levelTextures['_'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_Down");
            _levelTextures[']'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_Right");
            _levelTextures['['] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_Left");
            _levelTextures['.'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground");
            _levelTextures['/'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_UpLeft");
            _levelTextures['\\'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_UpRight");
            _levelTextures['L'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_DownLeft");
            _levelTextures['J'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Ground_DownRight");
            _levelTextures['~'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Water");
            _levelTextures['d'] = ScreenManager.Game.Content.Load<Texture2D>(@"Diamonds\D");
            _levelTextures['f'] = ScreenManager.Game.Content.Load<Texture2D>(@"Fish\f_");
            _levelTextures['F'] = ScreenManager.Game.Content.Load<Texture2D>(@"Fish\f_");
            _levelTextures['M'] = ScreenManager.Game.Content.Load<Texture2D>(@"Fish\f_");
            _levelTextures['t'] = ScreenManager.Game.Content.Load<Texture2D>(@"Water Decor\t_");
            _levelTextures['T'] = ScreenManager.Game.Content.Load<Texture2D>(@"Water Decor\T");
            _levelTextures['x'] = ScreenManager.Game.Content.Load<Texture2D>(@"Level\Exit");
            _levelTextures['S'] = ScreenManager.Game.Content.Load<Texture2D>(@"Fish\Shark");
            _levelTextures['P'] = ScreenManager.Game.Content.Load<Texture2D>(@"Tiles\Water");

            // Get number of levels
            string[] levelFiles = Directory.GetFiles(@"Levels", "*.txt");
            numLevels = levelFiles.Length;

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
            int i;
            for (i = firstItem; i < lastItem; i++)
            {
                if (i < numLevels)
                {
                    string levelFile = levelFiles[i];
                    var gameLevel = new Level(levelFile);

                    for (int y = gameLevel.Height - 20; y < gameLevel.Height; y++)
                    {
                        for (int x = gameLevel.Width - 20; x < gameLevel.Width; x++)
                        {
                            char tile = gameLevel.TileAt(x, y);
                            levelTexture[gameLevel.Height - y - 1, gameLevel.Width - x - 1] = _levelTextures[tile];
                        }
                    }

                    var menuEntry = new ImageMenuEntry(Path.GetFileNameWithoutExtension(levelFile), levelTexture);
                    menuEntry.Selected += (s, e) => play_Selected(levelFile);
                    level.Add(menuEntry);

                    MenuEntries.Add(menuEntry);
                }
                //else
                //{
                //   // MenuEntries.Add(new ImageMenuEntry("", null));
                //}
            }

            ///<summary>
            /// Next
            ///</summary>
            if (screenIndex < Math.Ceiling((float)numLevels / 25))
            {
                ImageMenuEntry next = new ImageMenuEntry("Next", null);
                next.Selected += new EventHandler<PlayerIndexEventArgs>(next_Selected);
                MenuEntries.Add(next);
            }
        }

        /// <summary>
        ///  Override cancel (B & ESC) to go back 1 screen
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            back_Selected(this, null);
        }

        #endregion

        #region events

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.RemoveScreen(this);
            if (screenIndex <= 0)
                ScreenManager.AddScreen(new MainMenu(), null);
            else
                ScreenManager.AddScreen(new LevelMenu(--screenIndex), null);
        }

        void play_Selected(string levelFile)
        {
            ScreenManager.AddScreen(new GameplayScreen(levelFile, "0"), ControllingPlayer);
        }

        void next_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new LevelMenu(++screenIndex), null);
        }
        #endregion

    }
}
