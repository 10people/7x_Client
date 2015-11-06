using UnityEngine;
using System.Collections;

public class ScreenTool : MonoBehaviour {

	public const int DesignWidth = 960;
	
	public const int DesignHeight = 640;
	
	
	public static Vector2 ScreenToDesign( Vector2 p_screen_coord ){
		return new Vector2(
			p_screen_coord.x / Screen.width * DesignWidth,
			p_screen_coord.y / Screen.height * DesignHeight );
	}

	public static void LogScreenInfo( GameObject p_child ){
		UIRoot t_root = NGUITools.FindInParents<UIRoot>( p_child.transform );
		
		Debug.Log( "Screen: " + Screen.width + ", " + Screen.height );
		
		Debug.Log( "Manual: " + t_root.manualWidth + ", " + t_root.manualHeight  );
		
//		Debug.Log( "Active: " + t_root.activeWidth + ", " + t_root.activeHeight );

	}

	public static float GetX( float p_percent ){
		return Screen.width * p_percent;
	}

	public static float GetY( float p_percent ){
		return Screen.height * p_percent;
	}
}
