using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class PlatformHelper {

	#region Reset

	public static void ResetPlatformSettings(){
		TimeHelper.ResetTimeScale();
	}

	#endregion



	#region Platform Path
	
	/* Return:	iOS/Android/Windows
     */
	public static string GetPlatformTag(){
		RuntimePlatform t_runtime_platform = Application.platform;
		
		string t_platform = "";
		
		if ( t_runtime_platform == RuntimePlatform.WindowsEditor ){
			t_platform = t_platform + GetAndroidTag();
		}
		else if ( t_runtime_platform == RuntimePlatform.Android ){
			t_platform = t_platform + GetAndroidTag();
		}
		else if ( t_runtime_platform == RuntimePlatform.OSXEditor ){
			t_platform = t_platform + GetiOSTag();
		}
		else if ( t_runtime_platform == RuntimePlatform.IPhonePlayer ){
			t_platform = t_platform + GetiOSTag();
		}
		else if ( t_runtime_platform == RuntimePlatform.WindowsPlayer ){
			t_platform = t_platform + GetWindowsTag();
		}
		else{
			Debug.LogError("TargetPlatform Error: " + t_runtime_platform);
		}
		
		return t_platform;
	}
	
	/// "Android"
	public static string GetAndroidTag(){
		return "Android";
	}
	
	/// "iOS"
	public static string GetiOSTag(){
		return "iOS";
	}
	
	public static string GetWindowsTag(){
		return "Windows";
	}
	
	#endregion

}
