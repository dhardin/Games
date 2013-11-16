using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;

namespace PolarRecall
{
    class MainMenu : MenuScreen
    {
        public MainMenu()
            : base("Main Menu for Polar Recall (It's all about Shayne)")
        {
            MenuEntry play = new MenuEntry("Play with Shayne");
            play.Selected += new EventHandler<PlayerIndexEventArgs>(play_Selected);

            MenuEntry quit = new MenuEntry("Leave shayne alone");
            quit.Selected += new EventHandler<PlayerIndexEventArgs>(quit_Selected);

            MenuEntries.Add(play);
            MenuEntries.Add(quit);
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
