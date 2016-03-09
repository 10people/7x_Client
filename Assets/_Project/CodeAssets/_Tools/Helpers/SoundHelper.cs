using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundHelper {

	#region Play Sound

	public static void PlayFxSound( GameObject p_gb, string p_fx_path ){
		EffectIdTemplate t_template = EffectIdTemplate.getEffectTemplateByEffectPath( p_fx_path, false );
		
		if( t_template == null ){
			return;
		}
		
		if( t_template.HaveSound() ){
			SoundPlayEff spe = p_gb.AddComponent<SoundPlayEff>();
			
			spe.PlaySound( t_template.sound );
		}
	}

	#endregion



	#region Audio Status

	public static bool HaveClip( AudioSource p_source ){
		if( p_source.clip != null ){
			return true;
		}

		return false;
	}

	#endregion



	#region Trace Sound

	public static void OnUpdate(){
		Debug.Log( "------------------ Trace Audio Source --------------------" );

		// clean null
		for( int i =  m_trace_source_list.Count - 1; i >= 0; i-- ){
			if( m_trace_source_list[ i ] == null ){
				m_trace_source_list.RemoveAt( i );
			}
		}

		for( int i =  m_trace_source_list.Count - 1; i >= 0; i-- ){
			if( IsRemovable ( m_trace_source_list[ i ] ) ){
				Debug.Log( "Remove Removable AudioSource: " + m_trace_source_list[ i ] );

				m_trace_source_list.RemoveAt( i );
			}
		}
	}

	private static List<AudioSource> m_trace_source_list = new List<AudioSource>();

	public static void TraceAudioSource( AudioSource p_source ){
		if( p_source == null ){
			Debug.Log( "Error, source is null: " + p_source );

			return;
		}

		Debug.Log( "Add Trace AudioSource: " + p_source );

		m_trace_source_list.Add( p_source );
	}

	public static bool IsRemovable( AudioSource p_source ){
		if( p_source.clip == null ){
			Debug.Log( "Clip is Null: " + p_source );

			return true;
		}

		if( !p_source.isPlaying ){
			Debug.Log( "Clip Is Not Playing: " + p_source.isPlaying );

			return true;
		}

		if( p_source.time >= p_source.clip.length ){
			Debug.Log( "Clip Played Done: " + p_source.time + " / " + p_source.clip.length );

			return true;
		}

		if( !p_source.gameObject.activeInHierarchy ){
			Debug.Log( "GameObject Deactivated." );

			return true;
		}

		if( !p_source.enabled ){
			Debug.Log( "Audio Disabled." );

			return true;
		}

		return false;
	}

	#endregion

}