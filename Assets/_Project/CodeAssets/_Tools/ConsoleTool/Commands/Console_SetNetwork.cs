using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using System.IO;

using qxmobile;
using qxmobile.protobuf;

public class Console_SetNetwork {

	#region Ping

	// unity
	private static float m_on_ping_time 		= 0.0f;

	// cs
	private static long m_on_ping_time_long		= 0;

	private static bool m_log_ping				= true;
	
	public static void OnPing( string[] p_params, bool p_log_ping = true ){
		#if DEBUG_CONSOLE
		Debug.Log( "OnPing() " + Time.realtimeSinceStartup );
		#endif
		
		ErrorMessage t_msg = new ErrorMessage ();
		
		{
			t_msg.cmd = 1;
			
			t_msg.errorCode = 10;
			
			t_msg.errorDesc = Time.realtimeSinceStartup + "";
		}
		
		{
			m_on_ping_time = Time.realtimeSinceStartup;

			m_on_ping_time_long = TimeHelper.GetCurrentTime_MilliSecond();

			m_log_ping = p_log_ping;
		}

		if( m_log_ping ){
			Debug.Log( "Ping Time: " + TimeHelper.GetCurrentTime_MilliSecond() + "   - " + TimeHelper.GetCurrentTime_Second() );
		}

//		{
//			int t_count = 1000;
//			
//			for( int i = 0; i < t_count; i++ ){
//				for( int j = 0; j < t_count; j++ ){
//					float t_value = i * j;
//				}
//			}
//		}
		
		SocketHelper.SendQXMessage( t_msg, ProtoIndexes.DELAY_REQ );
	}
	
	public static void OnPingReceive( QXBuffer p_message, ErrorMessage p_msg ){
		#if DEBUG_CONSOLE
		Debug.Log( Time.realtimeSinceStartup + " OnPingReceive( " + p_msg.cmd + 
		          "   - " + p_msg.errorCode +
		          "   - " + p_msg.errorDesc + " )" );
		#endif

//		{
//			Debug.Log( "Message Create Time: " + p_message.GetCreateTime_MilliSecond() );
//
//			Debug.Log( "Message Create Time: " + p_message.GetCreateTime_Second() );
//			
//			Debug.Log( "Ping Sent Time: " + m_on_ping_time_long );
//		}

		// time from sent to receive
		long t_ping = p_message.GetCreateTime_MilliSecond() - m_on_ping_time_long;

		// time from receive to process
		long t_delay = (int)( ( Time.realtimeSinceStartup - m_on_ping_time ) * 1000 ) - t_ping;

		{
			NetworkHelper.SetPingMS( t_ping );
		}

		if( m_log_ping ){
			Debug.Log ( "Ping ms: " + t_ping  + "   Delay ms: " + t_delay );
		
			ChatPct tempChatPct = new ChatPct();
			
			tempChatPct.senderName = "Sys";
			
			tempChatPct.content = "Ping ms: " + t_ping + "   Delay ms: " + t_delay;

			tempChatPct.channel = ChatWindow.s_ChatWindow.CurrentChannel;
			
			ChatWindow.s_ChatWindow.GetChannelFrame(ChatWindow.s_ChatWindow.CurrentChannel).m_ChatBaseDataHandler.OnChatMessageReceived(tempChatPct );
		}
		
		
		
		//		Global.CreateBox( "Ping",
		//		                 "Duration: " + ( Time.realtimeSinceStartup - m_on_ping_time ),
		//		                 "",
		//		                 null,
		//		                 "OK",
		//		                 null,
		//		                 OnCloseBoxCallback );
	}
	
	public void OnCloseBoxCallback( int p_index ){
		#if DEBUG_CONSOLE
		Debug.Log( "OnCloseBoxCallback()" );
		#endif
	}
	
	#endregion
	
	
	
	#region Socket 
	
	public static void LogSocketProcessor( string[] p_params ){
		SocketTool.LogSocketProcessor ();
	}
	
	public static void LogSocketListener( string[] p_params ){
		SocketTool.LogSocketListener ();
	}
	
	#endregion



	#region MMO

	/// value stored for PreRunC.
	private static float m_pre_run_c		= 3.0f;
	
	public static void SetPreRunC( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		float t_param_1_c = 0;
		
		try{
			t_param_1_c = float.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		m_pre_run_c = t_param_1_c;
		
		Debug.Log( "PreRun C: " + m_pre_run_c );
	}

	/// Pre Run C for other players.
	public static float GetPreRunC(){
		return m_pre_run_c;
	}

	/// Value stored for ValidRunC.
	private static float m_valid_run_c		= 3.0f;

	public static void SetValidRunC( string[] p_params ){
		if( p_params.Length <= 1 ){
			Debug.LogError( "Error, params not enough." );
			
			return;
		}
		
		float t_param_1_c = 0;
		
		try{
			t_param_1_c = float.Parse( p_params[ 1 ] );
		}
		catch( Exception e ){
			StringHelper.LogStringArray( p_params );
			
			Debug.LogError( "Error, params error: " + e );
			
			return;
		}
		
		m_valid_run_c = t_param_1_c;
		
		Debug.Log( "ValidRun C: " + m_valid_run_c );
	}

	/// Valid Run C for local player config.
	public static float GetValidRunC(){
		return m_valid_run_c;
	}
	
	#endregion
}
