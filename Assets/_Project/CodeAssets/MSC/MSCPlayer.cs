using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

public class MSCPlayer : Singleton<MSCPlayer>, SocketListener
{
    public List<string> SoundTagList = new List<string>();

    public void PlaySound(int seq, ChatPct.Channel channel)
    {
        if (m_AudioSource.isPlaying && m_path == "/sdcard/msc/Bamboo/msc_" + (int)channel + "_" + seq + ".mp3")
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
            {
                Debug.Log("Stop play " + m_path + " sound.");
            }

            stopSound();
            return;
        }

        m_path = "/sdcard/msc/Bamboo/msc_" + (int)channel + "_" + seq + ".mp3";

        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
        {
            Debug.Log(m_path);
        }

        StopAllCoroutines();

        if (SoundTagList.Contains((int)channel + "_" + seq) && File.Exists(m_path))
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
            {
                Debug.Log("play sound directly");
            }

            StartCoroutine(PlaySound());
        }
        else
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
            {
                Debug.Log("request sound message.");
            }

            CGetYuYing tempReq = new CGetYuYing()
            {
                seq = seq,
                channel = channel
            };
            SocketHelper.SendQXMessage(tempReq, ProtoIndexes.C_Get_Sound);
        }
    }

    private void LogAudioSourceInfo(string p_prefix = "")
    {
        Debug.Log(p_prefix +
            ComponentHelper.GetAudioSourcePlayTime(m_AudioSource) + " / " +
            ComponentHelper.GetAudioClipLength(m_AudioSource.clip));
    }

    public void stopSound()
    {
        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC) && m_AudioSource.isPlaying)
        {
            Debug.Log("Trying to stop a playing audio");
        }

        StopAllCoroutines();

        if (m_AudioSource.isPlaying)
        {
            m_AudioSource.Stop();
        }

        if (ExecuteAfterPlayEnds != null)
        {
            ExecuteAfterPlayEnds();
        }

        SoundManager.EYuyin();
    }

    private string m_path;

    public AudioSource m_AudioSource
    {
        get
        {
            if (m_audioSource == null)
            {
                m_audioSource = gameObject.AddComponent<AudioSource>();
            }
            return m_audioSource;
        }
    }

    private AudioSource m_audioSource;

    private IEnumerator PlaySound()
    {
        WWW www = new WWW("file://" + m_path);
        yield return www;

        if (www.bytes.Length <= 0)
        {
            Debug.Log("Cancel play sound: " + m_path + " cause file is empty.");
            yield break;
        }

        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
        {
            Debug.Log("play sound with length: " + www.bytes.Length);
        }


        m_AudioSource.clip = www.GetAudioClip(false, false);

        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
        {
            Debug.Log("audio clip length: " + m_AudioSource.clip.length);

        }

        {
            m_AudioSource.volume = 0.0f;

            LeanTween.value(gameObject, 0.0f, 1.0f, 1.0f).setEase(LeanTweenType.easeInQuart).setOnUpdate(UpdateVolume);
        }


        m_AudioSource.Play();
        SoundManager.BYuyin();

        StartCheck(m_AudioSource.clip.length);
    }

    public void UpdateVolume(float p_volume)
    {
        m_AudioSource.volume = p_volume;

    }

    private bool isCheckPlay = false;
    private float m_check_start_time = 0.0f;
    private float m_check_end_time = 0.0f;

    private void StartCheck(float p_clip_duration)
    {
        isCheckPlay = true;

        m_check_start_time = Time.realtimeSinceStartup;

        m_check_end_time = m_check_start_time + p_clip_duration + 2.0f;
    }

    private void StopCheck()
    {
        isCheckPlay = false;
    }

    public DelegateHelper.VoidDelegate ExecuteAfterPlayEnds;

    void Update()

    {
        if (isCheckPlay)
        {
            if (Time.realtimeSinceStartup > m_check_end_time)
            {
                if (!m_AudioSource.isPlaying)
                {
                    if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
                    {
                        Debug.Log("execute playing ends");
                    }

                    if (ExecuteAfterPlayEnds != null)
                    {
                        ExecuteAfterPlayEnds();

                        StopCheck();
                    }

					SoundManager.EYuyin();
                }
            }
        }
    }

    new void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        SoundTagList.ForEach(item =>
        {
            if (File.Exists("/sdcard/msc/Bamboo/msc_" + item + ".mp3"))
            {
                File.Delete("/sdcard/msc/Bamboo/msc_" + item + ".mp3");
            }
        });

        SoundTagList.Clear();

        Destroy(m_audioSource);
        m_audioSource = null;

        base.OnDestroy();
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);

        m_audioSource = gameObject.AddComponent<AudioSource>();
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_Get_Sound:
                    {
                        object tempReveive = new SGetYuYing();
                        SocketHelper.ReceiveQXMessage(ref tempReveive, p_message, ProtoIndexes.S_Get_Sound);
                        var tempSound = tempReveive as SGetYuYing;

                        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_MSC))
                        {
                            Debug.Log("receive sound data: " + tempSound.soundData.Length);
                        }

                        FileStream stream = File.Open(m_path, FileMode.OpenOrCreate);
                        byte[] bytes = EncryptTool.GetBytesFrom64(tempSound.soundData);
                        foreach (byte b in bytes)
                        {
                            stream.WriteByte(b);
                        }
                        stream.Close();

                        if (!SoundTagList.Contains((int)tempSound.channel + "_" + tempSound.seq))
                        {
                            SoundTagList.Add((int)tempSound.channel + "_" + tempSound.seq);
                        }

                        StartCoroutine(PlaySound());

                        return true;
                    }
            }
        }
        return false;
    }
}