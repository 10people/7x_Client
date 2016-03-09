using UnityEngine;
using System.Collections;

public class DevelopUtility : MonoBehaviour {

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Utilites

	public static void MakeSureObjectExist( Object p_gb, string p_log_if_error ){
		if( p_gb == null ){
			Debug.LogError( "Object is Null: " + p_log_if_error );
		}
	}

	/// Play Sound On p_gb With Sound_Id in "SoundId.xml".
	public static void PlaySound( int p_sound_id, GameObject p_gb ){
//		Debug.Log( "PlaySound( " + p_sound_id + " on " + p_gb + " )" );

		AudioClip t_sound_clip = ( AudioClip )Resources.Load( SoundManager.GetSoundInfo( p_sound_id ).sPath );

		AudioSource[] t_sources = p_gb.GetComponents<AudioSource>();

		AudioSource t_target_source = null;

		for( int i = 0; i < t_sources.Length; i++ ){
			AudioSource t_source = t_sources[ i ];

			if( !t_source.isPlaying ){
//				Debug.Log( "Find Stopped Source." );

				t_target_source = t_source;

				break;
			}
		}

		if( t_target_source == null ){
//			Debug.Log( "Create New Source." );

			t_target_source = p_gb.AddComponent<AudioSource>();
		}
		
		t_target_source.rolloffMode = AudioRolloffMode.Linear;
		
		t_target_source.clip = (AudioClip)t_sound_clip;
		
		t_target_source.Play();
	}

	#endregion

	public class DevelopFxTarget{
		public GameObject m_fx_target;
		
		public float m_create_time;
		
		public DevelopFxTarget( GameObject p_gb ){
			m_fx_target = p_gb;
			
			m_create_time = Time.realtimeSinceStartup;
		}
		
		public bool IsDone( float p_time_out ){
			if( Time.realtimeSinceStartup - m_create_time > p_time_out ){
				return true;
			}
			
			//			Debug.Log( m_fx_target.name + ": " + ( Time.realtimeSinceStartup - m_create_time ) );
			
			return false;
		}
		
		public void FxDone(){
			if( m_fx_target == null ){
				return;
			}
			
			m_fx_target.gameObject.SetActive( false );
			
			Destroy( m_fx_target );
		}
	}


}
