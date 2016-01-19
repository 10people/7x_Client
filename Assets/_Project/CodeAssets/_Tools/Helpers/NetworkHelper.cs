//#define CLOSE_CE_SHI_SERVER

//#define DEBUG_HUGE_UPDATE_SERVER

//#define DEBUG_PING



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkHelper {

	#region Network Status

	private static long m_ping_long_ms		= 0;

	public static float GetPingSec(){
		return m_ping_long_ms / 1000.0f;
	}

	public static float GetPingSecWithMin( float p_min_sec ){
		float t_cur_sec = GetPingSec();

		if( t_cur_sec > p_min_sec ){
			return t_cur_sec;
		}
		else{
			return p_min_sec;
		}
	}

	public static long GetPingMS(){
		return m_ping_long_ms;
	}

	public static void SetPingMS( long p_ping_ms ){
		if( p_ping_ms < 0 ){
			Debug.LogError( "Error in set ping ms: " + p_ping_ms );

			return;
		}

		#if DEBUG_PING
		Debug.Log( "SetPingMS( " + p_ping_ms + " )" );
		#endif

		m_ping_long_ms = p_ping_ms;
	}

	/// Pre Run C for other players.
	public static float GetPreRunC(){
		return Console_SetNetwork.GetPreRunC();
	}

	/// Valid Run C for local player config.
	public static float GetValidRunC(){
		return Console_SetNetwork.GetValidRunC();
	}

	#endregion


	
	#region Update Url
	
	private static string m_cur_update_server_url = "";
	
	public static void SetUpdateUrl( string p_url ){
		m_cur_update_server_url = p_url;

		#if DEBUG_HUGE_UPDATE_SERVER
		m_cur_update_server_url = NetworkHelper.UPDATE_URL_HUGE;
		#endif
	}
	
	public static string GetUpdateUrl(){
		return m_cur_update_server_url;
	}
	
	#endregion

	#region HTTP Server Prefex

	public static string GetPrefix(){
		string t_prefix = "";
		
		if( NetworkHelper.GetServerType() == NetworkHelper.ServerType.NeiWang ){
			t_prefix = SERVER_NEIWANG_PREFIX;
		}
		else if( NetworkHelper.GetServerType() == NetworkHelper.ServerType.TiYan ){
			t_prefix = SERVER_TIYAN_PREFIX;
		}
		else if( NetworkHelper.GetServerType() == NetworkHelper.ServerType.CeShi ){
			t_prefix = SERVER_CESHI_PREFIX;
		}
		else{
			Debug.LogError( "Error, Not Existed Server Type." );
		}

//		t_prefix = SERVER_CESHI_PREFIX;

//		t_prefix = SERVER_TIYAN_PREFIX;

//		t_prefix = SERVER_NEIWANG_PREFIX;

//		Debug.Log( "GetPrefix: " + t_prefix );
		
		return t_prefix;
	}
	
	private static string GetTiYanPrefix(){
		return SERVER_TIYAN_PREFIX;
	}
	
	private static string GetCeShiPrefix(){
		return SERVER_CESHI_PREFIX;
	}
	
	private static string GetNeiWangPrefix(){
		return SERVER_NEIWANG_PREFIX;
	}

	#endregion



	#region Default Server Type
	
	public enum ServerType{
		NeiWang = 0,
		TiYan,
		CeShi,
	}

	#if CLOSE_CE_SHI_SERVER
	// Now is the same as NeiWang
	private static ServerType m_server_type_enum 	= ServerType.NeiWang;
	
	private const string DEFAULT_SERVER_NAME 		= "内网服";

	private const SelectUrl.UrlSeclect m_default_select_url	= SelectUrl.UrlSeclect.NeiWang;
	#else
	private static ServerType m_server_type_enum 	= ServerType.CeShi;
	
	private const string DEFAULT_SERVER_NAME 		= "测试服";

	private const SelectUrl.UrlSeclect m_default_select_url	= SelectUrl.UrlSeclect.CeShi;
	#endif


	
	// login use
	public static ServerType GetDefaultServerType(){
		return m_server_type_enum;
	}
	
	// login use
	public static SelectUrl.UrlSeclect GetDefaultLoginServerType(){
		return m_default_select_url;
	}
	
	// login use
	public static string GetDefaultServerName(){
		return DEFAULT_SERVER_NAME;
	}
	
	public static ServerType GetServerType(){
		return m_server_type_enum;
	}
	
	public static void SetServerType( ServerType p_type ){
		m_server_type_enum = p_type;
		
//		Debug.Log ( "ServerType.Setted: " + p_type );
	}
	
	#endregion

	
	
	#region Server Url Prefix
	
	// NeiWang Develop server
	private const string SERVER_NEIWANG_PREFIX		= "http://192.168.3.80:8090/";
	
	// TiYan Public server
	private const string SERVER_TIYAN_PREFIX		= "http://203.195.230.100:9090/";
	
	
	#if CLOSE_CE_SHI_SERVER
	// Now is the same as NeiWang
	private const string SERVER_CESHI_PREFIX		= "http://192.168.3.80:8090/";
	#else
	// Ceshi develop test server
	private const string SERVER_CESHI_PREFIX		= "http://203.195.230.100:9091/";
	#endif
	
	public const string SERVER_HUGE_PREFIX			= "http://192.168.0.176:8080/";

	#endregion



	#region Update Server

	/// Update Server
	public const string UPDATE_URL_NEIWANG			= "http://192.168.3.80:8070/wsRes/compare1.1.jsp";
	
	public const string UPDATE_URL_TIYAN			= "http://203.195.230.100:9090/wsRes/compare1.1.jsp";
	
	public const string UPDATE_URL_CESHI			= "http://203.195.230.100:8010/wsRes/compare1.1.jsp";

	public const string UPDATE_URL_HUGE				= SERVER_HUGE_PREFIX + "wsRes/compare1.1.jsp";

	#endregion


	
	#region Request Url
	
	// develop register
	public const string REGISTER_URL					= "qxrouter/accountReg.jsp";
	
	// develop login
	public const string LOGIN_URL						= "qxrouter/accountLogin.jsp";
	
	// verify 3rd platform login 
	public const string THIRD_PLATFORM_LOGIN_URL		= "qxrouter/channel/checkLogin.jsp";
	
	// report unsupported device to server
	public const string REPORT_UNSUPPORT_DEVICE_URL		= "qxrouter/client/clientTooLow.jsp";
	
	/// 公告
	public const string NOTICE_URL 						= "qxrouter/sysNotice.jsp";	
	
	
	
	// report client info to server
	public const string OPERATION_SUPPORT_REPORT_ACTION_URL 	= "qxrouter/client/reportAction.jsp";
	
	
	#endregion
}
