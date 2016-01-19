﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
[RequireComponent (typeof(AudioSource))]

public class MicroPhoneInput : MonoBehaviour {

	private static MicroPhoneInput m_instance;

	public float sensitivity=100;
	public float loudness=0;

    private static string[] micArray=null;

    const int HEADER_SIZE = 44;

    const int RECORD_TIME = 10;

	// Use this for initialization
	void Start(){
	}

	public static MicroPhoneInput getInstance()
	{
		if (m_instance == null) 
		{
            micArray = Microphone.devices;
            if (micArray.Length == 0)
            {
                Debug.LogError ("Microphone.devices is null");
            }
            foreach (string deviceStr in Microphone.devices)
            {
                Debug.Log("device name = " + deviceStr);
            }
			if(micArray.Length==0)
			{
				Debug.LogError("no mic device");
			}

			GameObject MicObj=new GameObject("MicObj");
			m_instance= MicObj.AddComponent<MicroPhoneInput>();
		}
		return m_instance;
	}

	void OnDestroy(){
		m_instance = null;

		micArray = null;
	}

	void OnGUI()
    {
		GUI.Label(new Rect(10,10,200,100),"loudness = "+loudness);
		GUI.Label(new Rect(10,210,200,100),"Microphone.GetPosition = "+Microphone.GetPosition(null));
	}

	public void StartRecord()
	{
        GetComponent<AudioSource>().Stop();
        if (micArray.Length == 0)
        {
            Debug.Log("No Record Device!");
            return;
        }
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().mute = true;
        GetComponent<AudioSource>().clip = Microphone.Start(null, false, RECORD_TIME, 44100); //22050 
		while (!(Microphone.GetPosition(null)>0)) {
		}
		GetComponent<AudioSource>().Play ();
        Debug.Log("StartRecord");
        //倒计时
        StartCoroutine(TimeDown());
       
	}

	public  void StopRecord()
	{
        if (micArray.Length == 0)
        {
            Debug.Log("No Record Device!");
            return;
        }
        if (!Microphone.IsRecording(null))
        {
            return;
        }
		Microphone.End (null);
        GetComponent<AudioSource>().Stop();

        Debug.Log("StopRecord");
       // PlayRecord();

        //调试Int16[] 数据的转化与播放
        //PlayClipData(GetClipData());

	}

	public Byte[] GetClipData()
    {
        if (GetComponent<AudioSource>().clip == null)
        {
            Debug.Log("GetClipData audio.clip is null");
            return null; 
        }

        float[] samples = new float[GetComponent<AudioSource>().clip.samples];
        
        GetComponent<AudioSource>().clip.GetData(samples, 0);


		Byte[] outData = new byte[samples.Length * 2];
        //Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            short temshort = (short)(samples[i] * rescaleFactor);

			Byte[] temdata=System.BitConverter.GetBytes(temshort);

			outData[i*2]=temdata[0];
			outData[i*2+1]=temdata[1];


        }
		if (outData == null || outData.Length <= 0)
        {
            Debug.Log("GetClipData intData is null");
            return null; 
        }
        //return intData;
		return outData;
    }
    public void PlayClipData(Int16[] intArr)
    {

        string aaastr = intArr.ToString();
        long  aaalength=aaastr.Length;
        Debug.LogError("aaalength=" + aaalength);

		string aaastr1 = Convert.ToString (intArr);
		aaalength = aaastr1.Length;
		Debug.LogError("aaalength=" + aaalength);

        if (intArr.Length == 0)
        {
            Debug.Log("get intarr clipdata is null");
            return;
        }
        //从Int16[]到float[]
        float[] samples = new float[intArr.Length];
        int rescaleFactor = 32767;
        for (int i = 0; i < intArr.Length; i++)
        {
            samples[i] = (float)intArr[i] / rescaleFactor;
        }
        
        //从float[]到Clip
        AudioSource audioSource = this.GetComponent<AudioSource>();
        if (audioSource.clip == null)
        {
            audioSource.clip = AudioClip.Create("playRecordClip", intArr.Length, 1, 44100, false, false);
        }
        audioSource.clip.SetData(samples, 0);
        audioSource.mute = false;
        audioSource.Play();
    }
    public void ayRecord()
	{
        if (GetComponent<AudioSource>().clip == null)
        {
            Debug.Log("audio.clip=null");
            return;
        }
		GetComponent<AudioSource>().mute = false;
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play ();
        Debug.Log("PlayRecord");

	}

    public void LoadAndPlayRecord()
    {
//		string recordPath ="your path";

		//SavWav.LoadAndPlay (recordPath);
    }


	public  float GetAveragedVolume()
	{
		float[] data=new float[256];
		float a=0;
		GetComponent<AudioSource>().GetOutputData(data,0);
		foreach(float s in data)
		{
			a+=Mathf.Abs(s);
		}
		return a/256;
	}
	
	// Update is called once per frame
	void Update ()
    {
		loudness = GetAveragedVolume () * sensitivity;
		if (loudness > 1) 
		{
			Debug.Log("loudness = "+loudness);
		}
	}

    private IEnumerator TimeDown()
    {
        Debug.Log(" IEnumerator TimeDown()");

        int time = 0;
        while (time < RECORD_TIME)
        {
			if (!Microphone.IsRecording (null)) 
			{ //如果没有录制
				Debug.Log ("IsRecording false");
				yield break;
			}
            Debug.Log("yield return new WaitForSeconds "+time);
            yield return new WaitForSeconds(1);
            time++;
        }
        if (time >= 10)
        {
            Debug.Log("RECORD_TIME is out! stop record!");
            StopRecord();
        }
        yield return 0;
    }
}
