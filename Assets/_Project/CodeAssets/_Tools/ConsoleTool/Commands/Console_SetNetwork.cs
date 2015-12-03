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
	private static float m_on_ping_time 	= 0.0f;

	// cs
	private static long m_on_ping_time_long	= 0;
	
	public static void OnPing( string[] p_params ){
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

			m_on_ping_time_long = TimeHelper.GetCurrentTimeMillis();
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
//			Debug.Log( "Message Create Time: " + p_message.GetTimeMillis() );
//			
//			Debug.Log( "Ping Sent Time: " + m_on_ping_time_long );
//		}

		// time from sent to receive
		long t_ping = p_message.GetTimeMillis() - m_on_ping_time_long;

		// time from receive to process
		long t_delay = (int)( ( Time.realtimeSinceStartup - m_on_ping_time ) * 1000 ) - t_ping;

		{

			Debug.Log ( "Ping ms: " + t_ping  + "   Delay ms: " + t_delay );
		}

		{
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
}
