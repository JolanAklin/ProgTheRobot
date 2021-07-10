using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    /// <summary>
    /// Singleton representing the current language manager
    /// </summary>
    public static LanguageManager instance;

    private void Awake()
    {
        InverseKV();
        instance = this;
    }

    /* one string in clear language and one string with function name abreviated
     * displayed string :   Wall distance = 10 And Wall right
     * abreviated string :  iwd = 10 And bwr
     * i = integer function and b = boolean function
     */
    // should be loaded dynamicly with a language file in the future
    private static Dictionary<string, string> abrevToFullName = new Dictionary<string, string>();
    private static Dictionary<string, string> fullNameToAbrev = new Dictionary<string, string>()
    {
        // boolean func
        {"Wall in front","bwf#"},
        {"Wall right","bwr#"},
        {"Wall left","bwl#"},
        {"Out","bo#"},
        {"Robot on an outlet","boao#"},
        {"Tile marked","btm#"},
        {"Ball on the ground","bbg#"},
        // int func
        {"Wall distance","iwd#"},
        {"Power","ip#"},
        {"x robot","ixr#"},
        {"y robot","iyr#"},
        {"dx robot","idxr#"},
        {"dy robot","idyr#"},
        {"x ball","ixb#"},
        {"y ball","iyb#"},
    };

    /// <summary>
    /// Get the fullname of a function by using it's abreviation
    /// </summary>
    /// <param name="abrev">The abreviation used to return the fullname</param>
    /// <returns>The fullname of an abreviation</returns>
    public string getFullnameFromAbrev(string abrev)
    {
        if(abrevToFullName.ContainsKey(abrev))
        {
            return abrevToFullName[abrev];
        }
        return null;
    }

    /// <summary>
    /// Test if a key is present in the abreviation to fullname dictionnary
    /// </summary>
    /// <param name="key">The key to test for</param>
    /// <returns>True if the key is present</returns>
    public bool AbrevToFullNameContainsKey(string key)
    {
        return abrevToFullName.ContainsKey(key);
    }

    /// <summary>
    /// Get the abreviation of a function by using it's fullname
    /// </summary>
    /// <param name="fullname">The fullname used to return the abreviation</param>
    /// <returns>The abreviation of an fullname</returns>
    public string getAbrevFromFullname(string fullname)
    {
        if (fullNameToAbrev.ContainsKey(fullname))
        {
            return fullNameToAbrev[fullname];
        }
        return null;
    }

    /// <summary>
    /// Test if a key is present in the fullname to abreviation dictionnary
    /// </summary>
    /// <param name="key">The key to test for</param>
    /// <returns>True if the key is present</returns>
    public bool FullNameToAbrevContainsKey(string key)
    {
        return fullNameToAbrev.ContainsKey(key);
    }

    /// <summary>
    /// inverse the key and the value from the fullname to abreviation dictionary
    /// </summary>
    private static void InverseKV()
    {
        foreach (KeyValuePair<string, string> item in fullNameToAbrev)
        {
            abrevToFullName.Add(item.Value, item.Key);
        }
    }

    /// <summary>
    /// Convert all fullname to abreviation present in a string
    /// </summary>
    /// <param name="toConvert">String to convert</param>
    /// <returns>The converted string</returns>
    public string FullNameToAbrev(string toConvert)
    {
        foreach (KeyValuePair<string, string> item in fullNameToAbrev)
        {
            toConvert = toConvert.Replace(item.Key, item.Value);
        }
        return toConvert;
    }
}
