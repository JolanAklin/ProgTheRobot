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

public class SaveManager : MonoBehaviour
{
    [Tooltip("Create temporary files in this directory. Put / before and a / after")]
    public string tmpSavePath;
    [Tooltip("Save the compressed save file to this directory. Put / before and a / after")]
    public string savePath;
    [Tooltip("Extract the save file in this directory. Put / before and a / after")]
    public string extractPath;

    public string destroyerScene;

    private void Awake()
    {
        tmpSavePath = Application.persistentDataPath + tmpSavePath;
        savePath = Application.persistentDataPath + savePath;
        extractPath = Application.persistentDataPath + extractPath;

        DontDestroyOnLoad(this.gameObject);

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftControl))
        {
            Save();
        }

        if(Input.GetKeyDown(KeyCode.L) && Input.GetKeyDown(KeyCode.LeftControl))
        {
            JsonToObj(savePath + "save.pr");
            //CleanDir(extractPath);
        }
    }


    /// <summary>
    /// Will create json for all the object that need to be saved
    /// </summary>
    public void Save()
    {
        StreamWriter sr;
        
        // save the robot with his script and their nodes
        foreach (KeyValuePair<int,Robot> robot in Robot.robots)
        {
            string jsonRobot = JsonUtility.ToJson(robot.Value.SerializeRobot());
            // check if the directory exist if not creates it

            sr = File.CreateText(tmpSavePath + robot.Value.robotName);
            sr.WriteLine(jsonRobot);
            sr.Close();
        }

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


        //convert to a tar archive
        string[] files = new string[Robot.robots.Count + 2];
        int i = 0;
        foreach (KeyValuePair<int, Robot> robot in Robot.robots)
        {
            files[i] = tmpSavePath + robot.Value.robotName;
            i++;
        }
        files[i] = tmpSavePath + "Splines";
        i++;
        files[i] = tmpSavePath + "Ids";


        CreateTarGZ(savePath + "save.pr", files);
        CleanDir(tmpSavePath);
    }


    /// <summary>
    /// Create obj from the json file stored in the archive
    /// </summary>
    /// <param name="archivePath">Path were the archive is stored</param>
    public void JsonToObj(string archivePath)
    {
        ExtractTGZ(archivePath, extractPath);

        SaveId saveId = null;
        List<Robot.SerializedRobot> serializedRobots = new List<Robot.SerializedRobot>();
        SplineList splineList = null;

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
            }
            else
            {
                serializedRobots.Add(JsonUtility.FromJson<Robot.SerializedRobot>(fileContent));
            }
        }
        ClearGame(saveId, serializedRobots, splineList);
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
    public void ClearGame(SaveId saveId, List<Robot.SerializedRobot> serializedRobots, SplineList splineList)
    {
        //reset all the static vars
        RobotScript.nextid = 0;
        RobotScript.robotScripts = new Dictionary<int, RobotScript>();

        Robot.nextid = 0;
        Robot.robots = new Dictionary<int, Robot>();
        Robot.idSelected = 0;

        RobotScript.nextid = 0;
        RobotScript.robotScripts = new Dictionary<int, RobotScript>();

        Nodes.nextid = 0;
        Nodes.NodesDict = new Dictionary<int, Nodes>();

        SplineManager.splineManagers = new List<SplineManager>();

        IEnumerator coroutine = LoadScene(saveId, serializedRobots, splineList);
        StartCoroutine(coroutine);
    }

    private void LoadObject(SaveId saveId, List<Robot.SerializedRobot> serializedRobots, SplineList splineList)
    {
        foreach (Robot.SerializedRobot serializedRobot in serializedRobots)
        {
            Robot robot = new Robot(serializedRobot.id, serializedRobot.power, serializedRobot.robotColor, serializedRobot.robotName, serializedRobot.serializedRobotScripts);
            Manager.instance.listRobot.AddChoice(robot.id, robot.ConvertToListElement());
            Manager.instance.listRobot.Select(robot.id);
        }

        RobotScript.nextid = saveId.robotScriptNextId;
        Robot.nextid = saveId.robotNextId;
        Nodes.nextid = saveId.nodeNextId;
    }

    private IEnumerator LoadScene(SaveId saveId, List<Robot.SerializedRobot> serializedRobots, SplineList splineList)
    {
        // Start loading the scene
        Scene scene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Single);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone)
            yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
        LoadObject(saveId, serializedRobots, splineList);
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
}
