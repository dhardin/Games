using Engine.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PolarRecall
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : GameBase
    {
        public Game1()
        {
            Content.RootDirectory = "Content";
            ScreenWidth = 800;
            ScreenHeight = 600;
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void AddInitialScreens()
        {
            ScreenManager.AddScreen(new LevelMenu(0), null);
        }
    }
}
