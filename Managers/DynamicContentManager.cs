using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CommonCode
{
    /// <summary>
    /// A variable level manager that tracks all content loaded through it.  Can be used on a global, per-screen, or per-level basis.
    /// </summary>
    public class DynamicContentManager
    {
        //TODO:Clean this class up for new environment.  Make it a bit more extensible, so new loadable data types can be added with ease.

        /*Things this class needs:
         Load function
         * Needs to look in dictionary for file first, then add it in if it isn't there
         Save function?
         File Dictionaries (One for each file type?)
         public string BaseDirectory{get;}
         bound to the map class usually*/

        /// <summary>
        /// The dictionary containing everything this manager has loaded.
        /// </summary>
        Dictionary<string,object> FileDictionary;
        Dictionary<string, string> aliasDictionary;
        /// <summary>
        /// Game reference used in texture/font/model loading.
        /// </summary>
        public static Game Game;
        /// <summary>
        /// The directory from which relative paths given to the manager will start.
        /// </summary>
        public string BaseDirectory { get { return baseDirectory; } }
        string baseDirectory;
        /// <summary>
        /// Used to load files that must go through the XNA Content Pipeline (Models, SpriteFonts, etc.).
        /// </summary>
        ContentManager XNALoader;
        List<string> preloadFiles = new List<string>();

        /// <summary>
        /// Static dictionary used for custom file loading behaviors.
        /// </summary>
        static Dictionary<Type, CustomLoader> customLoaders = new Dictionary<Type, CustomLoader>();
        /// <summary>
        /// Delegate defining the signature for the loading function.  
        /// Note that in practice the returned object should always be cast to the function's key type in the dictionary.
        /// </summary>
        /// <param name="path">The path to the file to load</param>
        /// <param name="content">The content manager to load with</param>
        /// <returns>The loaded object</returns>
        public delegate object CustomLoader(string path, DynamicContentManager content);

        public static void RegisterCustomLoader(Type loadedType, CustomLoader function)
        {
            customLoaders[loadedType] = function;
        }

        public bool Initialized { get; private set; }

        public DynamicContentManager(Game game, string contentDirectory)
        {
            Game = game;
            XNALoader = new ContentManager(game.Services, contentDirectory);
            baseDirectory = contentDirectory;
            FileDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            aliasDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Initialized = true;
        }

        public DynamicContentManager(Game game, ContentManager content, string contentDirectory)
        {
            Game = game;
            XNALoader = content;
            baseDirectory = contentDirectory;
            FileDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            aliasDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Initialized = true;
        }

        /// <summary>
        /// Starts a new thread to load the given set of paths into the Content Manager for use later on.
        /// </summary>
        /*public async void Preload(string filePath, Type t)
        {
            preloadFiles.Add(filePath);
            await new Task();
            preloadFiles.Remove(filePath);
        }*/

        string pathFormat(string path)
        {
            path = path.Replace('/', '\\');
            return path;
        }

        string pathCombine(string path1, string path2)
        {
            string result;
            path1 = pathFormat(path1);
            path2 = pathFormat(path2);
            int index = path2.IndexOf(path1);
            if (index == 0 || (index == 2 && path2[0] == '.' && path2[1] == '\\')) //Path was already coming from the content directory, return it (mostly) as-is
                return path2;
            if (path2[0] == '.' && path2[1] == '\\')
                path2 = path2.Substring(2);
            if (path1[path1.Length-1] == '\\')
            {
                if(path2[0] == '\\')
                    path2 = path2.Substring(1);
            }
            else
            {
                if (path2[0] != '\\')
                    path2 = "\\" + path2;
            }
            result = path1 + path2;
            return result;
        }
        
        private void addAsset<T>(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            if(customLoaders.ContainsKey(typeof(T)))
            {
                //If we've got a custom loader for this type, invoke it to load this object
                FileDictionary.Add(filePath, customLoaders[typeof(T)].Invoke(pathCombine(baseDirectory, filePath), this));
            }
            else if(extension == ".xnb" || extension == ".spritefont" || extension == ".fbx" || extension == ".x" || extension == ".obj")
            {
                FileDictionary.Add(filePath, XNALoader.Load<T>(filePath.Remove(filePath.Length - (extension.Length))));
            }
            else if(extension == ".png" || extension == ".jpg" || 
                    extension == ".bmp" || extension == ".hdr" ||
                    extension == ".dds" || extension == ".pfm" ||
                    extension == ".dib" || extension == ".ppm" ||
                    extension == ".tga")
            {
                string combined = filePath;
                if (!combined.Contains(":\\")) //If the path isn't rooted, assume it's starting from the execution location
                    combined = pathCombine(baseDirectory, filePath);
                FileStream stream = new FileStream(combined, FileMode.Open);
                Texture2D image = Texture2D.FromStream(Game.GraphicsDevice, stream);
                stream.Dispose();
                FileDictionary.Add(filePath, image);
            }
            else
            {
                throw new ArgumentException("The extension " + extension + " or type " + typeof(T).Name + " is not loadable.");
            }
        }
        /// <summary>
        /// Loads a file from storage, or returns previously loaded files.
        /// </summary>
        /// <typeparam name="T">The type of the object being loaded.</typeparam>
        /// <param name="name">The path or alias of the file to load.</param>
        /// <returns></returns>
        public T Load<T>(string name)
        {
            name = pathFormat(name);
            if (aliasDictionary.ContainsKey(name))
                name = aliasDictionary["filePath"];

            if (!FileDictionary.ContainsKey(name))
                addAsset<T>(name);

            return (T)FileDictionary[name];
        }

        /// <summary>
        /// Disposes of the given content.  Do not call this on content that is still in use.
        /// </summary>
        /// <param name="name">The path or alias of the file to unload.</param>
        public void Unload(string name)
        {
            if (aliasDictionary.ContainsKey(name))
                name = aliasDictionary[name];
            if (FileDictionary.ContainsKey(name))
            {
                if (FileDictionary[name] is GraphicsResource)
                    ((GraphicsResource)FileDictionary[name]).Dispose();
                FileDictionary.Remove(name);
                if (aliasDictionary.ContainsValue(name))
                {
                    LinkedList<string> invalidEntries = new LinkedList<string>();
                    foreach ( KeyValuePair<string, string> pair in aliasDictionary)
                    {
                        if (pair.Value == name)
                            invalidEntries.AddLast(pair.Key);
                    }
                    while(invalidEntries.First != null)
                    {
                        aliasDictionary.Remove(invalidEntries.First.Value);
                        invalidEntries.RemoveFirst();
                    }
                }
            }
        }

        /// <summary>
        /// Disposes of the given content.  Do not call this on content that is still in use.
        /// </summary>
        public void Unload(string[] names)
        {
            foreach (string path in names)
                Unload(path);
        }
        /// <summary>
        /// Adds an alias to a filepath that may also be used to find the file.
        /// </summary>
        /// <param name="path">Path to add an alias to.</param>
        /// <param name="alias">Alternate name for your path.</param>
        /// <returns>Returns true if that path exists in the dictionary, otherwise false.</returns>
        public bool Alias(string path, string alias)
        {
            if(FileDictionary.ContainsKey(path))
            {
                aliasDictionary.Add(alias, path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Releases all used memory and destroys the object.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, object> texture in FileDictionary)
            { 
                if(texture.Value is Texture2D)
                {
                    (texture.Value as Texture2D).Dispose();
                }
            }
            Initialized = false;
            Game = null;
            baseDirectory = null;
            FileDictionary = null;
            aliasDictionary = null;
        }
    }

    public class LoadArgs
    {
        public DynamicContentManager Content;

        public static LoadArgs Empty { get { return new LoadArgs(); } }

        public LoadArgs() { }

        public LoadArgs(DynamicContentManager content)
        {
            Content = content;
        }
    }
}
