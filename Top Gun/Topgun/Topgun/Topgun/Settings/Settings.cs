using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Engine;
using Engine.Screens;

namespace Topgun
{      
    //[Serializable]
    public class Settings
    {
        private List<string> _settings = new List<string>();

        private bool fullScreen;
        private bool sound;
        private bool music;

        public bool Music
        {
            get { return music; }
            set { music = value; }
        }

        public bool Sound
        {
            get { return sound; }
            set { sound = value; }
        }

        public bool FullScreen
        {
            get { return fullScreen; }
            set { fullScreen = value; }
        }

        public Settings()
        {
            fullScreen = true;
            sound = true;
        }

        public/* static Settings*/ void Load()
        {
            _settings.AddRange(File.ReadAllLines(@"Settings\settings.txt"));
            fullScreen = _settings[0] == "FullScreen=True" ? true : false;
            sound = _settings[1] == "Sound=True" ? true : false;
            music = _settings[2] == "Music=True" ? true : false;

            /*
            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            return (Settings)serializer.Deserialize(stream);
            */
        }

        public void Save()
        {
            _settings[0] = fullScreen ? "FullScreen=True" : "FullScreen=False";
            _settings[1] = sound ? "Sound=True" : "Sound=False";
            _settings[2] = music ? "Music=True" : "Music=False";
            File.WriteAllLines(@"Settings\settings.txt", _settings);

            /*
            Stream stream = File.Create(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            serializer.Serialize(stream, this);
            stream.Close();
            */
        }
    }
}
