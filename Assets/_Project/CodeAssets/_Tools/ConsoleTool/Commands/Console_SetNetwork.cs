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
	
	private static float m_on_ping_time = 0.0f;
	
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
		}
		
		SocketHelper.SendQXMessage( t_msg, ProtoIndexes.DELAY_REQ );
	}
	
	public static void OnPingReceive( ErrorMessage p_msg ){
		#if DEBUG_CONSOLE
		Debug.Log( Time.realtimeSinceStartup + " OnPingReceive( " + p_msg.cmd + 
		          "   - " + p_msg.errorCode +
		          "   - " + p_msg.errorDesc + " )" );
		#endif
		
		Debug.Log ( "Ping Duration: " + ( Time.realtimeSinceStartup - m_on_ping_time ) );
		
		
		{
			ChatPct tempChatPct = new ChatPct();
			
			tempChatPct.senderName = "Sys";
			
			tempChatPct.content = "Delay: " + (Time.realtimeSinceStartup - m_on_ping_time);
			
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
