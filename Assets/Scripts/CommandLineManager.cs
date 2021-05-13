using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// start tpi
public class CommandLineManager : MonoBehaviour
{
    private string file;
    public static CommandLineManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Load the file specified in the command line argument when lauching the program
    /// </summary>
    /// <returns></returns>
    public bool LoadFromCmd()
    {
        ParseCommandLineArguments();
        if (File.Exists(this.file))
        {
            SaveManager.instance.LoadFile(this.file, true);
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Get the file specified in the command line arguments
    /// </summary>
    private void ParseCommandLineArguments()
    {
        string[] args = Environment.GetCommandLineArgs();
        this.file = "";

        for (int i = 1; i < args.Length; i++)
        {
            this.file += args[i];
            if (i < args.Length - 1)
                this.file += " ";
        }
    }
}
// end tpi
