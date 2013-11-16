using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnderwaterPlatformer
{
    class HighScore
    {
        #region Fields
        private string[] path;
        //Directory.GetFiles(@"Levels", "*.txt");
        private List<string> highScores = new List<string>();
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
            
            path = Directory.GetFiles(@"High Scores", "High Scores.dat");
            if (path.Length.Equals(0))
            {
                path = new string[1];
                path[0] = (@"High Scores\High Scores.dat");
            }

            if (!File.Exists(path[0]))
            {
                // Create a file to write to.
                throw new ArgumentException("High Scores.dat does not exist");

            }
            else
            {

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
         

        }

        public void AddScore(string score)
        {
            using (StreamWriter sw = File.AppendText(path[0]))
            {
                sw.WriteLine(score);
                highScores.Add(score);
            }
        }

        public void SortScores()
        {
        }
    }
}
