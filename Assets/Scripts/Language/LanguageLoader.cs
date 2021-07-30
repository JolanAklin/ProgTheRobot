using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Language.Loader
{
    public class LanguageLoader : MonoBehaviour
    {
        public enum SupportedLanguages
        {
            eng,
        }
        public enum FileType
        {
            functions,
            ui,
            errors
        }
        private static Dictionary<FileType, string> filePath = new Dictionary<FileType, string>()
        {
            {FileType.functions, $"Language/Functions/" },
            {FileType.ui, $"Language/UI/" },
        };

        public static Dictionary<string, string> Load(SupportedLanguages lang, FileType fileType)
        {
            TextAsset data = Resources.Load<TextAsset>(filePath[fileType] + lang.ToString());
            if (data != null)
                return ParseFile(data.text);
            return null;
        }

        // from there http://www.demonixis.net/ajout-du-multilingue-dans-votre-jeux-avec-unity-3d/
        private static Dictionary<string, string> ParseFile(string data)
        {
            Dictionary<string, string> kvDict = new Dictionary<string, string>();
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

                        if (kvDict.ContainsKey(key))
                            kvDict[key] = value;
                        else
                            kvDict.Add(key, value);
                    }

                    line = stream.ReadLine();
                }
            }
            return kvDict;
        }
    }
}
