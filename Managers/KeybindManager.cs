using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;


namespace CommonCode
{
    /// <summary>
    /// An InputManager add-on that wraps a dictionary holding the keybindings for the game.
    /// </summary>
    public class KeybindManager
    {
        private Dictionary<string, Keys> keybinds;
        private static string rootDirectory;
        /* Shoehorned keys:
          * Keys.F20 = LeftMouse
          * Keys.F21 = MiddleMouse
          * Keys.F22 = RightMouse
          * Keys.F23 = XButton1
          * Keys.F24 = XButton2
          */

        /// <summary>
        /// Default keybindings.  Hardcoded.
        /// </summary>
        public KeybindManager(string rootDirectory)
        {
            keybinds = new Dictionary<string, Keys>(6);
            KeybindManager.rootDirectory = rootDirectory;

            /*Example*/
            //keybinds.Add("Camera Forward", Keys.Up);
        }

        public KeybindManager(string rootDirectory, Dictionary<string, Keys> bindings)
        {
            keybinds = bindings;
            KeybindManager.rootDirectory = rootDirectory;
        }

        /// <summary>
        /// Loads keybindings from file.
        /// </summary>
        /// <param name="filePath">Path from the root(.exe location) to the keybinds file.</param>
        public static KeybindManager Load(string filePath)
        {
            StreamReader streamReader = new StreamReader(Path.Combine(rootDirectory, filePath));
            Dictionary<string, Keys> newDict = new Dictionary<string, Keys>();

            int currentLine = 1;
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                string[] keyValuePair = line.Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries);
                if (keyValuePair.Length == 2)
                {
                    keyValuePair[0] = keyValuePair[0].Trim();
                    keyValuePair[1] = keyValuePair[1].Trim();
                    if(keyValuePair[0] == "")
                        throw new InvalidDataException("The key at line " + currentLine.ToString() + " is invalid.");
                    try
                    {
                        Keys value = (Keys)Enum.Parse(typeof(Keys), keyValuePair[1]);
                        newDict.Add(keyValuePair[0], value);
                    }
                    catch (ArgumentException e)
                    {
                        throw new InvalidDataException("The value at line " + currentLine.ToString() + " is invalid.", e);
                    }
                }
                else
                    throw new InvalidDataException("The Key|Value pair at line " + currentLine.ToString() + " is invalid.");
                currentLine++;
            }

            streamReader.Close();
            return new KeybindManager(rootDirectory, newDict);
        }

        /// <summary>
        /// Saves the current keybindings to the specified path.
        /// </summary>
        /// <param name="toBeSerialized">KeybindManager to save.</param>
        /// <param name="filePath">Path from the root(.exe location) to the keybinds file.</param>
        /// <returns>Absolute path to the new file.</returns>
        static public string Save(KeybindManager toBeSerialized, string filePath)
        {
            string newFilePath = Path.Combine(rootDirectory, filePath);
            StreamWriter streamWriter = new StreamWriter(File.Create(newFilePath));

            int longestKeyLength = 0;
            foreach (KeyValuePair<string, Keys> keyValuePair in toBeSerialized.keybinds)
                if (keyValuePair.Key.Length > longestKeyLength)
                    longestKeyLength = keyValuePair.Key.Length;
            longestKeyLength++;

            foreach (KeyValuePair<string, Keys> keyValuePair in toBeSerialized.keybinds)
            {
                string whitespace = new string(' ', longestKeyLength - keyValuePair.Key.Length);
                string generatedLine = keyValuePair.Key + whitespace + "| " + Enum.GetName(typeof(Keys), keyValuePair.Value);
                streamWriter.WriteLine(generatedLine);
            }

            streamWriter.Flush();
            streamWriter.Close();
            return newFilePath;
        }

        public Keys this[string key]
        {
            get { return keybinds[key]; }
            private set { keybinds[key] = value; }
        }

        /// <summary>
        /// Creates or overwrites a keybinding.
        /// </summary>
        /// <param name="key">Name of the action to bind.</param>
        /// <param name="button">Button to bind it to.</param>
        public void Bind(string command, Keys button)
        {
            if (keybinds.ContainsKey(command))
                keybinds.Remove(command);
            keybinds.Add(command, button);
        }
    }
}
