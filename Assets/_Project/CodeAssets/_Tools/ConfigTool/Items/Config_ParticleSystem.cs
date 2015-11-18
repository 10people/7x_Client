using UnityEngine;
using System.Collections;

public class Config_ParticleSystem {

	#region Mono

	public static void OnGUI(){
		OnGUI_Particles();
	}

	#endregion



	#region GUI Particles
	
	private static int m_particle_item_count = 3;
	
	private static string[] m_particle_item_names = { 
		"",
		"",
		"",
	};
	
	private static void OnGUI_Particles(){
		if( !ConfigTool.GetBool( ConfigTool.CONST_SHOW_PARTICLE_CONTROLLERS ) ){
			return;
		}
		
		Vector2 t_offset = new Vector2( Screen.width * 0.1f, Screen.height * 0.0f );
		
		float[] m_btn_enable_params = {
			Screen.width * 0.0f + t_offset.x, Screen.height * 0.05f + t_offset.y,
			Screen.width * 0.095f, Screen.height * 0.06f,
			0, Screen.height * 0.1f };
		
		float[] m_btn_disable_params = {
			m_btn_enable_params[ 0 ] + Screen.width * 0.1f, m_btn_enable_params[ 1 ],
			m_btn_enable_params[ 2 ], m_btn_enable_params[ 3 ],
			0, m_btn_enable_params[ 5 ], };
		
		float[] m_text_field_params = {
			Screen.width * 0.2f + t_offset.x, m_btn_enable_params[ 1 ] + t_offset.y,
			Screen.width * 0.2f, m_btn_enable_params[ 3 ],
			0, m_btn_enable_params[ 5 ] };
		
		int t_row = 0;
		
		// all
		{
			if( GUI.Button( GUIHelper.GetGUIRect( t_row, m_btn_enable_params ), "En All", GUIHelper.m_gui_btn_style ) ){
				//				Debug.Log( "Enable All." );
				
				DebugParticles( true, "" );
			}
			
			if( GUI.Button( GUIHelper.GetGUIRect( t_row, m_btn_disable_params ), "Dis All", GUIHelper.m_gui_btn_style ) ){
				//				Debug.Log( "Disable All." );
				
				DebugParticles( false, "" );
			}
			
			t_row++;
		}
		
		// sub
		{
			for( int i = 0; i < m_particle_item_count; i++ ){
				string t_particle_name = GUI.TextField( GUIHelper.GetGUIRect( t_row, m_text_field_params ), 
				                                       m_particle_item_names[ i ], 
				                                       GUIHelper.m_gui_text_field_style );
				
				m_particle_item_names[ i ] = t_particle_name;
				
				if( GUI.Button( GUIHelper.GetGUIRect( t_row, m_btn_enable_params ), "En", GUIHelper.m_gui_btn_style ) ){
					//					Debug.Log( "Enable: " + i + " - " + t_particle_name );
					
					DebugParticles( true, t_particle_name );
				}
				
				if( GUI.Button( GUIHelper.GetGUIRect( t_row, m_btn_disable_params ), "Dis", GUIHelper.m_gui_btn_style ) ){
					//					Debug.Log( "Disable: " + i + " - " + t_particle_name );
					
					DebugParticles( false, t_particle_name );
				}
				
				t_row++;
			}
		}
	}
	
	/// params:
	/// 
	/// p_particle_name: "" means all.
	private static void DebugParticles( bool p_enable, string p_particle_name ){
		Object[] t_ps_array = GameObject.FindObjectsOfType( typeof(ParticleSystem) );
		
		for( int i = 0; i < t_ps_array.Length; i++ ){
			ParticleSystem t_ps = (ParticleSystem)t_ps_array[ i ];
			
			if( string.IsNullOrEmpty( p_particle_name ) ){
				t_ps.GetComponent<Renderer>().enabled = p_enable;
				
				//				Debug.Log( "Set " + t_ps.gameObject.name + ": " + p_enable );
				
				continue;
			}
			
			if( t_ps.gameObject.name.ToLowerInvariant() == p_particle_name.ToLowerInvariant() ){
				t_ps.GetComponent<Renderer>().enabled = p_enable;
				
				//				Debug.Log( "Set " + t_ps.gameObject.name + ": " + p_enable );
			}
		}
		
		// debug
		//		{
		//			Object[] t_audio_listeners = GameObject.FindObjectsOfType( typeof(AudioListener) );
		//
		//			for( int i = 0; i < t_audio_listeners.Length; i++ ){
		//				AudioListener t_as = (AudioListener)t_audio_listeners[ i ];
		//
		//				Debug.Log( "Audio Listener " + i + ": " + t_as.gameObject.name );
		//			}
		//		}
	}
	
	#endregion
	
	
	
	#region Utilities



	#endregion
}
