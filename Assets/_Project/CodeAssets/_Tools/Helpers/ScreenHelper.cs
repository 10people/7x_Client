using UnityEngine;
using System.Collections;

public class ScreenHelper {

	public const int DesignWidth = 960;
	
	public const int DesignHeight = 640;

	#region Info
	
	public static Vector2 ScreenToDesign( Vector2 p_screen_coord ){
		return new Vector2(
			p_screen_coord.x / Screen.width * DesignWidth,
			p_screen_coord.y / Screen.height * DesignHeight );
	}

	public static float GetX( float p_percent ){
		return Screen.width * p_percent;
	}

	public static float GetY( float p_percent ){
		return Screen.height * p_percent;
	}

	public static float GetWidth( float p_percent ){
		return Screen.width * p_percent;
	}

	public static float GetHeight( float p_percent ){
		return Screen.height * p_percent;
	}

	public static float GetUIScale( int p_w, int p_h ){
		int t_width = Mathf.Max( 2, Screen.width );
		
		int t_height = Mathf.Max( 2, Screen.height );

		float t_scale_w = t_width * 1.0f / p_w;
		
		float t_scale_h = t_height * 1.0f / p_h;

		return t_scale_w > t_scale_h ? t_scale_h : t_scale_w;
	}

	public static float GetBGScale( int p_w, int p_h ){
		int t_width = Mathf.Max( 2, Screen.width );
		
		int t_height = Mathf.Max( 2, Screen.height );

		float t_scale_w = t_width * 1.0f / p_w;
		
		float t_scale_h = t_height * 1.0f / p_h;

		float t_final = t_scale_w > t_scale_h ? ( t_width / t_scale_h / p_w ) : ( t_height / t_scale_w / p_h );

		return t_final;
	}

	#endregion



	#region Log

	public static void LogScreen(){
		Debug.Log( "Screen w, h:" + Screen.width + ", " + Screen.height );
		
		Debug.Log( "Screen resolution: " + Screen.currentResolution );
	}
	
	public static void LogScreenInfo( GameObject p_child ){
		UIRoot t_root = NGUITools.FindInParents<UIRoot>( p_child.transform );
		
		Debug.Log( "Screen: " + Screen.width + ", " + Screen.height );
		
		Debug.Log( "Manual: " + t_root.manualWidth + ", " + t_root.manualHeight  );
		
		//		Debug.Log( "Active: " + t_root.activeWidth + ", " + t_root.activeHeight );
		
	}

	#endregion
}
