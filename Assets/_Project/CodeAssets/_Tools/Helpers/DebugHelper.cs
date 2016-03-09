//#define CUSTOM_DEBUG

//#define LOG_TO_CONSOLE



using System;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using System.Collections;
using System.Collections.Generic;



public class DebugHelper{

	#region Common Code Error Utility
	
	private static string m_common_code_error_tag = "";
	
	private static string m_common_code_error_content = "";
	
	private static string m_common_code_error = "";
	
	public static Rect m_common_code_scroll_rect = new Rect(0, 110, 960, 600);
	
	public static Rect m_common_code_scroll_content_rect = new Rect(0, 0, 960, 640);
	
	public static Vector2 m_common_code_scroll_pos = Vector2.zero;
	
	public static void SetCommonCodeError(string p_tag, string p_content)
	{
		m_common_code_error = "Tag: " + p_tag + "\n" +
			"Content: " + p_content;
		
		{
			m_common_code_scroll_rect.width = Screen.width * 0.8f;
			
			m_common_code_scroll_rect.height = Screen.height * 0.5f;
		}
		
		{
			m_common_code_scroll_content_rect.width = Screen.width;
			
			m_common_code_scroll_content_rect.height = Screen.height;
		}
	}
	
	public static void ClearCommonCodeError(){
		m_common_code_error = "";
	}
	
	public static string GetCommonCodeError(){
		return m_common_code_error;
	}
	
	#endregion



	#region Screen Log

	public static Vector2 m_screen_log_scroll_pos 	= Vector2.zero;

	private static List<string> m_screen_log_list	= new List<string>();

	private const int MAX_LIST_LEN					= 100;

	private static string m_str_final				= "";

	public static void LogScreen( string p_log, string p_stack, LogType p_type ){
		if( !ConfigTool.GetBool( ConfigTool.CONST_SHOW_SCREEN_LOG ) ){
			return;
		}

		if( m_screen_log_list.Count >= MAX_LIST_LEN ){
			m_screen_log_list.RemoveAt( 0 );
		}

		string t_log_string = p_type + ": " + p_log + 
			"\n" + p_stack + 
			"\n";

		m_screen_log_list.Add( t_log_string );

		UpdateStrFinal();
	}

	public static string GetLogString(){
		return m_str_final;
	}

	private static void UpdateStrFinal(){
		StringBuilder t_builder = new StringBuilder();

		for( int i = m_screen_log_list.Count - 1; i >= 0; i-- ){
			t_builder.Append( m_screen_log_list[ i ] );
		}

		m_str_final = t_builder.ToString();
	}

	public static void RegisterLog(){
		Application.logMessageReceived -= DebugHelper.LogScreen;

		Application.logMessageReceived += DebugHelper.LogScreen;
	}

	#endregion



	#region Utilities


	#endregion
}



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
