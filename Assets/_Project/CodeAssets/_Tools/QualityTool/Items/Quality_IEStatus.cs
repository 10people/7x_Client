using UnityEngine;
using System.Collections;

public class Quality_IEStatus {

	public enum IEStatus{
		None = 0,
		Low,
		Medium,
		High,
	}

	// make it the same as old version
	private static IEStatus m_mem_level = IEStatus.High;

	#region Load

	public static void LoadIEStatusLevel( string p_config_level ){
		if( p_config_level == CONST_IE_STATUS_NONE ){
			SetMemLevel( IEStatus.None );
		}
	
		if( p_config_level == CONST_IE_STATUS_LOW ){
			SetMemLevel( IEStatus.Low );
		}
		
		if( p_config_level == CONST_IE_STATUS_MEDIUM ){
			SetMemLevel( IEStatus.Medium );
		}

		if( p_config_level == CONST_IE_STATUS_HIGH ){
			SetMemLevel( IEStatus.High );
		}
	}

	#endregion



	#region Get Set

	public static void SetMemLevel( IEStatus p_scene_fx_level ){
		m_mem_level = p_scene_fx_level;
	}

	public static IEStatus GetStatus(){
		return m_mem_level;
	}

	#endregion

	
		
	#region Scene Fx

	public static bool IsStatusNone(){
		return GetStatus() == IEStatus.None;
	}
	
	/// Unload all unloadable items.
	public static bool IsStatusLow(){
		return GetStatus() == IEStatus.Low;
	}

	/// Retain, do nothing.
	public static bool IsStatusMedium(){
		return GetStatus() == IEStatus.Medium;
	}

	/// Do nothing.
	public static bool IsStatusHigh(){
		return GetStatus() == IEStatus.High;
	}
	
	#endregion

	
	
	#region Scene Fx Level Values
	
	public const string CONST_IE_STATUS_NONE		= "None";
	
	public const string CONST_IE_STATUS_LOW			= "Low";
	
	public const string CONST_IE_STATUS_MEDIUM		= "Medium";
	
	public const string CONST_IE_STATUS_HIGH		= "High";
	
	#endregion
}
