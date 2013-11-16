using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Topgun
{
    class HighScore
    {
        #region Fields
        private string[] path;
        //Directory.GetFiles(@"Levels", "*.txt");
        private List<string> highScores = new List<string>();
        int indexP1, indexP2, lineP1, lineP2;
        #endregion

        #region Properties
        public string[] Path
        {
            get { return path; }
        }
        public List<string> HighScores
        {
            get { return highScores; }
        }
        #endregion

        public HighScore()
        {

            if (Directory.Exists(@"High Scores\High Scores.dat"))
                path = Directory.GetFiles(@"High Scores", "High Scores.dat");
            else
            {
                Directory.CreateDirectory(@"High Scores");
                path = Directory.GetFiles(@"High Scores", "High Scores.dat");
            }
           
            if (path.Length.Equals(0))
            {
                path = new string[1];
                path[0] = (@"High Scores\High Scores.dat");
            }

            if (!File.Exists(path[0]))
            {
                    // Open the file to read from.
                using (StreamWriter sw = File.CreateText(path[0]))
                {

                    sw.WriteLine("Player 1: 0");
                    sw.WriteLine("Player 2: 0");

                }
            }
         
            // Open the file to read from.
            using (StreamReader sr = File.OpenText(path[0]))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    highScores.Add(s);
                }
            }
            
         

        }

        public void Erase()
        {
            using (FileStream stream = new FileStream(path[0], FileMode.Create))
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.Write("");
            }
        }
        public void AddScore(string score, Player player)
        {
            using (StreamWriter sw = File.AppendText(path[0]))
            {
                if (player.playerIndex.Equals(PlayerIndex.One))
                {
                    sw.WriteLine("Player 1: " + score);
                    highScores.Add(score);
                }
                else
                {
                    sw.WriteLine("Player 2: " + score);
                    highScores.Add(score);
                }
            }
        }
        public void AddScore(string score, PlayerIndex playerIndex)
        {
            using (StreamWriter sw = File.AppendText(path[0]))
            {
                if (playerIndex.Equals(PlayerIndex.One))
                {
                    sw.WriteLine("Player 1: " + score);
                    highScores.Add(score);
                }
                else
                {
                    sw.WriteLine("Player 2: " + score);
                    highScores.Add(score);
                }
            }
        }

        public void SortScores()
        {
        }
    }
}



  