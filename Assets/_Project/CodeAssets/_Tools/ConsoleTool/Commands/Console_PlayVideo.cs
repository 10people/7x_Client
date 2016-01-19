using UnityEngine;
using System.Collections;
using System;

public class Console_PlayVideo : MonoBehaviour {

	#region Play Video

	/// /playvideo Video/x7.mp4 none
	public static void PlayVideo( string[] p_params ){
		if( p_params.Length < 3 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}

//		string t_path = "Video/x7.mp4";
		string t_param_1_path = "";

		string t_param_2_mode = "";
		
		try{
			t_param_1_path = p_params[ 1 ];

			t_param_2_mode = p_params[ 2 ];
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		VideoHelper.VideoControlMode t_mode = VideoHelper.VideoControlMode.Full;

		if( StringHelper.IsLowerEqual( t_param_2_mode, "None" ) ){
			t_mode = VideoHelper.VideoControlMode.None;
		}
		else if( StringHelper.IsLowerEqual( t_param_2_mode, "NBC" ) ){
			t_mode = VideoHelper.VideoControlMode.NoneButCancelable;
		}
		else if( StringHelper.IsLowerEqual( t_param_2_mode, "Min" ) ){
			t_mode = VideoHelper.VideoControlMode.Minimal;
		}
		else if( StringHelper.IsLowerEqual( t_param_2_mode, "Full" ) ){
			t_mode = VideoHelper.VideoControlMode.Full;
		}

		Debug.Log( "string path: " + t_param_1_path );

		Debug.Log( "string mode: " + t_param_2_mode );

		Debug.Log( "control mode: " + t_mode );

		VideoHelper.PlayVideo( t_param_1_path, t_mode, VideoPlayDone );
	}

	public static void VideoPlayDone(){
		Debug.Log( "VideoPlayDone()" );
	}

	#endregion



}
