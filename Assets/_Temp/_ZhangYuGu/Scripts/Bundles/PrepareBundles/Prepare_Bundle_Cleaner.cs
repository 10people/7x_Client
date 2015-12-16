using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;


public class Prepare_Bundle_Cleaner : MonoBehaviour {

	#region Mono

	void Awake(){
		CleanBundleConfigs();
	}

	public static void CleanBundleConfigs(){
		Debug.Log( "Clear PlayerPrefs & Caching." );
		
		{
			CleanBundleVersionPrefs();
		}

		{
			BundleHelper.CleanBundleConfigs();
		}
		
		{
//				Debug.Log( "Delete Bundle Version File." );

			Prepare_Bundle_Config_4.CleanBundleUpdateList();
		}
	}

	private static void CleanBundleVersionPrefs(){
//		Debug.Log( "CleanBundleVersionPrefs()" );
		
		PlayerPrefs.DeleteKey( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION );
		
		PlayerPrefs.DeleteKey( ConstInGame.CONST_FIRST_TIME_TO_PLAY_VIDEO );
		
		PlayerPrefs.DeleteKey( ConstInGame.CONST_EXTRACT_BUNBLES_KEY );

		PlayerPrefs.Save();
	}

	#endregion
	
}