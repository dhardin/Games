#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Engine.Audio;
using Microsoft.Xna.Framework;
using Engine.GameStateManagerment;
using Engine.TaskManagement;

namespace Engine.Game
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public abstract class GameBase : Microsoft.Xna.Framework.Game
    {
        readonly GraphicsDeviceManager _graphics;
        readonly ScreenManager _screenManager;
        IScreenFactory _screenFactory;
        readonly TaskManagerComponent _taskManagerComponent;

        #region Properties
        public int ScreenWidth
        {
            get { return _graphics.PreferredBackBufferWidth; }
            set { _graphics.PreferredBackBufferWidth = value; }
        }

        public int ScreenHeight
        {
            get { return _graphics.PreferredBackBufferHeight; }
            set { _graphics.PreferredBackBufferHeight = value; }
        }

        public IScreenFactory ScreenFactory
        {
            get { return _screenFactory; }
            set
            {
                _screenFactory = value;
                Services.RemoveService(typeof(IScreenFactory));
                Services.AddService(typeof(IScreenFactory), _screenFactory);
            }
        }

        public ScreenManager ScreenManager
        {
            get { return _screenManager; }
        }

        public TaskManager TaskManager
        {
            get { return TaskManagerComponent.Instance; }
        }
        #endregion

        /// <summary>
        /// The main game constructor.
        /// </summary>
        protected GameBase()
        {
            Content.RootDirectory = "Content";

            _graphics = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromTicks(333333);

#if WINDOWS_PHONE
            graphics.IsFullScreen = true;

            // Choose whether you want a landscape or portait game by using one of the two helper functions.
            InitializeLandscapeGraphics();
            // InitializePortraitGraphics();
#endif

            // Create the screen factory and add it to the Services
            _screenFactory = new DefaultScreenFactory();
            Services.AddService(typeof(IScreenFactory), _screenFactory);

            // Create the screen manager component.
            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);

            // Create the process manager component
            _taskManagerComponent = new TaskManagerComponent(this);
            Components.Add(_taskManagerComponent);

            // Create audio engine component
            Components.Add(new AudioEngine(this));

#if WINDOWS
            IsMouseVisible = true;    
#endif

#if WINDOWS_PHONE
            // Hook events on the PhoneApplicationService so we're notified of the application's life cycle
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Launching += 
                new EventHandler<Microsoft.Phone.Shell.LaunchingEventArgs>(GameLaunching);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Activated += 
                new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(GameActivated);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated += 
                new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(GameDeactivated);
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();

            // On Windows and Xbox we just add the initial screens
            AddInitialScreens();
        }

        protected abstract void AddInitialScreens();

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Helper method to the initialize the game to be a portrait game.
        /// </summary>
        private void InitializePortraitGraphics()
        {
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Helper method to initialize the game to be a landscape game.
        /// </summary>
        private void InitializeLandscapeGraphics()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
        }

        void GameLaunching(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            AddInitialScreens();
        }

        void GameActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            // Try to deserialize the screen manager
            if (!screenManager.Activate(e.IsApplicationInstancePreserved))
            {
                // If the screen manager fails to deserialize, add the initial screens
                AddInitialScreens();
            }
        }

        void GameDeactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            // Serialize the screen manager when the game deactivated
            screenManager.Deactivate();
        }
#endif
    }
}
