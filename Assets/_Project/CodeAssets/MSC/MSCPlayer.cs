using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

public class MSCPlayer : MonoBehaviour, SocketListener
{
    public static MSCPlayer Instance
    {
        get
        {
            if (m_instance == null)
            {
                //lock to assure only instance once.
                lock (temp)
                {
                    //another thread may wait outside the lock while this thread goes into lock and instanced singleton, so check null again when enter into lock.
                    if (m_instance == null)
                    {
                        var singleton = new GameObject();
                        singleton.name = "MSCSoundPlayer";

                        m_instance = singleton.AddComponent<MSCPlayer>();

                        DontDestroyOnLoad(singleton);
                    }
                }
            }

            return m_instance;
        }
    }

    private static MSCPlayer m_instance;
    private static readonly object temp = new object();

    public List<string> SoundTagList = new List<string>();

    public void PlaySound(int seq, ChatPct.Channel channel)
    {
        m_path = "/sdcard/msc/Bamboo/msc_" + (int)channel + "_" + seq + ".mp3";
        StopAllCoroutines();

        if (SoundTagList.Contains((int)channel + "_" + seq) && File.Exists(m_path))
        {
            StartCoroutine(PlaySound());
        }
        else
        {
            CGetYuYing tempReq = new CGetYuYing()
            {
                seq = seq,
                channel = channel
            };
            SocketHelper.SendQXMessage(tempReq, ProtoIndexes.C_Get_Sound);
        }
    }

    private string m_path;
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

        m_audioSource.clip = www.GetAudioClip(false, false, AudioType.MPEG);

        m_audioSource.Play();
    }

    void OnDestroy()
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
