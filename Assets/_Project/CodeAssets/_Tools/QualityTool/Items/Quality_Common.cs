using UnityEngine;
using System.Collections;

public class Quality_Common {

	private enum AA{
		None,
		Low,	// 2
		Medium,	// 4
		High,	// 8
	}

	#region AA
	
	public static void ConfigAA(){
		string t_aa = QualityTool.GetString( QualityTool.CONST_AA );
		
		if( string.IsNullOrEmpty( t_aa ) ){
			Debug.LogError( "Error, No AA Setted." );
			
			return;
		}
		
		switch( t_aa ){
		case "None":
			QualitySettings.antiAliasing = 0;
			
			break;
			
		case "Low":
			QualitySettings.antiAliasing = 2;
			
			break;
			
		case "Medium":
			QualitySettings.antiAliasing = 4;
			
			break;
			
		case "High":
			QualitySettings.antiAliasing = 8;
			
			break;
			
		default:
			Debug.LogError( "Error, No Value Exist." );
			
			return;
		}
		
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			Debug.Log( "AA: " + QualitySettings.antiAliasing );
		}
	}
	
	#endregion
	
	
	
	#region Bloom
	
	//	private static FastBloom m_fast_bloom = null;
	
	public static void ConfigBloom( bool p_enable_bloom ){
		#if UNITY_IOS && !ENABLE_BLOOM
		return;
		#endif
		
		#if UNITY_ANDROID
		return;
		#endif
		
		/*
		#if DEBUG_QUALITY
		Debug.Log( "ConfigBloom( " + p_enable_bloom + " )" );
		#endif

		FastBloom t_pre_bloom = null; 

		{
			Object[] t_objs = GameObject.FindObjectsOfType( typeof(FastBloom) );

			if( t_objs.Length > 1 || t_objs.Length < 0 ){
				Debug.LogError( "Error for Bloom Config." );

				return;
			}

			if( t_objs.Length == 1 ){
				t_pre_bloom = (FastBloom)t_objs[ 0 ];
			}
		}

		if( Camera.main == null ){
			Debug.LogError( "Error, No Main Camera Setted." );

//			#if UNITY_EDITOR
//			Debug.Log( "Scene: " + Application.loadedLevelName );
//
//			UnityEditor.EditorApplication.isPaused = true;
//			#endif

			return;
		}

		if( p_enable_bloom ){
			if( QualitySettings.antiAliasing != 0 ){
				QualitySettings.antiAliasing = 0;
			}
		}

		FastBloom t_bloom = Camera.main.GetComponent<FastBloom>();

		if( t_bloom == null ){
			t_bloom = Camera.main.gameObject.AddComponent<FastBloom>();
		}
		else{

		}
		
		if( t_bloom != m_fast_bloom ){
			m_fast_bloom = t_bloom;
		}

//		Shader t_shader = Shader.Find( "Custom/Effects/MobileBloom" );

		Shader t_shader = Shader.Find( "Hidden/FastBloom" );

		if( t_shader != null ){
			t_bloom.fastBloomShader = t_shader;
		}
		else{
			Debug.LogError( "Error, Bloom Shader Not Found." );
		}

		if( m_fast_bloom != null ){
			m_fast_bloom.enabled = p_enable_bloom;
		}
		else{
			Debug.LogError( "Error, Bloom Not Found." );
		}

		if( t_pre_bloom != null && m_fast_bloom != null ){
			m_fast_bloom.threshhold = t_pre_bloom.threshhold;

			m_fast_bloom.intensity = t_pre_bloom.intensity;

			m_fast_bloom.blurSize = t_pre_bloom.blurSize;

			m_fast_bloom.blurIterations = t_pre_bloom.blurIterations;
		}
		else{
			m_fast_bloom.threshhold = 0.3f;
			
			m_fast_bloom.intensity = 0.5f;
			
			m_fast_bloom.blurSize = 0.5f;
			
			m_fast_bloom.blurIterations = 1;
		}

		if( !p_enable_bloom ){
//			Debug.Log( "Destroy.FB." );

			Destroy( t_bloom );
		}

		#if DEBUG_QUALITY
		Debug.Log( "ConfigBloom Done." );
		#endif

		*/
	}
	
	#endregion
	
	
	

}
