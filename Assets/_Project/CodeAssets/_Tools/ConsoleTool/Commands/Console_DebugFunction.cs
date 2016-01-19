using UnityEngine;
using System.Collections;
using System;

public class Console_DebugFunction {

	#region Debug Function
	
	public static void OnDebugFunction( string[] p_params ){
//		TestTimeString();


	}
		
	#endregion



	#region Sub Debug

	private static void TestTimeString(){
		Debug.Log( "3693: " + TimeHelper.GetUniformedTimeString( 3693 ) );

		Debug.Log( "1757: " + TimeHelper.GetUniformedTimeString( 1800 ) );

		Debug.Log( "61: " + TimeHelper.GetUniformedTimeString( 61 ) );

		Debug.Log( "27: " + TimeHelper.GetUniformedTimeString( 27 ) );
	}

	#endregion
}
