using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

[RequireComponent(typeof(AudioSource))]

public class SingleMicrophoneCapture : MonoBehaviour{

    private bool micConnected = false;
   
	private int minFreq;
    
	private int maxFreq;
    
	private AudioSource m_AudioSource;
   
	private UILabel m_label;

    float[] m_samples;
   
	private AudioClip m_playerClip;


    void Start()
    {
        m_label = this.GetComponentInChildren<UILabel>();

        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Microphone not connected!");
        }
        else
        {
            micConnected = true;

            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

			Debug.Log( "minFreq: " + minFreq );

			Debug.Log( "maxFreq: " + maxFreq );

            maxFreq = 4000;
            //maxFreq = 44100;

            m_AudioSource = this.GetComponent<AudioSource>();
        }
    }


    void OnClick()
    {
        if (micConnected == false) return;

        if (m_label.text == "Record")
        {
            if (!Microphone.IsRecording(null))
            {
                m_AudioSource.GetComponent<AudioSource>().clip = Microphone.Start(null, true,10, maxFreq);
            }
			m_label.text = "Begain";
        }
        else
        {
            m_label.text = "Send";
            Microphone.End(null);
            if (m_samples == null)
            { 
                m_samples = new float[m_AudioSource.clip.samples];

				SendPlayerSound();
            }
        }
    }

    void SendPlayerSound()
    {
		Debug.Log("send audio");

		if(m_samples == null)
		{
			Debug.Log("m_samples = null");
		}

		m_label.text = "Record";

        ChatPct tempSound = new ChatPct {senderId = JunZhuData.Instance().m_junzhuInfo.id};
        List<float> tempS = new List<float>();
        tempS.AddRange(m_samples);
		tempSound.soundData.AddRange(tempS.GetRange(0,m_samples.Length));

        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        t_qx.Serialize(t_tream,tempSound);

        byte[] t_protof;
        t_protof = t_tream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_get_sound, ref t_protof);
		
		m_samples = null;
    }

}