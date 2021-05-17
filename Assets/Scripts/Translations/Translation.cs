// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Language
{
    // from there http://www.demonixis.net/ajout-du-multilingue-dans-votre-jeux-avec-unity-3d/ modified by me
    public static class Translation
    {
        private static string currentLanguage;
        public static string CurrentLanguage { get => currentLanguage; private set => currentLanguage = value; }

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
            CurrentLanguage = lang;
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

            Debug.LogError($"The key \"{key}\" is missing");

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