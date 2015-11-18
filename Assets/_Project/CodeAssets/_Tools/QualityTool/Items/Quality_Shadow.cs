using UnityEngine;
using System.Collections;

public class Quality_Shadow {

	#region InCity Shadow
	
	/// Show simple plane shadow incity or not.
	/// 
	/// Notes:
	/// 1.if not showed, never create the shadow.
	public static bool InCity_ShowSimpleShadow(){
		bool t_show_simple_shadow = true;

		#if UNITY_EDITOR && !IGNORE_EDITOR
		t_show_simple_shadow = true;
		#elif UNITY_STANDALONE
		t_show_simple_shadow = true;
		#elif UNITY_ANDROID
		return !QualityTool.GetBool( QualityTool.CONST_IN_CITY_SHADOW );
		#elif UNITY_IOS
		return InCity_iOS_ShowSimpleShadow();
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return true;
		#endif
		
		#if SHOW_REAL_SHADOW
		t_show_simple_shadow = false;
		#endif
		
		return t_show_simple_shadow;
	}
	
	private static bool InCity_iOS_ShowSimpleShadow(){
		if( !DeviceHelper.Is_iOS_Target_Device() ){
			return true;
		}
		
		#if UNITY_IOS
		return !QualityTool.GetBool( QualityTool.CONST_IN_CITY_SHADOW );
		#endif
		
		return false;
	}
	
	#endregion
	
	
	
	#region BattleField Shadow
	
	/// Show simple plane shadow in battle field or not.
	/// 
	/// Notes:
	/// 1.if not showed, never create the shadow.
	public static bool BattleField_ShowSimpleShadow(){
		bool t_show_simple_shadow = true;
		
		#if UNITY_EDITOR && !IGNORE_EDITOR
		t_show_simple_shadow = true;
		#elif UNITY_STANDALONE
		t_show_simple_shadow = true;
		#elif UNITY_ANDROID
		return !QualityTool.GetBool( QualityTool.CONST_BATTLE_FIELD_SHADOW );
		#elif UNITY_IOS
		return BattleField_iOS_ShowSimpleShadow();
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return true;
		#endif
		
		#if SHOW_REAL_SHADOW
		t_show_simple_shadow = false;
		#endif
		
		return t_show_simple_shadow;
	}
	
	private static bool BattleField_iOS_ShowSimpleShadow(){
		if( !DeviceHelper.Is_iOS_Target_Device() ){
			return true;
		}
		
		#if UNITY_IOS
		return !QualityTool.GetBool( QualityTool.CONST_BATTLE_FIELD_SHADOW );
		#endif
		
		return false;
	}
	
	#endregion
	
	
	
	#region Shadow
	
	/// Add Shadows:
	/// 
	/// 1.Add Directional light;
	/// 2.Edit Light Color and Intensity, Edit Light Direction(transform);
	/// 3.Set Shadow Type: Hard Shadow, and Edit Shadow Strength;
	/// 4.Set Culling Mask: 3DLayer, 3D Shadow Ground;
	/// 5.Enable the GameObject, Disable the Light Components;
	public static void ConfigLights( bool p_active_light ){
		#if DEBUG_QUALITY
		Debug.Log( "ConfigLights( " + p_active_light + " )" );
		#endif
		
		Object[] t_objs = GameObject.FindObjectsOfType( typeof(Light) );
		
		//		Debug.Log( "Active Light's GameObject Count: " + t_objs.Length );
		
		for( int i = 0; i < t_objs.Length; i++ ){
			Light t_light = (Light)t_objs[ i ];
			
			t_light.enabled = p_active_light;
			
			#if DEBUG_QUALITY
			GameObjectHelper.LogGameObjectHierarchy( t_light.gameObject, i + " Light " );
			#endif
			
			int t_mask = 0;
			
			{
				int t_index = LayerMask.NameToLayer( "3D Shadow Ground" );
				
				t_mask += 1 << t_index;
			}
			
			{
				int t_index = LayerMask.NameToLayer( "3D Layer" );
				
				t_mask += 1 << t_index;
			}
			
			
			t_light.cullingMask = t_mask;
		}
		
		#if DEBUG_QUALITY
		Debug.Log( "ConfigLights Done." );
		#endif
	}
	
	#endregion

}
