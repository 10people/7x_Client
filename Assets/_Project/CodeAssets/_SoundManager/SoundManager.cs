using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class SoundManager{
	public static string M_SOUNDPATH = "Sound/";

	// cur music
	private int m_iCurMusicIndex = 0;
	
	/// The list music, only two bgm.
	public List<AudioSource> m_ListMusic = new List<AudioSource>();
	
	public Dictionary< int, AudioClip > m_sound_dict = new Dictionary< int, AudioClip >();


	// max volume
	public float m_fMaxVolume = 1.0f;

	public float m_fMaxEffVolume = 1.0f;

	// tween music speed
	public float m_fReduceSpeed = 0.10f;

	// reduce music volume to X, increase scene music
	private float m_fChangeVolumeNum = 0.6f;

	// wheteher is tweening music
	private bool m_isChange = false;

	// whether new bg music could increase now
	private bool m_isChange1Over = false;

	private bool m_isSoundStop = false;

	private bool m_isSoundPlay = false;

	public struct shoudId{
		public int iId;

		public string sPath;

		public int iLoop;

		public int iStereo;
	}

	public static List<shoudId> m_listShoudID = new List<shoudId>();
	
	public static List<AudioSource> m_listSource = new List<AudioSource>();
	public static List<AudioSource> m_listSourceEff = new List<AudioSource>();

	public List<float> m_listPlayTime = new List<float>();

	public List<int> m_listPlayerID = new List<int>();

	public List<int> m_listPlayerNextID = new List<int>();
	public List<Bundle_Loader.LoadResourceDone> m_listPlayerNextDone = new List<Bundle_Loader.LoadResourceDone>();
	public List<List<EventDelegate>> m_listPlayerNextEvent = new List<List<EventDelegate>>();

	public SoundManager(AudioSource sound1, AudioSource sound2){
		{
			m_listSource.Clear();
		}

		m_ListMusic.Add(sound1);

		m_ListMusic.Add(sound2);

		if(m_ListMusic[1] != null)
		{
			if(m_ListMusic[1].clip != null)
			{
				m_ListMusic[1].volume = 0;
			}
		}
	}

	/// Game Use.
	public void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad( PathManager.GetUrl(XmlLoadManager.m_LoadPath + "SoundId.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	public void updata(){
		if(m_isChange)
		{
			bool tempBSound1 = false;
			bool tempBSound2 = false;
			m_ListMusic[m_iCurMusicIndex].volume -= m_fReduceSpeed;
			if(m_ListMusic[m_iCurMusicIndex].volume <= m_fChangeVolumeNum)
			{
				m_isChange1Over = true;
			}
				
			if(m_ListMusic[m_iCurMusicIndex].volume <= 0)
			{
				m_ListMusic[m_iCurMusicIndex].volume = 0;
				tempBSound1 = true;
			}
			if(m_isChange1Over)
			{
				m_ListMusic[m_iCurMusicIndex ^ 1].volume += m_fReduceSpeed;
				if(m_ListMusic[m_iCurMusicIndex ^ 1].volume >= m_fMaxVolume)
				{
					m_ListMusic[m_iCurMusicIndex ^ 1].volume = m_fMaxVolume;
					tempBSound2 = true;
				}
			}

			if(tempBSound1 & tempBSound2)
			{
				RemoveSound(m_ListMusic[m_iCurMusicIndex].clip);
				m_ListMusic[m_iCurMusicIndex].clip = null;

				m_isChange = false;
				m_isChange1Over = false;
				m_iCurMusicIndex = (m_iCurMusicIndex ^ 1);
			}
		}
		if(m_isSoundStop)
		{
			m_ListMusic[m_iCurMusicIndex].volume -= m_fReduceSpeed;
			if(m_ListMusic[m_iCurMusicIndex].volume <= 0)
			{
				m_isSoundStop = false;
				m_ListMusic[m_iCurMusicIndex].volume = 0;
				m_ListMusic[m_iCurMusicIndex].Stop();
			}
		}
		if(m_isSoundPlay)
		{
			m_ListMusic[m_iCurMusicIndex].volume += m_fReduceSpeed;
			if(m_ListMusic[m_iCurMusicIndex].volume >= m_fMaxVolume)
			{
				m_isSoundPlay = false;
				m_ListMusic[m_iCurMusicIndex].volume = m_fMaxVolume;
			}
		}
		for(int i = 0; i < m_listPlayerNextID.Count; i ++)
		{
			if(getEffIdClip(m_listPlayerNextID[i], m_listPlayerNextDone[i], m_listPlayerNextEvent[i], false))
			{
				m_listPlayerNextID.RemoveAt(i);
				m_listPlayerNextDone.RemoveAt(i);
				m_listPlayerNextEvent.RemoveAt(i);
				i--;
			}
		}
	}

	// set background music
	public void setBGSound(int id, List<EventDelegate> p_callback_list = null ){
//		Debug.Log( "setBGSound: " + id );
		getIdClip( id, setBGSoundResLoadCallback, p_callback_list );
	}

	public void setBGSoundResLoadCallback( ref WWW p_www, string p_path, Object p_object ){
		getIdClipResLoad( ref p_www, p_path, p_object );

		m_ListMusic[m_iCurMusicIndex].clip = (AudioClip)p_object;

		m_ListMusic[m_iCurMusicIndex].volume = m_fMaxVolume;

		m_ListMusic[m_iCurMusicIndex].Play();
	}

	public void chagneBGSound(int id){
		getIdClip( id, chagneBGSoundResLoadCallback );
	}

	public void chagneBGSoundResLoadCallback( ref WWW p_www, string p_path, Object p_object ){
		getIdClipResLoad( ref p_www, p_path, p_object );

		m_ListMusic[m_iCurMusicIndex ^ 1].clip = (AudioClip)p_object;

		m_ListMusic[m_iCurMusicIndex ^ 1].volume = 0f;

		m_ListMusic[m_iCurMusicIndex ^ 1].Play();

		m_isChange = true;
	}

	public void shopBGSound()
	{
		m_isSoundStop = true;
		m_isSoundPlay = false;
	}

	public void playBGSound()
	{
		m_isSoundStop = false;
		m_isSoundPlay = true;
		m_ListMusic[m_iCurMusicIndex].Play();
	}

	public void setMaxVolume(float Volume)
	{
		m_fMaxVolume = Volume;

		m_ListMusic[m_iCurMusicIndex].volume = m_fMaxVolume;
	
//		Debug.Log("m_fMaxVolume="+m_fMaxVolume);

//		for(int i = 0; i < 2; i ++)
//		{
//			if(m_ListMusic[m_iCurMusicIndex ^ 1].clip != null)
//			{
//				m_ListMusic[m_iCurMusicIndex ^ 1].volume = m_fMaxVolume;
//			}
//		}
	}

	public void setMaxEffVolume(float Volume)
	{
		m_fMaxEffVolume = Volume;
		NGUITools.soundVolume = Volume;
		for(int i = 0; i < m_listSource.Count; i ++)
		{
			if(m_listSource[i] == null)
			{
				m_listSource.RemoveAt(i);
				i --;
				continue;
			}
			else
			{
				m_listSource[i].volume = m_fMaxEffVolume;
			}
		}
	}

	#region Pure Data Load

	public static shoudId GetSoundInfo( int p_sound_id ){
		for( int i = 0; i < m_listShoudID.Count; i++ ){
			shoudId t_info = m_listShoudID[ i ];

			if( t_info.iId == p_sound_id ){
				return t_info;
			}
		}

		Debug.LogError( "Error In Getting Sound Info With Id: " + p_sound_id );

		shoudId t_empty;

		t_empty.iId  = -1;

		t_empty.sPath = "";

		t_empty.iLoop = 1;

		t_empty.iStereo = 1;

		return t_empty;
	}

	/// Only Load.
	public static void PureLoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad( PathManager.GetUrl(XmlLoadManager.m_LoadPath + "SoundId.xml"), PureLoadCallback, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	public static void PureLoadCallback( ref WWW www, string path, Object obj ){
		{
			m_listShoudID.Clear();
		}
		
		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "SoundList" );
			
			if( !t_has_items ){
				break;
			}
			
			{
				shoudId tempSoundID;
				
				t_reader.MoveToNextAttribute();
				tempSoundID.iId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				// name
				
				t_reader.MoveToNextAttribute();
				tempSoundID.sPath = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				tempSoundID.iLoop = int.Parse(t_reader.Value);
				// loops
				
				t_reader.MoveToNextAttribute();
				// type
				
				t_reader.MoveToNextAttribute();
				tempSoundID.iStereo = int.Parse(t_reader.Value);
				// stereo
				
				m_listShoudID.Add( tempSoundID );
			}
		}
		while( t_has_items );
	}

	#endregion

	public void CurLoad( ref WWW www, string path, Object obj ){
		PureLoadCallback( ref www, path, obj );

		setBGSound( 1000, UtilityTool.GetEventDelegateList( ClientMain.LoadTemplates ) );
	}

	#region Clip Load & Unload

	/// MUST MANUALLY CALL getIdClipResLoad
	public void getIdClip( int p_id, Bundle_Loader.LoadResourceDone p_callback, List<EventDelegate> p_callback_list = null ){
//		Debug.Log( "SoundManager.GetIdClip: " + p_id );

		string t_sound_path = "";

		for(int i = 0; i < m_listShoudID.Count; i ++){
			if( p_id == m_listShoudID[i].iId ){
				t_sound_path = m_listShoudID[i].sPath;
				if( m_sound_dict.ContainsKey( p_id ) ){
					WWW t_www = null;

					AudioClip t_clip = m_sound_dict[ p_id ];

					if( p_callback != null ){
						p_callback( ref t_www, t_sound_path, t_clip );
					}

					EventDelegate.Execute( p_callback_list );

					return;
				}
				else{
					Global.ResourcesDotLoad( t_sound_path,
					                        p_callback,
					                        p_callback_list );

					return;
				}
			}
		}

		Debug.LogError( "Sound Not Exist in SoundId.xml: " + p_id );

		Debug.LogError( "Sound Not Exist in SoundId.xml: " + t_sound_path );
	}

	public bool getEffIdClip(int p_id, Bundle_Loader.LoadResourceDone p_callback, List<EventDelegate> p_callback_list = null, bool isAdd = true)
	{
//		if((p_id >= 300001 && p_id <= 300008) || (p_id >= 3309 && p_id <= 3316))
//		{
//			Debug.Log("==========================" + p_id);
//		}
		float thisTime = Time.time;

		for(int i = 0; i < m_listPlayTime.Count; i ++)
		{
			if(thisTime - m_listPlayTime[i] > SoundIdGroup.getSoundGroup(m_listPlayerID[i]).allNumTime)
			{
				m_listPlayTime.RemoveAt(i);
				m_listPlayerID.RemoveAt(i);
				i --;
				continue;
			}
		}

		for(int i = 0; i < SoundIdGroup.m_ListSoundGroup.Count; i ++)
		{
			if(SoundIdGroup.m_ListSoundGroup[i].SoundID.Contains(p_id))
			{
				int tempNum = 0;
				for(int q = 0; q < SoundIdGroup.m_ListSoundGroup[i].SoundID.Count; q ++)
				{
					if(m_listPlayerID.Contains(SoundIdGroup.m_ListSoundGroup[i].SoundID[q]))
					{
//						if((p_id >= 300001 && p_id <= 300008) || (p_id >= 3309 && p_id <= 3316))
//						{
//							Debug.Log("??????ID="+SoundIdGroup.m_ListSoundGroup[i].SoundID[q]);
//						}
						tempNum++;
					}
				}
//				if((p_id >= 300001 && p_id <= 300008) || (p_id >= 3309 && p_id <= 3316))
//				{
//					Debug.Log("??????????????????="+tempNum);
//				}
				if(tempNum >= SoundIdGroup.m_ListSoundGroup[i].playNum)
				{
//					if((p_id >= 300001 && p_id <= 300008) || (p_id >= 3309 && p_id <= 3316))
//					{
//						Debug.Log("?????????????????????????????????);
//					}
					return false;
				}
				else if(thisTime - SoundIdGroup.m_ListSoundGroup[i].pTime < SoundIdGroup.m_ListSoundGroup[i].nextTime)
				{
					if(isAdd)
					{
						m_listPlayerNextID.Add(p_id);
						m_listPlayerNextDone.Add(p_callback);
						m_listPlayerNextEvent.Add(p_callback_list);
					}
					return false;
				}
				else
				{
					SoundIdGroup.SoundGroup tempa = SoundIdGroup.m_ListSoundGroup[i];
					tempa.pTime = thisTime;
					SoundIdGroup.m_ListSoundGroup[i] = tempa;
				}
			}
		}
		m_listPlayerID.Add(p_id);
		m_listPlayTime.Add(thisTime);
//		if((p_id >= 300001 && p_id <= 300008) || (p_id >= 3309 && p_id <= 3316))
//		{
//			Debug.Log("????????????????????????");
//		}
		getIdClip(p_id, p_callback, p_callback_list);
		return true;
	}

	// add to dict.
	// right now, we could just call this manually.
	public void getIdClipResLoad( ref WWW p_www, string p_path, Object p_object ){
		AudioClip t_clip = ( AudioClip )p_object;

		for(int i = 0; i < m_listShoudID.Count; i ++){
			if( p_path == m_listShoudID[ i ].sPath ){
				if( !m_sound_dict.ContainsKey( m_listShoudID[i].iId ) ){
					LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections,
					                         PrepareForBattleField.CONST_BATTLE_LOADING_SOUND, p_path );

					m_sound_dict[ m_listShoudID[i].iId ] = t_clip;
				}
			}
		}
	}

	public void RemoveAllSound(){
//		Debug.Log( "SoundManager.RemoveAllSound()" );

		m_sound_dict.Clear();
	}

	// Remove From m_sound_dict
	public void RemoveSound( int p_id ){
		if( m_sound_dict.ContainsKey( p_id ) ){
//			Debug.Log( p_id + " Removed From SoundDict." );

			m_sound_dict.Remove( p_id );
		}
	}

	public void RemoveSound( string p_sound_name ){
		if( string.IsNullOrEmpty( p_sound_name ) ){
			return;
		}

		int t_id = -1;

		string t_target_name = PathHelper.GetFileNameFromPath( p_sound_name ).ToLowerInvariant();

		for(int i = 0; i < m_listShoudID.Count; i ++){
			shoudId t_sound_id = m_listShoudID[ i ];

			string t_name = PathHelper.GetFileNameFromPath( t_sound_id.sPath ).ToLowerInvariant();

			if( t_target_name == t_name ){
				t_id = t_sound_id.iId;

				break;
			}
		}

		if( t_id >= 0 ){
			RemoveSound( t_id );
		}
	}

	public void RemoveSound( AudioSource p_audio_source ){
		if( p_audio_source == null ){
			return;
		}

		int t_id = 0;

		AudioClip t_clip = p_audio_source.clip;

		RemoveSound( t_clip );
	}

	public void RemoveSound( AudioClip p_audio_clip ){
		if( p_audio_clip == null ){
			return;
		}

		RemoveSound( p_audio_clip.name );
	}

	#endregion

	public static void getAudioSource( GameObject obj ){
//		Debug.Log( "Root: " + obj.name );
//
//		Debug.Log( "Root: " + obj.transform.parent );

//		Transform[] temp = GameObject.FindObjectsOfType( typeof( Transform ) ) as Transform[];

		AudioSource[] temp = obj.GetComponentsInChildren<AudioSource>();
//	
//		Debug.Log("temp.Length=temp.Lengthtemp.Lengthtemp.Lengthtemp.Lengthtemp.Length="+temp.Length);

//		for(int i = 0 ; i < temp.Length; i ++)
//		{
//			if( temp[i].gameObject.name == "Fire" ){
//				Debug.Log("i=" + i + ":name=" + temp[i].name);
//
//				Debug.Log( "GB:" +temp[ i ].gameObject );
//
//				AudioSource t_src = temp[ i ].gameObject.GetComponent<AudioSource>();
//				Debug.Log( "src: " + t_src );
//			}
//		}

//		for(int i = 0 ; i < temp.Length; i ++)
//		{
//			Debug.Log("i=" + i + ":name=" + temp[i].name);
//		}
//		Debug.Log(temp[302].name);
//		Debug.Log(temp[302].gameObject.GetComponent<AudioSource>());
//		Debug.Log(temp[302].gameObject.activeSelf);
//		Debug.Log(temp[302].gameObject.GetComponent<BoxCollider>());

		for(int i = 0; i < temp.Length; i ++)
		{
			m_listSource.Add(temp[i]);
		}

		for(int i = 0; i < m_listSource.Count; i ++)
		{
			if(m_listSource[i] == null)
			{
				m_listSource.RemoveAt(i);
				i --;
				continue;
			}
			else
			{
				m_listSource[i].volume = ClientMain.m_sound_manager.m_fMaxEffVolume;
			}
		}
//		m_listSource
	}

	public static void addEffSound(AudioSource audioSource)
	{
		m_listSourceEff.Add(audioSource);

	}

	public static void BYuyin()
	{
		ClientMain.m_sound_manager.m_ListMusic[ClientMain.m_sound_manager.m_iCurMusicIndex].volume = ClientMain.m_sound_manager.m_fMaxVolume / 4;

		for(int i = 0; i < m_listSourceEff.Count; i ++)
		{
			if(m_listSourceEff[i] == null)
			{
				m_listSourceEff.RemoveAt(i);
				i --;
				continue;
			}
			else
			{
				m_listSourceEff[i].volume = ClientMain.m_sound_manager.m_fMaxEffVolume / 4;
			}
		}
	}

	public static void EYuyin()
	{
		if(!ClientMain.m_ClientMain.m_SoundPlayEff.m_isPlay && !QXChatPage.chatPage.m_isPlaying)
		{
			ClientMain.m_sound_manager.m_ListMusic[ClientMain.m_sound_manager.m_iCurMusicIndex].volume = ClientMain.m_sound_manager.m_fMaxVolume * 4;
			for(int i = 0; i < m_listSourceEff.Count; i ++)
			{
				if(m_listSourceEff[i] != null)
				{
					m_listSourceEff[i].volume = ClientMain.m_sound_manager.m_fMaxEffVolume * 4;
				}
			}
		}
	}
}