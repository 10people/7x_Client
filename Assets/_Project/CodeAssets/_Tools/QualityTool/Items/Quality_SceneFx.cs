using UnityEngine;
using System.Collections;

public class Quality_SceneFx {

	public enum SceneFxLevel{
		None = 0,
		Low,
		Medium,
		High,
	}

	private static SceneFxLevel m_scene_fx_level = SceneFxLevel.None;

	#region Load

	public static void LoadSceneFxLevel( string p_config_level ){
		if( p_config_level == CONST_SCENE_FX_LEVEL_NONE ){
			SetSceneFxLevel( SceneFxLevel.None );
		}
		
		if( p_config_level == CONST_SCENE_FX_LEVEL_LOW ){
			SetSceneFxLevel( SceneFxLevel.Low );
		}
		
		if( p_config_level == CONST_SCENE_FX_LEVEL_MEDIUM ){
			SetSceneFxLevel( SceneFxLevel.Medium );
		}

		if( p_config_level == CONST_SCENE_FX_LEVEL_HIGH ){
			SetSceneFxLevel( SceneFxLevel.High );
		}
	}

	#endregion



	#region Get Set

	public static void SetSceneFxLevel( SceneFxLevel p_scene_fx_level ){
		m_scene_fx_level = p_scene_fx_level;
	}

	public static SceneFxLevel GetSceneFxLevel(){
		return m_scene_fx_level;
	}

	#endregion

	
		
	#region Scene Fx
	
	/// None Scene Fx
	public static bool IsSceneFxNone(){
		return GetSceneFxLevel() == SceneFxLevel.None;
	}
	
	public static bool IsSceneFxLow(){
		return GetSceneFxLevel() == SceneFxLevel.Low;
	}
	
	public static bool IsSceneFxMedium(){
		return GetSceneFxLevel() == SceneFxLevel.Medium;
	}
	
	public static bool IsSceneFxHigh(){
		return GetSceneFxLevel() == SceneFxLevel.High;
	}
	
	#endregion

	
	
	#region Scene Fx Level Values
	
	public const string CONST_SCENE_FX_LEVEL_NONE		= "None";
	
	public const string CONST_SCENE_FX_LEVEL_LOW		= "Low";
	
	public const string CONST_SCENE_FX_LEVEL_MEDIUM		= "Medium";
	
	public const string CONST_SCENE_FX_LEVEL_HIGH		= "High";
	
	#endregion
}
