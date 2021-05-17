// Copyright 2021 Jolan Aklin

//This file is part of Prog the robot.

//Prog the robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog the robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System.Linq;
using UnityEngine.SceneManagement;
using Language;
using UnityEngine.Events;

public class SaveManager : MonoBehaviour
{
    [Tooltip("Create temporary files in this directory. Put / before and a / after")]
    public string tmpSavePath;
    [Tooltip("Save the compressed save file to this directory. Put / before and a / after")]
    public string savePath;
    [Tooltip("Extract the save file in this directory. Put / before and a / after")]
    public string extractPath;
    [Tooltip("Path to the saved app settings. Put / before and a / after")]
    public string settingsPath;

    public GameObject splineLink;

    public string filepath = "";

    public static SaveManager instance;

    public List<nodeObject> nodeObjects = new List<nodeObject>();
    // object used to fill the list of node object in the inspector
    [Serializable]
    public class nodeObject
    {
        public string nodeType;
        public GameObject gameObject;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }


        tmpSavePath = Application.persistentDataPath + tmpSavePath;
        savePath = Application.persistentDataPath + savePath;
        extractPath = Application.persistentDataPath + extractPath;
        settingsPath = Application.persistentDataPath + settingsPath;


        // create the directory if they don't exist
        if (!Directory.Exists(extractPath))
        {
            Directory.CreateDirectory(extractPath);
        }
        if (!Directory.Exists(tmpSavePath))
        {
            Directory.CreateDirectory(tmpSavePath);
        }
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        if(!Directory.Exists(settingsPath))
        {
            Directory.CreateDirectory(settingsPath);
        }

