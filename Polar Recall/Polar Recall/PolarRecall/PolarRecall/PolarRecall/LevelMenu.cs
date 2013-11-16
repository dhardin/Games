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

namespace PolarRecall
{
    class LevelMenu : ImageMenuScreen
    {
        #region Fields
        List<ImageMenuEntry> level = new List<ImageMenuEntry>(); // List of level menu entries
        Dictionary<char, Texture2D> _levelTextures = new Dictionary<char, Texture2D>();
        Texture2D[,] levelTexture = new Texture2D[20, 20];
        #endregion

        #region Properties
        private int numLevels { get; set; }
        private int screenIndex { get; set; }
        #endregion

        #region LevelMenu

        /// <summary>
        /// Level Menu
        /// </summary>
        /// <param name="ScreenIndex">Level Select Screen number</param>
        /// <param name="content">Screenmanager.Game.Content</param>
        public LevelMenu(int ScreenIndex) : base("Level Select")
        {
            screenIndex = ScreenIndex;

        }

        #endregion

        #region Overrides

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            _levelTextures[' '] = ScreenManager.Game.Content.Load<Texture2D>("Blank");
            _levelTextures['.'] = ScreenManager.Game.Content.Load<Texture2D>("Floor");
            _levelTextures['#'] = ScreenManager.Game.Content.Load<Texture2D>("IceBlock");
            _levelTextures['r'] = ScreenManager.Game.Content.Load<Texture2D>(@"Presents\Red");
            _levelTextures['g'] = ScreenManager.Game.Content.Load<Texture2D>(@"Presents\Green");
            _levelTextures['b'] = ScreenManager.Game.Content.Load<Texture2D>(@"Presents\Blue");
            _levelTextures['y'] = ScreenManager.Game.Content.Load<Texture2D>(@"Presents\Yellow");
            _levelTextures['R'] = ScreenManager.Game.Content.Load<Texture2D>(@"Buckets\BKR");
            _levelTextures['G'] = ScreenManager.Game.Content.Load<Texture2D>(@"Buckets\BKG");
            _levelTextures['B'] = ScreenManager.Game.Content.Load<Texture2D>(@"Buckets\BKB");
            _levelTextures['Y'] = ScreenManager.Game.Content.Load<Texture2D>(@"Buckets\BKY");

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
                if(i < numLevels)
                {
                    string levelFile = levelFiles[i];
                    var gameLevel = new Level(levelFile);

                    for (int y = 0; y < 20; y++)
                    {
                        for (int x = 0; x < 20; x++)
                        {
                            char tile = gameLevel.TileAt(x, y);
                            levelTexture[y, x] = _levelTextures[tile];
                        }
                    }

                    var menuEntry = new ImageMenuEntry(Path.GetFileNameWithoutExtension(levelFile), levelTexture);
                    menuEntry.Selected += (s, e) => play_Selected(levelFile);
                    level.Add(menuEntry);

                    MenuEntries.Add(menuEntry);
                }
                else
                {
                    MenuEntries.Add(new ImageMenuEntry("", null));
                }
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
            ScreenManager.AddScreen(new GameplayScreen(levelFile), ControllingPlayer);
        }

        void next_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new LevelMenu(++screenIndex), null);         
        }
        #endregion

    }
}
