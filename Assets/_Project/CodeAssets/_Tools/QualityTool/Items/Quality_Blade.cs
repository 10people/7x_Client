using UnityEngine;
using System.Collections;

public class Quality_Blade {

	private static Shader m_blade_effect = null;

	#region Blade Effect
	
	private static bool ShowCoolBlade(){
		#if UNITY_EDITOR
		return false;
		#elif UNITY_STANDALONE
		return true;
		#elif UNITY_ANDROID
		return QualityTool.GetBool( QualityTool.CONST_BLADE_EFFECT );
		#elif UNITY_IOS
		return iOS_ShowCoolBlade();
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return false;
		#endif
	}
	
	private static bool iOS_ShowCoolBlade(){
		if( !DeviceHelper.Is_iOS_Target_Device() ){
			return false;
		}

		#if UNITY_IOS
		return QualityTool.GetBool( QualityTool.CONST_BLADE_EFFECT );
		#else
		return false;
		#endif
	}
	
	public static void UpdateBladeEffect( GameObject p_gb ){
		if( !ShowCoolBlade() ){
			return;
		}
		
		//		Debug.Log( "Set New Blade Effect." );
		
		if( p_gb == null ){
			Debug.LogError( "Error, GameObject Is Null." );
			
			return;
		}
		
		ParticleSystem t_ps = p_gb.GetComponent<ParticleSystem>();
		
		if( t_ps == null ){
			Debug.LogError( "Error, GameObject.PS Is Null." );
			
			return;
		}
		
		Renderer t_renderer = t_ps.GetComponent<Renderer>();
		
		if( t_renderer == null ){
			Debug.LogError( "Error, GameObject.PS.Renderer Is Null." );
			
			return;
		}
		
		Material t_mat = t_renderer.sharedMaterial;
		
		if( t_mat == null ){
			Debug.LogError( "Error, GameObject.PS.Renderer.Mat Is Null." );
			
			return;
		}
		
		if( m_blade_effect == null ){
			m_blade_effect = Shader.Find( "Custom/Effects/Blade Effect" );
			
			if( m_blade_effect == null ){
				Debug.LogError( "Error, Blade Effect not found." );
				
				return;
			}
		}
		
		t_mat.shader = m_blade_effect;
	}
	
	#endregion
}
