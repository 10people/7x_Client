//#define CUSTOM_DEBUG

//#define LOG_TO_CONSOLE

#if CUSTOM_DEBUG
using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class Debug {

	public static void Log( bool p_bool ) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log( p_bool );
		#endif
	}

	public static void Log( Vector3 p_vec3 ) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log( p_vec3 );
		#endif
	}

	public static void Log( int p_int ) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log( p_int );
		#endif
	}

	public static void Log( Object p_obj ){
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log( p_obj );
		#endif
	}

	public static void Log( System.Exception p_e ){
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log( p_e );
		#endif
	}

	public static void Log( float p_float ) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log( p_float );
		#endif
	}

	public static void Log( string message ) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log( message );
		#endif
	}

	public static void LogInfo(string message, UnityEngine.Object obj = null) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.Log(message, obj);
		#endif
	}
	
	public static void LogWarning(string message, UnityEngine.Object obj = null) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.LogWarning(message, obj);
		#endif
	}
	
	public static void LogError(string message, UnityEngine.Object obj = null) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.LogError(message, obj);
		#endif
	}
	
	public static void LogException(System.Exception e) {
		#if LOG_TO_CONSOLE
		UnityEngine.Debug.LogError(e.Message);
		#endif
	}
}
#endif
