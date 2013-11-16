using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Engine.GameStateManagerment;
using Engine.Game;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Engine.Audio;

namespace Topgun
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : GameBase
    {
        Settings _settings = new Settings();

        public Game1()
        {
            Content.RootDirectory = "Content";
            
            /// <summary>
            /// Load settings
            /// </summary>
            _settings.Load();

            FullScreen = _settings.FullScreen;

            ScreenWidth = FullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : 800;
            ScreenHeight = FullScreen ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ScreenManager.FontName = "MenuFont";

            base.Initialize();
        }

        protected override void AddInitialScreens()
        {
            ScreenManager.AddScreen(new Splash(), null);
        }

    }
}
