//#define DEBUG_FX_SOUND

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class DevelopFxSound : MonoBehaviour {

	public int m_target_fx_id = -1;

	public string m_target_fx_path = "";

	public string m_target_fx_sound = "";

	public float m_target_fx_position = 0.0f;



	// auto get info
	private int m_cached_fx_id = -1;

	// auto save
	private float m_cached_fx_position = 0.0f;



	private List<DevelopUtility.DevelopFxTarget> m_fx_info_list = new List<DevelopUtility.DevelopFxTarget>();

	public float m_fx_time_out	= 10.0f;



	private static DevelopFxSound m_instance = null;

	#region Mono

	void Awake(){
		{
			m_instance = this;
		}

		LoadFiles();
	}

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		TryGetInfo();

		UpdateFxInfo();

		UpdateFxTargets();
	}

	void OnDestroy(){
		m_instance = null;
	}

	#endregion



	#region Develop Fx

	void UpdateFxTargets(){
		for( int i = m_fx_info_list.Count - 1; i >= 0; i-- ){
			DevelopUtility.DevelopFxTarget t_target = m_fx_info_list[ i ];

			if( t_target.IsDone( DevelopFxSound.m_instance.m_fx_time_out ) ){
				t_target.FxDone();

				m_fx_info_list.Remove( t_target );
			}
		}
	}

	private void TryGetInfo(){
		if( m_cached_fx_id == m_target_fx_id ){
			return;
		}

		EffectIdTemplate t_effect_id_template = EffectIdTemplate.getEffectTemplateByEffectId( m_target_fx_id, false );

		if( t_effect_id_template == null ){
			return;
		}

		GetFxInfo();
	}

	public void GetFxInfo(){
		#if DEBUG_FX_SOUND
		Debug.Log( "GetFxInfo( " + m_target_fx_id + " )" );
		#endif

		EffectIdTemplate t_effect_id_template = EffectIdTemplate.getEffectTemplateByEffectId( m_target_fx_id );
		
		if( t_effect_id_template == null ){
			Debug.LogError( "Error In Getting Info." );
			
			return;
		}
		
		{
			m_target_fx_id = t_effect_id_template.effectId;
			
			m_target_fx_path = t_effect_id_template.path;
			
			m_target_fx_sound = t_effect_id_template.sound;
			
			//		m_target_fx_position = t_effect_id_template.;
		}
		
		{
			m_cached_fx_id = m_target_fx_id;
			
			//		m_cached_fx_position = ;
		}
	}

	public void PlayFx(){
		#if DEBUG_FX_SOUND
		Debug.Log( "PlayFx( " + m_target_fx_id + " )" );
		#endif

		EffectIdTemplate t_effect_id_template = EffectIdTemplate.getEffectTemplateByEffectId( m_target_fx_id );

		if( t_effect_id_template == null ){
			Debug.LogError( "Error In Getting Info." );

			return;
		}

		Debug.Log( "Playing " + t_effect_id_template.effectId + " - " + t_effect_id_template.path + " - " + t_effect_id_template.sound );

		GameObject t_gb = ( GameObject )Instantiate( Resources.Load( t_effect_id_template.path ) );

		PlaySound( t_effect_id_template.sound, t_gb );

		{
			DevelopUtility.DevelopFxTarget t_info = new DevelopUtility.DevelopFxTarget( t_gb );
			
			m_fx_info_list.Add( t_info );
		}
	}

	private void PlaySound( string p_config_path, GameObject p_gb ){
		int t_target_sound_id = SoundPlayEff.GetTargetSoundId( p_config_path );

		if( t_target_sound_id == SoundPlayEff.GetSoundNullId() ){
			return;
		}

		#if DEBUG_FX_SOUND
//		Debug.Log( "PlaySound( " + t_target_sound_id + " in " + p_config_path + " )" );
		#endif

		DevelopUtility.PlaySound( t_target_sound_id, p_gb );
	}
	
	#endregion



	#region File

	private void LoadFiles(){
		EffectIdTemplate.LoadTemplates();

		SoundManager.PureLoadTemplates();
	}

	public void SaveFiles(){
		#if DEBUG_FX_SOUND
		Debug.Log( "SaveFiles()" );
		#endif

		string t_text = EffectIdTemplate.ComposeXmlText();

		#if DEBUG_FX_SOUND
		Debug.Log( "Text: " + t_text );
		#endif

		string t_config_path_name = PathHelper.GetFullPath_WithRelativePath( "Resources/" + XmlLoadManager.m_LoadPath + "EffectId.xml" );

		FileStream t_file_stream = null;

		if( File.Exists( t_config_path_name ) ){
			#if DEBUG_FX_SOUND
			Debug.Log( "Tuncate File." );
			#endif

			t_file_stream = new FileStream( t_config_path_name, FileMode.Truncate );
		}
		else{
			#if DEBUG_FX_SOUND
			Debug.Log( "Create File." );
			#endif

			t_file_stream = new FileStream( t_config_path_name, FileMode.Create );
		}
		
		StreamWriter t_stream_writer = new StreamWriter(
			t_file_stream,
			Encoding.Default );
		
		t_stream_writer.Write( t_text );
		
		t_stream_writer.Close();
		
		t_file_stream.Close();
	}

	#endregion



	#region Utilities

	/// Check Info Changes.
	private void UpdateFxInfo(){
		bool m_changed = false;

		if( m_cached_fx_position != m_target_fx_position ){
			m_changed = true;
		}

		if( m_changed ){
			UpdateCachedInfo();
		}
	}

	/// Update Template Info.
	private void UpdateCachedInfo(){
		#if DEBUG_FX_SOUND
		Debug.Log( "UpdateCachedInfo()" );
		#endif

		{
			m_cached_fx_position = m_target_fx_position;
		}

		EffectIdTemplate t_effect_id_template = EffectIdTemplate.getEffectTemplateByEffectId( m_target_fx_id );

//		t_effect_id_template.
	}

	#endregion


}
