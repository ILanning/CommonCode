using System;
using System.Collections.Generic;
using System.IO;

namespace CommonCode
{
    //TODO: Fill out class
    public class GameSettings
    {
        /*Plan:
         * Text file opening, parsing, and saving
         * Defaults to settings.txt in the executable's folder, but can be set
         
         * Format: Key | Value(s)
         * Lines starting with a pound sign are comments
         */
        //public static bool RunInBackground = false;
        public static Dictionary<string, string> Settings;
        public static string Path = "Content\\settings.txt";

        public static void Initialize()
        {
            Settings = new Dictionary<string, string>();
            Settings["RunInBackground"] = "true";
            Settings["WindowInitWidth"] = "800";
            Settings["WindowInitHeight"] = "600";
            Settings["CreateLogs"] = "true";
        }
        public static void Initialize(Dictionary<string, string> settings) { Settings = settings; }
        public static void Initialize(string path) { Load(path); }

        public static void Load() { Load(Path); }
        public static void Load(string path)
        {
            StreamReader reader = new StreamReader(path);
            Settings = new Dictionary<string, string>();

            while(!reader.EndOfStream)
            {
                string[] pair = reader.ReadLine().Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if(pair.Length > 1)
                {
                    Settings[pair[0].Trim()] = pair[1].Trim();
                }
            }

            reader.Dispose();
        }

        public static void Save() { Save(Path); }
        public static void Save(string path)
        {
            throw new NotImplementedException();
        }
    }
}
