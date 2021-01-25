using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Language
{
    // from there http://www.demonixis.net/ajout-du-multilingue-dans-votre-jeux-avec-unity-3d/ modified by me
    public static class Translation
    {
        private static Dictionary<string, string> Translations = null;

        public static void Init()
        {
            // It's already initialized.
            if (Translations != null)
                return;

            Translations = new Dictionary<string, string>();
        }

        /// <summary>
        /// Load the translation file specified
        /// </summary>
        /// <param name="lang">The target language</param>
        public static void LoadData(string lang)
        {
            // Load and parse the translation file from the Resources folder.
            try
            {
                // Load text file from /Resources/Translations/langFile
                TextAsset data = Resources.Load<TextAsset>($"Translations/{lang}");
                if (data != null)
                    ParseFile(data.text);
            }catch
            {
        #if UNITY_EDITOR
                //Debug.LogError("An error occured while loading the file");
        #endif
            }
        }

        // Returns the translation for this key.
        public static string Get(string key)
        {
            if (Translations.ContainsKey(key))
                return Translations[key];

        #if UNITY_EDITOR
                Debug.LogError($"The key \"{key}\" is missing");
        #endif

            return key;
        }

        public static void ParseFile(string data)
        {
            using (var stream = new StringReader(data))
            {
                var line = stream.ReadLine();
                var temp = new string[2];
                var key = string.Empty;
                var value = string.Empty;

                while (line != null)
                {
                    if (line.StartsWith(";") || line.StartsWith("["))
                    {
                        line = stream.ReadLine();
                        continue;
                    }

                    temp = line.Split('=');

                    if (temp.Length == 2)
                    {
                        key = temp[0].Trim();
                        value = temp[1].Trim();

                        if (value == string.Empty)
                            continue;

                        if (Translations.ContainsKey(key))
                            Translations[key] = value;
                        else
                            Translations.Add(key, value);
                    }

                    line = stream.ReadLine();
                }
            }
        }
    }
}