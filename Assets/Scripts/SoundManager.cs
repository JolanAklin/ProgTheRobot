using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

// strat tpi
/// <summary>
/// <see cref="https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestMultimedia.GetAudioClip.html"/> and <see cref="https://www.youtube.com/watch?v=9gAHZGArDgU"/>
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private AudioClip audioClip;
    private string loadPath;
    private string webLoadPath;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // change loadPath dir depending of the environment
#if UNITY_EDITOR
        loadPath = $@"D:\001_TPI\projet\sounds";
#else
        loadPath = $@"{Application.dataPath}/sounds";
#endif
        if(!Directory.Exists(loadPath))
        {
            Debug.LogError("the specified sound directory cannot be found");
            return;
        }

        webLoadPath = $@"file://{loadPath}";
        loadAllAudioClip();
    }

    /// <summary>
    /// Get all the loaded sounds name
    /// </summary>
    /// <returns>All the loaded sounds name</returns>
    public string[] GetAllAudioNames()
    {
        string[] audioNames = new string[audioClips.Count];
        int i = 0;
        foreach (string audioName in audioClips.Keys)
        {
            audioNames[i] = audioName;
            i++;
        }
        return audioNames;
    }

    /// <summary>
    /// Check if the sound manager has a sound
    /// </summary>
    /// <param name="audioName">The sound to check if it extists</param>
    /// <returns></returns>
    public bool HasAudio(string audioName)
    {
        if (!audioClips.ContainsKey(audioName))
            return false;
        return true;
    }

    /// <summary>
    /// Play asynchronously the sound
    /// </summary>
    /// <param name="audioName">The sound to play</param>
    /// <returns></returns>
    public bool Play(string audioName, AudioSource audio)
    {
        /*
        * the audio source need to be one the robot
        */
        if (!audioClips.ContainsKey(audioName))
            return false;
        audio.clip = audioClips[audioName];
        audio.Play();
        return true;
    }

    /// <summary>
    /// Play asynchronously the sound and make a callback when it ends
    /// </summary>
    /// <param name="audioName">The sound to play</param>
    /// <param name="callback">The action that will be call back when the sound ends</param>
    /// <returns></returns>
    public bool PlaySync(string audioName, AudioSource audio, Action callback)
    {
        /*
        * the audio source need to be one the robot
        */
        if (!audioClips.ContainsKey(audioName))
            return false;
        AudioClip audioClip = audioClips[audioName];
        audio.clip = audioClip;
        audio.Play();
        IEnumerator coroutine = Wait(audioClip.length, callback);
        StartCoroutine(coroutine);
        return true;
    }

    /// <summary>
    /// Wait a certain amount of time and make a callback
    /// </summary>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator Wait(float time, Action callback)
    {
        yield return new WaitForSecondsRealtime(time);
        callback?.Invoke();
    }

    /// <summary>
    /// Load all audio clip found in the loadPath
    /// </summary>
    private void loadAllAudioClip()
    {
        foreach (string audio in Directory.GetFiles(loadPath))
        {
            string audioFile = Path.GetFileName(audio);
            IEnumerator coroutine = null;
            string extension = Path.GetExtension(Path.Combine(loadPath, audioFile));
            switch (extension)
            {
                case ".mpeg3":
                case ".mp3":
                    coroutine = GetAudioClip(Path.Combine(webLoadPath, audioFile), AudioType.MPEG);
                    break;
                case ".ogg":
                    coroutine = GetAudioClip(Path.Combine(webLoadPath, audioFile), AudioType.OGGVORBIS);
                    break;
                default:
                    continue;
            }
            StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Get an audio clip
    /// </summary>
    /// <param name="filePath">Path to the sound to load. Use a web path (ex. : file://...)</param>
    /// <param name="audioType">The type of the audio file to load</param>
    /// <returns></returns>
    IEnumerator GetAudioClip(string filePath, AudioType audioType)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioClips.Add(Path.GetFileNameWithoutExtension(filePath), audioClip);
            }
        }
    }
}

// end tpi