        LoadSettings();

    }

    private void Start()
    {
        if(!CommandLineManager.instance.LoadFromCmd())
            LoadFile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
        {
            Save();
        }
    }
    // start tpi
    /// <summary>
    /// Load the default file
    /// </summary>
    public void LoadFile()
    {
        CleanDir(extractPath);
#if UNITY_EDITOR
        JsonToObj(@"D:\001_TPI\projet\DefaultNewProject.pr");
#else
        JsonToObj(Application.dataPath + "/default/DefaultNewProject.pr");
#endif
    }
    // end tpi

    /// <summary>
    /// Load the specified file
    /// </summary>
    /// <param name="file">the path or the name of the file</param>
    /// <param name="isCompletePath">if true, use the file as a complete path. If false, search for the file in Application.PersistentDataPath</param>
    public void LoadFile(string file, bool isCompletePath)
    {
        if (isCompletePath)
            filepath = file;
        else
            filepath = Path.Combine(savePath + file);
        CleanDir(extractPath);
        JsonToObj(filepath);
    }


    /// <summary>
    /// Will create json for all the object that need to be saved
    /// </summary>
    public void Save()
    {
        StreamWriter sr;

        // create a serializable object to save all the robots
        SerializedRobotList serializedRobotList = new SerializedRobotList();
        serializedRobotList.serializedRobots = new List<Robot.SerializedRobot>();
        foreach (KeyValuePair<int, Robot> robot in Robot.robots)
        {
            serializedRobotList.serializedRobots.Add(robot.Value.SerializeRobot());
        }

        // convert the object to json and write it to a file
        sr = File.CreateText(tmpSavePath + "Robots");
        sr.WriteLine(JsonUtility.ToJson(serializedRobotList));
        sr.Close();

        // save next id
        sr = File.CreateText(tmpSavePath + "Ids");
        SaveId saveId = new SaveId()
        {
            robotNextId = Robot.nextid,
            robotScriptNextId = Robot.nextid,
            nodeNextId = Nodes.nextid,
        };
        sr.WriteLine(JsonUtility.ToJson(saveId));
        sr.Close();

        // save spline
        SplineList splineList = new SplineList() { serializedSplines = new List<SplineManager.SerializedSpline>() };
        foreach (SplineManager splineManager in SplineManager.splineManagers)
        {
            splineList.serializedSplines.Add(splineManager.SerializeSpline());
        }
        string splineJson = JsonUtility.ToJson(splineList);
        sr = File.CreateText(tmpSavePath + "Splines");
        sr.WriteLine(splineJson);
        sr.Close();

        // save the terrain
        sr = File.CreateText(tmpSavePath + "Terrain");
        sr.WriteLine(JsonUtility.ToJson(GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainManager>().Serialize()));
        sr.Close();

        // start tpi
        // save unassignedScripts
        sr = File.CreateText(tmpSavePath + "UnassignedScripts");
        SerializedUnassignedScripts serializedUnassignedScripts = new SerializedUnassignedScripts();
        serializedUnassignedScripts.scripts = new List<RobotScript.SerializedScriptsInRobotHierarchy>();
        // serialize all scripts before saving
        foreach (RobotScript.ScriptsInRobotHierarchy hierarchy in RobotScript.unassignedRobotScript)
        {
            RobotScript.SerializedScriptsInRobotHierarchy serializedScripts = new RobotScript.SerializedScriptsInRobotHierarchy();
            serializedScripts.main = hierarchy.main.SerializeScript();
            serializedScripts.childrens = new List<RobotScript.SerializedRobotScript>();
            foreach (RobotScript rs in hierarchy.childrens)
            {
                serializedScripts.childrens.Add(rs.SerializeScript());
            }
            serializedUnassignedScripts.scripts.Add(serializedScripts);
        }
        sr.WriteLine(JsonUtility.ToJson(serializedUnassignedScripts));
        sr.Close();

        //end tpi

        string[] files = new string[]
        {
            tmpSavePath + "Robots",
            tmpSavePath + "Splines",
            tmpSavePath + "Ids",
            tmpSavePath + "Terrain",
            tmpSavePath + "UnassignedScripts"
        };

        if (!filepath.EndsWith(".pr"))
            filepath += ".pr";

        // create a targz with the specified files
        CreateTarGZ(filepath, files);

        CleanDir(tmpSavePath);
    }


    public void SaveSettings()
    {
        StreamWriter sr = File.CreateText(settingsPath + "settings");
        sr.WriteLine(JsonUtility.ToJson(new Settings() { savePath = savePath, alwaysShowConnectHandle = Manager.instance.connectHandleAlwaysShown}));
        sr.Close();
    }

    public void LoadSettings()
    {
        if(File.Exists(settingsPath + "settings"))
        {
            StreamReader sr = File.OpenText(settingsPath + "settings");
            string content = sr.ReadToEnd();
            Settings settings = JsonUtility.FromJson<Settings>(content);

            savePath = settings.savePath;
            Manager.instance.connectHandleAlwaysShown = settings.alwaysShowConnectHandle;
        }
    }

    /// <summary>
    /// Create obj from the json file stored in the archive
    /// </summary>
    /// <param name="archivePath">Path were the archive is stored</param>
    public void JsonToObj(string archivePath)
    {
        // extract the archive
        ExtractTGZ(archivePath, extractPath);

        SerializedRobotList serializedRobotList = null;
        SaveId saveId = null;
        SplineList splineList = null;
        TerrainManager.SerializedTerrain serializedTerrain = null;
        string unassignedScripts = "";

        // read all files and and create object from the json
        foreach (string file in Directory.EnumerateFiles(extractPath))
        {
            StreamReader sr = File.OpenText(file);
            string fileContent = sr.ReadToEnd();
            if(file.EndsWith("Ids"))
            {
                saveId = JsonUtility.FromJson<SaveId>(fileContent);
            }else if (file.EndsWith("Splines"))
            {
                splineList = JsonUtility.FromJson<SplineList>(fileContent);
            }else if (file.EndsWith("Terrain"))
            {
                serializedTerrain = JsonUtility.FromJson<TerrainManager.SerializedTerrain>(fileContent);
            }
            else if(file.EndsWith("Robots"))
            {
                //serializedRobots.Add(JsonUtility.FromJson<Robot.SerializedRobot>(fileContent));
                serializedRobotList = JsonUtility.FromJson<SerializedRobotList>(fileContent);
            }
            else if(file.EndsWith("UnassignedScripts"))
            {
                unassignedScripts = fileContent;
            }
        }
        List<Robot.SerializedRobot> serializedRobots = new List<Robot.SerializedRobot>();
        foreach (Robot.SerializedRobot serializedRobot in serializedRobotList.serializedRobots)
        {
            serializedRobots.Add(serializedRobot);
        }

        // clean the app before creating the usable object
        ClearGame(saveId, serializedRobots, splineList, serializedTerrain, unassignedScripts);
    }

    //https://stackoverflow.com/questions/31836519/how-to-create-tar-gz-file-in-c-sharp modified by me
    /// <summary>
    /// Create a tarGZ archive
    /// </summary>
    /// <param name="tgzFilename">Where the archive will be stored</param>
    /// <param name="fileNames">an array of all the file that need to be compressed</param>
    private void CreateTarGZ(string tgzFilename, string[] fileNames)
    {
        using (var outStream = File.Create(tgzFilename))
        using (var gzoStream = new GZipOutputStream(outStream))
        using (var tarArchive = TarArchive.CreateOutputTarArchive(gzoStream))
        {
            foreach (string fileName in fileNames)
            {
                tarArchive.RootPath = Path.GetDirectoryName(fileName);

                var tarEntry = TarEntry.CreateEntryFromFile(fileName);
                tarEntry.Name = Path.GetFileName(fileName);

                tarArchive.WriteEntry(tarEntry, true);
            }
            tarArchive.Close();
            gzoStream.Close();
            outStream.Close();
        }
    }

    // from https://github.com/icsharpcode/SharpZipLib/wiki/GZip-and-Tar-Samples#anchorTGZ
    // example: ExtractTGZ(basePath + "save.pr", basePath + "extract");
    /// <summary>
    /// Extract a tarGZ archive
    /// </summary>
    /// <param name="gzArchiveName">path to the archive to extract</param>
    /// <param name="destFolder">path of the desination folder</param>
    public void ExtractTGZ(string gzArchiveName, string destFolder)
    {
        Stream inStream = File.OpenRead(gzArchiveName);
        Stream gzipStream = new GZipInputStream(inStream);

        TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream, System.Text.Encoding.UTF8);
        tarArchive.ExtractContents(destFolder);
        tarArchive.Close();

        gzipStream.Close();
        inStream.Close();
    }

    public void CleanDir(string directoryPath)
    {
        string[] files = Directory.GetFiles(directoryPath);
        foreach (string file in files)
        {
            File.Delete(file);
        }
    }

    /// <summary>
    /// clear the app before loading
    /// </summary>
    public void ClearGame(SaveId saveId, List<Robot.SerializedRobot> serializedRobots, SplineList splineList, TerrainManager.SerializedTerrain serializedTerrain, string unassignedScripts)
    {
        NewProject();
        IEnumerator coroutine = LoadScene(saveId, serializedRobots, splineList, serializedTerrain, unassignedScripts);
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// Prepare the app for a new project
    /// </summary>
    private void NewProject()
    {
        //reset all the static vars
        RobotScript.nextid = 0;
        RobotScript.robotScripts = new Dictionary<int, RobotScript>();
        RobotScript.unassignedRobotScript = new List<RobotScript.ScriptsInRobotHierarchy>();


        foreach (KeyValuePair<int, Robot> robot in Robot.robots)
        {
            Destroy(robot.Value.robotManager);
        }
        Robot.nextid = 0;
        Robot.robots = new Dictionary<int, Robot>();
        Robot.idSelected = 0;

        RobotScript.nextid = 0;
        RobotScript.robotScripts = new Dictionary<int, RobotScript>();

        Nodes.nextid = 0;
        Nodes.NodesDict = new Dictionary<int, Nodes>();

        SplineManager.splineManagers = new List<SplineManager>();
    }

    public void New()
    {
        NewProject();
        StartCoroutine("LoadSceneSimple");
    }

    // create usable objects from the saved form
    private void LoadObject(SaveId saveId, List<Robot.SerializedRobot> serializedRobots, SplineList splineList, TerrainManager.SerializedTerrain serializedTerrain, string unassignedScripts)
    {
        // create all the robot from the json created object
        foreach (Robot.SerializedRobot serializedRobot in serializedRobots)
        {
            Vector3 position = new Vector3(serializedRobot.position[0], serializedRobot.position[1], serializedRobot.position[2]);
            Quaternion rotation = new Quaternion(serializedRobot.rotation[0], serializedRobot.rotation[1], serializedRobot.rotation[2], serializedRobot.rotation[3]);

            // this constructor will also create all scripts
            Robot robot = new Robot(serializedRobot.id, serializedRobot.power, serializedRobot.robotColor, serializedRobot.robotName, position, rotation, serializedRobot.serializedRobotScripts, this);
            Manager.instance.listRobot.AddChoice(robot.id, robot.ConvertToListElement());
            Manager.instance.listRobot.Select(robot.id);
        }

        foreach (Nodes node in Nodes.NodesDict.Values)
        {
            node.FindParent();
        }
        Transform nodeHolder = GameObject.FindGameObjectWithTag("NodeHolder").transform;
        // create all links from the json created object
        foreach (SplineManager.SerializedSpline serializedSpline in splineList.serializedSplines)
        {
            GameObject splineLinkInstance = Instantiate(splineLink, new Vector3(0, 0, -899), Quaternion.identity, nodeHolder);
            SplineManager splineManager = splineLinkInstance.GetComponent<SplineManager>();
            splineManager.DeSerializeSpline(serializedSpline);
            Nodes.NodesDict[serializedSpline.idNodeStart].currentSplines[serializedSpline.handleStart] = splineManager;
        }

        // hide all nodes and splines from all scripts
        foreach (RobotScript rs in RobotScript.robotScripts.Values)
        {
            rs.HideNodesForThisScript();
        }

        //start tpi
        // create all unassigned scripts
        DeSerializedUnassignedScripts(unassignedScripts);
        // hide unassignedScripts
        foreach (RobotScript.ScriptsInRobotHierarchy hierarchy in RobotScript.unassignedRobotScript)
        {
            hierarchy.main.HideNodesForThisScript();
            foreach (RobotScript rs in hierarchy.childrens)
            {
                rs.HideNodesForThisScript();
            }
        }
        //end tpi

        Manager.instance.listRobot.SelectFirst();

        // create the terrain
        GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainManager>().DeSerialize(serializedTerrain);

        // set the next id when a new of this object will be created
        RobotScript.nextid = saveId.robotScriptNextId;
        Robot.nextid = saveId.robotNextId;
        Nodes.nextid = saveId.nodeNextId;


        GC.Collect();
    }

    public GameObject InstantiateSavedObj(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(gameObject, position, rotation, parent);
    }

    // reload the current scene to remove all unwanted object on the load of a new file
    private IEnumerator LoadScene(SaveId saveId, List<Robot.SerializedRobot> serializedRobots, SplineList splineList, TerrainManager.SerializedTerrain serializedTerrain, string unassignedScripts)
    {
        // Start loading the scene
        Scene scene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Single);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone)
            yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
        LoadObject(saveId, serializedRobots, splineList, serializedTerrain, unassignedScripts);
    }
    private IEnumerator LoadSceneSimple()
    {
        // Start loading the scene
        Scene scene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Single);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone)
            yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
    }


    [Serializable]
    public class SaveId
    {
        public int robotNextId;
        public int robotScriptNextId;
        public int nodeNextId;
    }

    [Serializable]
    public class SplineList
    {
        [SerializeField]
        public List<SplineManager.SerializedSpline> serializedSplines;
    }

    [Serializable]
    public class SerializedRobotList
    {
        [SerializeField]
        public List<Robot.SerializedRobot> serializedRobots;
    }

    [Serializable]
    public class Settings
    {
        public string savePath;
        public bool alwaysShowConnectHandle;
    }

    // the class that will be converted to json to save unassigned scripts
    [Serializable]
    public class SerializedUnassignedScripts
    {
        [SerializeField]
        public List<RobotScript.SerializedScriptsInRobotHierarchy> scripts;
    }

    private void DeSerializedUnassignedScripts(string json)
    {
        RobotScript.unassignedRobotScript = new List<RobotScript.ScriptsInRobotHierarchy>();
        SerializedUnassignedScripts serializedUnassignedScripts = JsonUtility.FromJson<SerializedUnassignedScripts>(json);
        foreach (RobotScript.SerializedScriptsInRobotHierarchy hierarchy in serializedUnassignedScripts.scripts)
        {
            RobotScript robotScript = new RobotScript();
            robotScript.DeSerializeScript(hierarchy.main);
            RobotScript.unassignedRobotScript.Add(robotScript.DeSerializedScriptsInRobotHierarchy(hierarchy));
        }

    }
}
