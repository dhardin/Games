using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;

namespace UnderwaterPlatformer
{
    class MainMenu : MenuScreen
    {
        public MainMenu()
            : base("Underwater Platformer")
        {
            MenuEntry play = new MenuEntry("Play in the water!");
            play.Selected += new EventHandler<PlayerIndexEventArgs>(play_Selected);

            MenuEntry quit = new MenuEntry("Don't want to get pruny...");
            quit.Selected += new EventHandler<PlayerIndexEventArgs>(quit_Selected);

            MenuEntry HighS = new MenuEntry("Highscores");
            HighS.Selected += new EventHandler<PlayerIndexEventArgs>(high_Selected);

            MenuEntries.Add(play);
            MenuEntries.Add(HighS);
            MenuEntries.Add(quit);
        }

        void high_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new HighScoreMenu(), ControllingPlayer);
            
        }
        void quit_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void play_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new LevelMenu(0), ControllingPlayer);
        }
    }
}
