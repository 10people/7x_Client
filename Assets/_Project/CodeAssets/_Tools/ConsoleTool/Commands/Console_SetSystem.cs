using UnityEngine;
using System.Collections;
using System;

public class Console_SetSystem {

	#region GC
	
	public static void GC( string[] p_params ){
		UtilityTool.UnloadUnusedAssets();
	}
	
	#endregion
}
