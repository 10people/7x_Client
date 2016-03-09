using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundPlayYuyin : MonoBehaviour 
{
	public AudioSource m_AudioSource;
	SoundManager.shoudId m_SoundData;
	private int m_iCurPlayID = -1;
	public bool m_isPlay = false;//是否播放语音中
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_isPlay)
		{
			if(!m_AudioSource.isPlaying)
			{
				SoundManager.EYuyin();
				m_isPlay = false;
			}
		}
	}
	
	public void PlaySound(string path)
	{
		if (path == null || path.Length == 0) return;
		
		//Debug.Log( "PlaySound: " + path + " )" );
		
		//		int indexRan;
		string tempSId;
		if(path.IndexOf("|") != -1){
			string[] tempPath = path.Split('|');
			tempSId = tempPath[Global.getRandom(tempPath.Length)];
		}
		else{
			tempSId = path;
		}
		
		AudioClip tempClip;
		try{
			int tempId = int.Parse(tempSId);
			if(tempId <= 0)
			{
				return;
			}
			if(m_AudioSource == null || (tempId != -1 && (m_iCurPlayID != tempId || !m_AudioSource.isPlaying)))
			{
				if(ClientMain.m_sound_manager == null)
				{
					return;
				}
				m_iCurPlayID = tempId;
				m_SoundData = SoundManager.GetSoundInfo(tempId);
				ClientMain.m_sound_manager.getEffIdClip( tempId, ResourceLoadCallback );
			}
		}
		catch{
			//			tempClip = ClientMain.M_SOUNDMANAGER.getPathClip(tempSId);
			
			Debug.LogError( "Never Shoud Be Here" );
			
			return;
		}
	}
	
	public void StopSound()
	{
		AudioSource[] tempAudio = gameObject.GetComponents<AudioSource>();
		
		for(int i = 0; i < tempAudio.Length; i ++)
		{
			tempAudio[i].Stop();
		}
	}
	
	public void ResourceLoadCallback( ref WWW p_www, string p_path, Object p_object)
	{
		ClientMain.m_sound_manager.getIdClipResLoad( ref p_www, p_path, p_object );
		if(m_AudioSource == null)
		{
			m_AudioSource = ClientMain.m_ClientMainObj.AddComponent<AudioSource>();
		}
		m_AudioSource.rolloffMode = AudioRolloffMode.Linear;
		m_AudioSource.clip = (AudioClip)p_object;
		m_AudioSource.maxDistance = 50;
		m_AudioSource.volume = ClientMain.m_sound_manager.m_fMaxEffVolume;
		m_AudioSource.Play();
		m_AudioSource.playOnAwake = false;

		if(m_SoundData.iLoop == 1)
		{
			m_AudioSource.loop = true;
		}
		
		m_AudioSource.spatialBlend = m_SoundData.iStereo;
		m_isPlay = true;
		SoundManager.BYuyin();
	}
	
	#region Utilities
	
	
	
	// select target sound id from random
	public static int GetTargetSoundId( string p_config_path ){
		if ( p_config_path == null || p_config_path.Length == 0) 
			return GetSoundNullId();
		
		//		int indexRan;
		string tempSId;
		
		if( p_config_path.IndexOf("|") != -1 ){
			string[] tempPath = p_config_path.Split('|');
			
			tempSId = tempPath[ Global.getRandom(tempPath.Length) ];
		}
		else{
			tempSId = p_config_path;
		}
		
		return int.Parse( tempSId );
	}
	
	public static int GetSoundNullId(){
		return -1;
	}
	
	#endregion
}
