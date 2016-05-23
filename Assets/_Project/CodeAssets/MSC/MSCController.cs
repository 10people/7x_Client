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
        var splited = result.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        if (splited.Count != 3)
        {
            Debug.LogError("Error in split MSCResult.");
            return;
        }

        m_MscListenerList.ForEach(item => item.MSCResult(splited[0], splited[1], EncryptTool.Get64StringFromBytes(File.ReadAllBytes(splited[2]))));

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
        m_MscListenerList.ForEach(item => item.MSCError(error));
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
}
