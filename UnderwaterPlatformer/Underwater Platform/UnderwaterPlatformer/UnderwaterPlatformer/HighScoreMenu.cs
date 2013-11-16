using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Screens;
using System.IO;


namespace UnderwaterPlatformer
{
    class HighScoreMenu : MenuScreen
    {
        private HighScore highScores;

        public HighScoreMenu() : base ("High Scores")
        {
             highScores = new HighScore();
             if (!highScores.HighScores.Count().Equals(0))
             {
                 List<int> scores = new List<int>();
                
                 for (int i = 0; i < highScores.HighScores.Count() && i < 15 ; i++)
                 {
                     int temp;
                     Int32.TryParse(highScores.HighScores[i], out temp);
                     scores.Add(temp);
                 }
                 scores.Sort();
                 highScores.HighScores.Clear();
                 for (int i = scores.Count() - 1; i >= 0; i-- )
                 {
                     highScores.HighScores.Add(scores[i].ToString());
                 }
                 
             }

             for (int i = 0; i < highScores.HighScores.Count(); i++)
             {
                 string scoreEntry = (i + 1) + ". " + highScores.HighScores[i];
                 MenuEntry highScore = new MenuEntry(scoreEntry.PadRight(10));
                 highScore.Selected += new EventHandler<PlayerIndexEventArgs>(score_selected);
                 MenuEntries.Add(highScore);
             }

             MenuEntry Back = new MenuEntry("Back");
             Back.Selected += new EventHandler<PlayerIndexEventArgs>(back_Selected);
             MenuEntries.Add(Back);

        }
        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }
        void score_selected(object sender, PlayerIndexEventArgs e)
        {
        }

    }
}
