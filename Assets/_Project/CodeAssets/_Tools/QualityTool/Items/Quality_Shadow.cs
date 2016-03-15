//#define DEBUG_QUALITY



#define DEVELOPMENT_SHADOW_TYPE



//#define SHOW_REAL_SHADOW

//#define SHOW_SIMPLE_SHADOW

//#define SHOW_NONE_SHADOW



using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class Quality_Shadow {

	#region InCity Shadow

	public static bool InCity_RealShadow(){
		bool t_show_shadow = true;

		#if UNITY_EDITOR && DEVELOPMENT_SHADOW_TYPE
		t_show_shadow = false;
		#elif UNITY_STANDALONE
		t_show_shadow = false;
		#elif UNITY_ANDROID
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_IN_CITY_SHADOW ),
		                                 SHADOW_HIGH );
		#elif UNITY_IOS
		if( !DeviceHelper.Is_iOS_Target_Device() ){
			return false;
		}
		
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_IN_CITY_SHADOW ),
		                                 SHADOW_HIGH );
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return true;
		#endif
		
		#if SHOW_REAL_SHADOW
		t_show_shadow = true;
		#endif

		#if SHOW_SIMPLE_SHADOW
		t_show_shadow = false;
		#endif

		#if SHOW_NONE_SHADOW
		t_show_shadow = false;
		#endif
		
		return t_show_shadow;
	}
	
	/// Show simple plane shadow incity or not.
	/// 
	/// Notes:
	/// 1.if not showed, never create the shadow.
	public static bool InCity_ShowSimpleShadow(){
		bool t_show_simple_shadow = true;

		#if UNITY_EDITOR && DEVELOPMENT_SHADOW_TYPE
		t_show_simple_shadow = true;
		#elif UNITY_STANDALONE
		t_show_simple_shadow = true;
		#elif UNITY_ANDROID
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_IN_CITY_SHADOW ),
		                                 SHADOW_LOW );
		#elif UNITY_IOS
		if( !DeviceHelper.Is_iOS_Target_Device() ){
			return true;
		}
		
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_IN_CITY_SHADOW ),
		                                 SHADOW_LOW );
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return true;
		#endif
		
		#if SHOW_REAL_SHADOW
		t_show_simple_shadow = false;
		#endif
		
		#if SHOW_SIMPLE_SHADOW
		t_show_simple_shadow = true;
		#endif
		
		#if SHOW_NONE_SHADOW
		t_show_simple_shadow = false;
		#endif
		
		return t_show_simple_shadow;
	}

	#endregion
	
	
	
	#region BattleField Shadow

	public static bool BattleField_RealShadow(){
		bool t_show_shadow = true;
		
		#if UNITY_EDITOR && DEVELOPMENT_SHADOW_TYPE
		t_show_shadow = false;
		#elif UNITY_STANDALONE
		t_show_shadow = false;
		#elif UNITY_ANDROID
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_BATTLE_FIELD_SHADOW ),
		                                 SHADOW_HIGH );
		#elif UNITY_IOS
		if( !DeviceHelper.Is_iOS_Target_Device() ){
			return false;
		}
		
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_BATTLE_FIELD_SHADOW ),
		                                 SHADOW_HIGH );
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return true;
		#endif
		
		#if SHOW_REAL_SHADOW
		t_show_shadow = true;
		#endif
		
		#if SHOW_SIMPLE_SHADOW
		t_show_shadow = false;
		#endif
		
		#if SHOW_NONE_SHADOW
		t_show_shadow = false;
		#endif
		
		return t_show_shadow;
	}

	/// Show simple plane shadow in battle field or not.
	/// 
	/// Notes:
	/// 1.if not showed, never create the shadow.
	public static bool BattleField_ShowSimpleShadow(){
		bool t_show_simple_shadow = true;
		
		#if UNITY_EDITOR && DEVELOPMENT_SHADOW_TYPE
		t_show_simple_shadow = true;
		#elif UNITY_STANDALONE
		t_show_simple_shadow = true;
		#elif UNITY_ANDROID
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_BATTLE_FIELD_SHADOW ),
		                                 SHADOW_LOW );
		#elif UNITY_IOS
		if( !DeviceHelper.Is_iOS_Target_Device() ){
			return true;
		}
		
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_BATTLE_FIELD_SHADOW ),
		                                 SHADOW_LOW );
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return true;
		#endif
		
		#if SHOW_REAL_SHADOW
		t_show_simple_shadow = false;
		#endif
		
		#if SHOW_SIMPLE_SHADOW
		t_show_simple_shadow = true;
		#endif
		
		#if SHOW_NONE_SHADOW
		t_show_simple_shadow = false;
		#endif
		
		return t_show_simple_shadow;
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

		#if UNITY_EDITOR
		EditorApplication.isPaused = true;
		#endif

		Object[] t_objs = GameObject.FindObjectsOfType( typeof(Light) );

		#if DEBUG_QUALITY
		Debug.Log( "Active Light's GameObject Count: " + t_objs.Length );
		#endif

		#if UNITY_EDITOR
		EditorApplication.isPaused = true;
		#endif

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



	#region Shadow Type

	public const string SHADOW_NONE		= "None";

	public const string SHADOW_LOW		= "Low";

	public const string SHADOW_HIGH		= "High";


	#endregion
}
