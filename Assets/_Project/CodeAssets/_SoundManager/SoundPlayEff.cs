using UnityEngine;
using System.Collections;

public class SoundPlayEff : MonoBehaviour 
{
	public AudioSource m_AudioSource;
	SoundManager.shoudId m_SoundData;

	void Awake(){
		
	}

	// Use this for initialization
	void Start () 
	{
//		AudioSource m_AudioSource = gameObject.AddComponent<AudioSource>();
//		m_AudioSource.rolloffMode = AudioRolloffMode.Linear;
//		m_AudioSource.maxDistance = 50;
//		m_AudioSource.Play();
//		gameObject.AddComponent<AudioSource>();
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
			if(tempId != -1)
			{
				if(ClientMain.m_sound_manager == null)
				{
					return;
				}
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
//			Debug.Log( "Sound Stop." );

			tempAudio[i].Stop();
		}
	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path, Object p_object){
		if( this == null ){
			return;
		}

	    if (p_object == null || gameObject == null)
	    {
	        return;
	    }

	    ClientMain.m_sound_manager.getIdClipResLoad( ref p_www, p_path, p_object );
		int tempIndex = -1;
		
		AudioSource[] tempAudio = gameObject.GetComponents<AudioSource>();
		
		for(int i = 0; i < tempAudio.Length; i ++){
			if(!tempAudio[i].isPlaying)
			{
				tempIndex = i;
				break;
			}
		}

		if(tempIndex == -1){
			AudioSource m_AudioSource = gameObject.AddComponent<AudioSource>();

			m_AudioSource.rolloffMode = AudioRolloffMode.Linear;

			m_AudioSource.maxDistance = 20;

			if(ClientMain.m_ClientMain.m_SoundPlayEff.m_isPlay){
				m_AudioSource.volume = ClientMain.m_sound_manager.m_fMaxEffVolume / 2;
			}
			else{
				m_AudioSource.volume = ClientMain.m_sound_manager.m_fMaxEffVolume;
			}

			m_AudioSource.playOnAwake = false;

			if(m_SoundData.iLoop == 1)
			{
				m_AudioSource.loop = true;
			}

			m_AudioSource.spatialBlend = m_SoundData.iStereo;

			m_AudioSource.clip = (AudioClip)p_object;

			SoundManager.addEffSound(m_AudioSource);
			if(m_AudioSource.gameObject.activeSelf)
			{
				m_AudioSource.Play();
			}
		}
		else{
			tempAudio[tempIndex].rolloffMode = AudioRolloffMode.Linear;

			tempAudio[tempIndex].maxDistance = 20;

			if(ClientMain.m_ClientMain.m_SoundPlayEff.m_isPlay){
				tempAudio[tempIndex].volume = ClientMain.m_sound_manager.m_fMaxEffVolume / 2;
			}
			else{
				tempAudio[tempIndex].volume = ClientMain.m_sound_manager.m_fMaxEffVolume;
			}

			if(m_SoundData.iLoop == 1){
				tempAudio[tempIndex].loop = true;
			}

			tempAudio[tempIndex].spatialBlend = m_SoundData.iStereo;

			tempAudio[tempIndex].clip = (AudioClip)p_object;
			if(tempAudio[tempIndex].gameObject.activeSelf)
			{
				tempAudio[tempIndex].Play();
			}
		}
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
