
using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class LogHelper : MonoBehaviour {

	
	
	
	
	#region Log
	
	public static void LogBytes(byte[] p_bytes, string p_tag = "")
	{
		if (p_bytes == null)
		{
			Debug.Log(p_tag + " is null.");
			
			return;
		}
		
		for (int i = 0; i < p_bytes.Length; i++)
		{
			Debug.Log(p_tag + "[ " + i + " ]: " + p_bytes[i]);
		}
	}
	
	public static void LogFloats(float[] p_floats, string p_tag = "")
	{
		if (p_floats == null)
		{
			Debug.Log(p_tag + " is null.");
			
			return;
		}
		
		for (int i = 0; i < p_floats.Length; i++)
		{
			Debug.Log(p_tag + "[ " + i + " ]: " + p_floats[i]);
		}
	}
	
	public static void LogStrings(List<string> p_string_list, string p_tag = "")
	{
		if (p_string_list == null)
		{
			Debug.Log(p_tag + " is null.");
			
			return;
		}
		
		for (int i = 0; i < p_string_list.Count; i++)
		{
			Debug.Log(p_tag + "[ " + i + " ]: " + p_string_list[i]);
		}
	}
	
	public static void LogStrings(string[] p_strings, string p_tag = "")
	{
		if (p_strings == null)
		{
			Debug.Log(p_tag + " is null.");
			
			return;
		}
		
		for (int i = 0; i < p_strings.Length; i++)
		{
			Debug.Log(p_tag + "[ " + i + " ]: " + p_strings[i]);
		}
	}
	
	public static void LogVector3(string p_tag, Vector3 p_vec3)
	{
		Debug.Log(p_tag + " : " + p_vec3);
	}
	
	#if UNITY_EDITOR || UNITY_STANDALONE
	public static void LogMovieInfo(string p_tag, MovieTexture p_movie)
	{
		Debug.Log("----------------Movie Info: " + p_tag + " --------------------");
		
		Debug.Log("Waiting Movie - size: " + p_movie.width + ", " + p_movie.height +
		          "   ReadyToPlay: " + p_movie.isReadyToPlay +
		          "   IsPlaying: " + p_movie.isPlaying +
		          "   duration: " + p_movie.duration);
	}
	#endif
	
	
	
	public static void LogCharacterController(CharacterController p_character_controller)
	{
		Debug.Log("Character.Info - IsGrounded: " + p_character_controller.isGrounded);
		
		if (p_character_controller.collisionFlags == CollisionFlags.None)
		{
			Debug.Log("Free floating!");
		}
		
		if ((p_character_controller.collisionFlags & CollisionFlags.Sides) != 0)
		{
			Debug.Log("Touching sides!");
		}
		
		if (p_character_controller.collisionFlags == CollisionFlags.Sides)
		{
			Debug.Log("Only touching sides, nothing else!");
		}
		
		if ((p_character_controller.collisionFlags & CollisionFlags.Above) != 0)
		{
			Debug.Log("Touching sides!");
		}
		
		if (p_character_controller.collisionFlags == CollisionFlags.Above)
		{
			Debug.Log("Only touching Ceiling, nothing else!");
		}
		
		if ((p_character_controller.collisionFlags & CollisionFlags.Below) != 0)
		{
			Debug.Log("Touching ground!");
		}
		
		if (p_character_controller.collisionFlags == CollisionFlags.Below)
		{
			Debug.Log("Only touching ground, nothing else!");
		}
	}
	
	#endregion



	#region Utilities

	public static void LogRunTime(){
		#if UNITY_EDITOR
		Type type = Type.GetType( "Mono.Runtime" );
		if( type != null ){
			MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
			if ( displayName != null ){
				Debug.Log( displayName.Invoke( null, null ) );
			}
			else{
				Debug.LogError( "Error in Get RunTime." );
			}
		}
		#endif
	}

	#endregion
}
