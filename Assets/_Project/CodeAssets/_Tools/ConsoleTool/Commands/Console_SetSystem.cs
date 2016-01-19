using UnityEngine;
using System.Collections;
using System;

public class Console_SetSystem {

	#region GC
	
	public static void GC( string[] p_params ){
		UtilityTool.UnloadUnusedAssets();
	}

	public static void LogScreen( string[] p_params ){
		ScreenHelper.LogScreen();
	}

	public static void Vibrate( string[] p_params ){
#if UNITY_ANDROID || UNITY_IOS
		Debug.Log( "Vibrate()" );

		Handheld.Vibrate();
#endif
	}
	
	#endregion
}
