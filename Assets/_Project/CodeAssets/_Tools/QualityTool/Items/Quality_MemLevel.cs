using UnityEngine;
using System.Collections;

public class Quality_MemLevel {

	public enum MemLevel{
		Low = 0,
		Medium,
		High,
	}

	// make it the same as old version
	private static MemLevel m_mem_level = MemLevel.High;

	#region Load

	public static void LoadMemLevel( string p_config_level ){
		if( p_config_level == CONST_MEM_LEVEL_LOW ){
			SetMemLevel( MemLevel.Low );
		}
		
		if( p_config_level == CONST_MEM_LEVEL_MEDIUM ){
			SetMemLevel( MemLevel.Medium );
		}

		if( p_config_level == CONST_MEM_LEVEL_HIGH ){
			SetMemLevel( MemLevel.High );
		}
	}

	#endregion



	#region Get Set

	public static void SetMemLevel( MemLevel p_scene_fx_level ){
		m_mem_level = p_scene_fx_level;
	}

	public static MemLevel GetMemLevel(){
		return m_mem_level;
	}

	#endregion

	
		
	#region Scene Fx

	/// Unload all unloadable items.
	public static bool IsMemLevelLow(){
		return GetMemLevel() == MemLevel.Low;
	}

	/// Retain, do nothing.
	public static bool IsMemLevelMedium(){
		return GetMemLevel() == MemLevel.Medium;
	}

	/// Do nothing.
	public static bool IsMemLevelHigh(){
		return GetMemLevel() == MemLevel.High;
	}
	
	#endregion

	
	
	#region Scene Fx Level Values
	
	public const string CONST_MEM_LEVEL_LOW			= "Low";
	
	public const string CONST_MEM_LEVEL_MEDIUM		= "Medium";
	
	public const string CONST_MEM_LEVEL_HIGH		= "High";
	
	#endregion
}
