using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Register MSCListener to get MSC result.
/// </summary>
public class MSCController : Singleton<MSCController>
{
    public static List<IMSCListener> m_MscListenerList = new List<IMSCListener>();

    public static void RegisterListener(IMSCListener p_listener)
    {
        if (!m_MscListenerList.Contains(p_listener))
        {
            m_MscListenerList.Add(p_listener);
        }
    }

    public static void UnregisterListener(IMSCListener p_listener)
    {
        if (m_MscListenerList.Contains(p_listener))
        {
            m_MscListenerList.Remove(p_listener);
        }
    }

    public List<string> SoundTagList = new List<string>();

    public void StartMSC(string tag)
    {
#if UNITY_ANDROID
        AndroidJavaClass tempClass = new AndroidJavaClass("com.tencent.tmgp.qixiongwushuang.UnityPlayerActivity");
        tempClass.CallStatic("StartRecognize", tag);
#endif
    }

    public void StopMSC()
    {
#if UNITY_ANDROID
        AndroidJavaClass tempClass = new AndroidJavaClass("com.tencent.tmgp.qixiongwushuang.UnityPlayerActivity");
        tempClass.CallStatic("StopRecognize");
#endif
    }

    public void CancelMSC()
    {
#if UNITY_ANDROID
        AndroidJavaClass tempClass = new AndroidJavaClass("com.tencent.tmgp.qixiongwushuang.UnityPlayerActivity");
        tempClass.CallStatic("CancelRecognize");
#endif
    }

    public void MSCLog(string log)
    {
        Debug.LogWarning("MSC_LOG" + log);
    }

    public void MSCResult(string result)
    {
        StopCoroutine("DoProcessMSCResult");
        StartCoroutine(DoProcessMSCResult(result));
    }

    private IEnumerator DoProcessMSCResult(string result)
    {
        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
        {
            Debug.Log(result);
        }

        var splited = result.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        if (splited.Count != 3)
        {
            Debug.LogError("Error in split MSCResult.");
            yield break;
        }

        byte[] fileBytes = File.ReadAllBytes(splited[1]);
        int readCount = 1;

        while (fileBytes.Length == 0)
        {
            if (readCount >= 10)
            {
                Debug.LogError("Cannot read bytes from file: " + splited[1] + " cause bytes is empty after reading for 10 times.");
                yield break;
            }

            yield return new WaitForSeconds(.5f);

            fileBytes = File.ReadAllBytes(splited[1]);
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
            {
                Debug.Log(fileBytes.Length);
            }

            readCount++;
        }

        ////Process file with cutting first 0.5s audio.
        //int bitRate = 8000;
        //int cutBytesCount = (int)(bitRate / 8f / 1000 * 1024 * float.Parse(seconds));
        //if (fileBytes.Length > cutBytesCount)
        //{
        //    fileBytes = fileBytes.Skip(cutBytesCount).ToArray();

        //    File.WriteAllBytes(splited[1], fileBytes);
        //}

        string fileString = EncryptTool.Get64StringFromBytes(fileBytes);
        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
        {
            Debug.Log(fileString.Length);
        }

        m_MscListenerList.ForEach(item => item.MSCResult(splited[0], splited[2], fileString));

        if (!SoundTagList.Contains(splited[0]))
        {
            SoundTagList.Add(splited[0]);
        }
    }

    public void MSCStarted(string info)
    {
        m_MscListenerList.ForEach(item => item.MSCStarted());
    }

    public void MSCEnded(string info)
    {
        m_MscListenerList.ForEach(item => item.MSCEnded());
    }

    public void MSCError(string error)
    {
        int errorCode = int.Parse(error);

        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
        {
            Debug.Log("MSCError, code:" + errorCode);
        }

        if (errorCode == 11606)
        {
            m_MscListenerList.ForEach(item => item.MSCError(LanguageTemplate.GetText(5102)));
        }
        else if (errorCode == 11605 || errorCode == 10118)
        {
            m_MscListenerList.ForEach(item => item.MSCError(LanguageTemplate.GetText(5103)));
        }
        else
        {
            m_MscListenerList.ForEach(item => item.MSCError(LanguageTemplate.GetText(5106)));
        }
    }

    public void MSCVolume(string vol)
    {
        m_MscListenerList.ForEach(item => item.MSCVolume(int.Parse(vol)));
    }

    void Awake()
    {

    }

    new void OnDestroy()
    {
        SoundTagList.ForEach(item =>
        {
            if (File.Exists("/sdcard/msc/Bamboo/msc_" + item + ".mp3"))
            {
                File.Delete("/sdcard/msc/Bamboo/msc_" + item + ".mp3");
            }

            if (File.Exists("/sdcard/msc/Bamboo/msc_" + item + ".wav"))
            {
                File.Delete("/sdcard/msc/Bamboo/msc_" + item + ".wav");
            }
        });

        SoundTagList.Clear();

        base.OnDestroy();
    }

    //public string seconds = "0.5";

    //void OnGUI()
    //{
    //    seconds = GUILayout.TextArea(seconds, GUILayout.Width(200), GUILayout.MinHeight(100));
    //}
}
